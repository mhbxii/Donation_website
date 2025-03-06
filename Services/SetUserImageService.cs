using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using dotnet9.Data;
using dotnet9.Dtos.Models;
using dotnet9.Interfaces;

namespace dotnet9.Services
{
    public class SetUserImageService : ISetUserImageService
    {
        private readonly AppDbContext _context;
        private readonly IImageUploadService _imageUploadService;
        private readonly Cloudinary _cloudinary;

        public SetUserImageService(AppDbContext context, IImageUploadService imageUploadService, IConfiguration configuration)
        {
            _context = context;
            _imageUploadService = imageUploadService;
            // Initialize Cloudinary for deletion as well.
            var account = new Account(
                configuration["Cloudinary:CloudName"],
                configuration["Cloudinary:ApiKey"],
                configuration["Cloudinary:ApiSecret"]);
            _cloudinary = new Cloudinary(account);
        }

        // Helper to extract Cloudinary public ID from the secure URL.
        private string ExtractPublicIdFromUrl(string imageUrl)
        {
            // Example URL: https://res.cloudinary.com/your_cloud_name/image/upload/v1234567890/Donation/abc123.jpg
            // We split by "upload/" and then remove the version and extension.
            var parts = imageUrl.Split("upload/");
            if (parts.Length < 2)
                throw new ArgumentException("Invalid Cloudinary URL format.");

            var afterUpload = parts[1]; // e.g., "v1234567890/Donation/abc123.jpg"
            var segments = afterUpload.Split('/');
            // Skip the version segment if it starts with "v"
            int startIndex = segments[0].StartsWith("v") ? 1 : 0;
            var publicIdWithExtension = string.Join("/", segments.Skip(startIndex));
            var dotIndex = publicIdWithExtension.LastIndexOf('.');
            var publicId = dotIndex > 0 ? publicIdWithExtension.Substring(0, dotIndex) : publicIdWithExtension;
            return publicId;
        }

        public async Task<UserDto?> SetUserImageAsync(Guid userId, IFormFile file)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
                return null;

            // If an old image exists, attempt to delete it.
            if (!string.IsNullOrEmpty(user.ImageUrl))
            {
                try
                {
                    var publicId = ExtractPublicIdFromUrl(user.ImageUrl);
                    var deletionParams = new DeletionParams(publicId);
                    var deletionResult = await _cloudinary.DestroyAsync(deletionParams);
                    // Optionally, you can log deletionResult.Error if deletion fails.
                }
                catch (Exception ex)
                {
                    // Log the error if needed but continue with the new upload.
                }
            }

            var newImageUrl = await _imageUploadService.UploadImageAsync(file);
            if (string.IsNullOrEmpty(newImageUrl))
                return null;

            user.ImageUrl = newImageUrl;
            await _context.SaveChangesAsync();

            return new UserDto
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                ImageUrl = user.ImageUrl,
                PhoneNumber = user.PhoneNumber,
                Origin = user.Origin,
            };
        }
    }
}
