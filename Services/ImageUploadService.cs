using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using dotnet9.Dtos.Models;
using dotnet9.Interfaces;
using dotnet9.Repositories.Interfaces;

namespace dotnet9.Services
{
    public class ImageUploadService : IImageUploadService
    {
        private readonly Cloudinary _cloudinary;
        public ImageUploadService(IConfiguration configuration, IImageRepository imageRepo)
        {
            // Initialize Cloudinary using configuration values.
            var account = new Account(
                configuration["Cloudinary:CloudName"],
                configuration["Cloudinary:ApiKey"],
                configuration["Cloudinary:ApiSecret"]);
            _cloudinary = new Cloudinary(account);
        }

        public async Task<string?> UploadImageAsync(IFormFile file)
        {
            try{
                // Validate the uploaded file.
                if (file == null || file.Length == 0)
                    throw new ArgumentException("No file uploaded.");

                if (!file.ContentType.StartsWith("image/"))
                    throw new ArgumentException("File is not a valid image.");

                // Enforce a 2MB file size limit.
                if (file.Length > 5 * 1024 * 1024)
                    throw new ArgumentException("File size exceeds 5MB limit.");

                // Set up Cloudinary upload parameters with basic transformations.
                var uploadParams = new ImageUploadParams
                {
                    File = new FileDescription(file.FileName, file.OpenReadStream()),
                    Folder = "Donation",
                    Transformation = new Transformation().Crop("fill")       // Crop to fill the dimensions.
                                                         .Width(300)         // Set desired width.
                                                         .Height(300)        // Set desired height.
                                                         .Quality("auto").FetchFormat("auto")
                };

                // Upload the image to Cloudinary.
                var uploadResult = await _cloudinary.UploadAsync(uploadParams);

                if (uploadResult.Error != null)
                    throw new Exception($"Cloudinary error: {uploadResult.Error.Message}");
                
                // Get the secure URL for the uploaded image.
                var imageUrl = uploadResult.SecureUrl.ToString();

                return imageUrl;
            }catch(Exception ex)
            {
                return null;
            }
        }
    }
}
