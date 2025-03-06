using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using dotnet9.Data;
using dotnet9.Dtos.Models;
using dotnet9.Interfaces;
using dotnet9.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace dotnet9.Services
{
    public class ArticleMgmtService : IArticleMgmtService
    {
        private readonly IArticleRepository _articleRepo;
        private readonly IArticleImageRepository _articleImageRepo;
        private readonly IImageUploadService _imageUploadService;
        private readonly AppDbContext _context;
        private readonly Cloudinary _cloudinary;

        public ArticleMgmtService(IArticleRepository articleRepo, IArticleImageRepository articleImageRepo, IImageUploadService imageUploadService, AppDbContext appDbContext, IConfiguration configuration)
        {
            _articleRepo = articleRepo;
            _articleImageRepo = articleImageRepo;
            _imageUploadService = imageUploadService;
            _context = appDbContext;

            var account = new Account(
                configuration["Cloudinary:CloudName"],
                configuration["Cloudinary:ApiKey"],
                configuration["Cloudinary:ApiSecret"]);
            _cloudinary = new Cloudinary(account);
        }

        public async Task<ArticleDto> CreateArticleWithImagesAsync(ArticleDto articleDto, List<IFormFile> imageFiles)
        {
            // Create the article first.
            using var transaction = await _context.Database.BeginTransactionAsync();

            if(imageFiles.Count > 5)
                throw new Exception("5 Images Max!");

            try{
                var createdArticle = await _articleRepo.AddAsync(articleDto);

                // Upload each image and attach it to the article.
                foreach (var file in imageFiles)
                {
                    // Upload the image to Cloudinary and get the URL.
                    var imageUrl = await _imageUploadService.UploadImageAsync(file);
                    if (!string.IsNullOrEmpty(imageUrl))
                    {
                        // Save the image record.
                        var imageDto = new ArticleImageDto
                        {
                            ArticleId = createdArticle.Id,
                            ImageUrl = imageUrl
                        };
                        await _articleImageRepo.AddAsync(imageDto);
                    }else
                    {
                        // If any image upload fails, throw an exception.
                        throw new Exception("Image upload failed. Please check your internet connection and try again.");
                    }
                }
                //Save Images
                await _context.SaveChangesAsync();
                // Commit the transaction if all operations succeeded.
                await transaction.CommitAsync();
                return createdArticle;
                
            }catch(Exception e){
                await transaction.RollbackAsync();
                throw;
            }
        }

        // New method to delete an article and its associated Cloudinary images.
        public async Task<bool> DeleteArticleAndImagesAsync(Guid articleId)
        {
            // Load article with its images.
            var article = await _context.Articles
                .Include(a => a.ArticleImages)
                .FirstOrDefaultAsync(a => a.Id == articleId);
            if (article == null)
                return false;

            // Delete each associated image from Cloudinary.
            foreach (var image in article.ArticleImages!)
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
            // Delete the article; cascade will remove the image records from the DB.
            _context.Articles.Remove(article);
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
