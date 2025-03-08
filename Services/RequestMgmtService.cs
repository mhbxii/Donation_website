using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using dotnet9.Data;
using dotnet9.Dtos.Models;
using dotnet9.Interfaces;
using dotnet9.Models;
using dotnet9.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace dotnet9.Services
{
    public class RequestMgmtService : IRequestMgmtService
    {
        private readonly IRequestRepository _RequestRepo;
        private readonly IImageRepository _ImageRepo;
        private readonly IImageUploadService _imageUploadService;
        private readonly AppDbContext _context;
        private readonly Cloudinary _cloudinary;

        public RequestMgmtService(IRequestRepository RequestRepo, IImageRepository ImageRepo, IImageUploadService imageUploadService, AppDbContext appDbContext, IConfiguration configuration)
        {
            _RequestRepo = RequestRepo;
            _ImageRepo = ImageRepo;
            _imageUploadService = imageUploadService;
            _context = appDbContext;

            var account = new Account(
                configuration["Cloudinary:CloudName"],
                configuration["Cloudinary:ApiKey"],
                configuration["Cloudinary:ApiSecret"]);
            _cloudinary = new Cloudinary(account);
        }

        public async Task<RequestDto> CreateRequestWithImagesAsync(RequestDto requestDto, List<IFormFile>? imageFiles)
        {
            // Create the Request first.
            using var transaction = await _context.Database.BeginTransactionAsync();

            if(imageFiles?.Count > 5)
                throw new Exception("5 Images Max!");

            try{

                var article = await _context.Articles.FindAsync(requestDto.ArticleId);
                if(article == null)
                    throw new Exception("There's no article with this Id!");
                
                if(requestDto.Quantity > article.Quantity)
                    throw new Exception("Quantity Insufficiant!");
            
                if(article.UserId == requestDto.UserId)
                    throw new Exception("Error: You're already the owner of the article!.");

                var already = await _context.Requests.FirstOrDefaultAsync(r => r.ArticleId == requestDto.ArticleId && r.UserId == requestDto.UserId);

                if(already != null)
                    throw new Exception("Request already exists!.");

                
               

                var createdRequest = await _RequestRepo.AddAsync(requestDto);


                // Upload each image and attach it to the Request.
                if(imageFiles != null){
                    foreach (var file in imageFiles)
                    {
                        // Upload the image to Cloudinary and get the URL.
                        var imageUrl = await _imageUploadService.UploadImageAsync(file);
                        if (!string.IsNullOrEmpty(imageUrl))
                        {
                            // Save the image record.
                            var imageDto = new ImageDto
                            {
                                ParentId = createdRequest.Id,
                                ImageUrl = imageUrl
                            };
                            await _ImageRepo.AddAsync(imageDto);
                            //add it to the final result:
                            createdRequest.RequestImages!.Add(imageUrl);
                        }else
                        {
                            // If any image upload fails, throw an exception.
                            throw new Exception("Image upload failed. Please check your internet connection and try again.");
                        }
                    }
                }
                //Save Images
                await _context.SaveChangesAsync();
                // Commit the transaction if all operations succeeded.
                await transaction.CommitAsync();
                return createdRequest;
                
            }catch(Exception e){
                await transaction.RollbackAsync();
                throw;
            }
        }

        // New method to delete an Request and its associated Cloudinary images.
        public async Task<bool> DeleteRequestAndImagesAsync(Guid RequestId)
        {
            // Load Request with its images.
            var Request = await _context.Requests.FindAsync(RequestId);

            if (Request == null)
                return false;

            Request.RequestImages = await _context.Images
                                    .Where(i => i.ParentId == Request.Id)  // Assumes only request images have ParentId matching request.Id
                                    .ToListAsync();
                
            // Delete each associated image from Cloudinary.
            foreach (var image in Request.RequestImages!)
            {
                if (!string.IsNullOrEmpty(image.ImageUrl))
                {
                    try
                    {
                        var publicId = ExtractPublicIdFromUrl(image.ImageUrl);
                        var deletionParams = new DeletionParams(publicId);
                        var deletionResult = await _cloudinary.DestroyAsync(deletionParams);
                        // Optionally log deletionResult.Error if needed.
                    }
                    catch (Exception ex)
                    {
                        // Log the error and continue (or decide to abort the deletion).
                    }
                }
            }
            // Delete the Request; cascade will remove the image records from the DB.
            _context.Requests.Remove(Request);
            var result = await _context.SaveChangesAsync();
            return result > 0;
        }

        // Helper method to extract Cloudinary public ID from the image URL.
        private string ExtractPublicIdFromUrl(string imageUrl)
        {
            // Example URL: https://res.cloudinary.com/your_cloud_name/image/upload/v1234567890/Donation/abc123.jpg
            // Split by "upload/" to isolate the portion after it.
            var parts = imageUrl.Split("upload/");
            if (parts.Length < 2)
                throw new ArgumentException("Invalid Cloudinary URL format.");

            var afterUpload = parts[1]; // e.g., "v1234567890/Donation/abc123.jpg"
            var segments = afterUpload.Split('/');
            // If the first segment is a version (starts with "v"), skip it.
            int startIndex = segments[0].StartsWith("v") ? 1 : 0;
            var publicIdWithExtension = string.Join("/", segments.Skip(startIndex));
            var dotIndex = publicIdWithExtension.LastIndexOf('.');
            var publicId = dotIndex > 0 ? publicIdWithExtension.Substring(0, dotIndex) : publicIdWithExtension;
            return publicId;
        }
    }
}
