using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;

namespace CMS.Server.Services
{
    public class CloudinaryImageStorageService : IImageStorageService
    {
        private readonly Cloudinary _cloudinary;
        private readonly string _folder;

        public CloudinaryImageStorageService(IConfiguration configuration)
        {
            // Initialize Cloudinary with your credentials
            var cloudName = configuration["Cloudinary:CloudName"];
            var apiKey = configuration["Cloudinary:ApiKey"];
            var apiSecret = configuration["Cloudinary:ApiSecret"];

            if (string.IsNullOrEmpty(cloudName) || string.IsNullOrEmpty(apiKey) || string.IsNullOrEmpty(apiSecret))
            {
                throw new ArgumentException("Cloudinary configuration is missing or incomplete");
            }

            var account = new Account(cloudName, apiKey, apiSecret);
            _cloudinary = new Cloudinary(account);
            _folder = "fancy-collection"; // Optional folder name within your Cloudinary account
        }

        public async Task<string> UploadImageAsync(IFormFile file)
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException("File is empty");

            using var stream = file.OpenReadStream();

            // Create upload parameters
            var uploadParams = new ImageUploadParams
            {
                File = new FileDescription(file.FileName, stream),
                Folder = _folder,
                UseFilename = true,
                UniqueFilename = true,
                Overwrite = false
            };

            // Upload to Cloudinary
            var uploadResult = await _cloudinary.UploadAsync(uploadParams);

            if (uploadResult.Error != null)
            {
                throw new Exception($"Failed to upload image to Cloudinary: {uploadResult.Error.Message}");
            }

            // Return the secure URL of the uploaded image
            return uploadResult.SecureUrl.ToString();
        }

        public async Task DeleteImageAsync(string imageUrl)
        {
            if (string.IsNullOrEmpty(imageUrl))
                return;

            try
            {
                // Extract the public ID from the URL
                var publicId = GetPublicIdFromUrl(imageUrl);

                if (string.IsNullOrEmpty(publicId))
                    return;

                // Delete from Cloudinary
                var deleteParams = new DeletionParams(publicId);
                var result = await _cloudinary.DestroyAsync(deleteParams);

                if (result.Error != null)
                {
                    Console.WriteLine($"Error deleting image from Cloudinary: {result.Error.Message}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting image from Cloudinary: {ex.Message}");
            }
        }

        private string GetPublicIdFromUrl(string url)
        {
            try
            {
                // Parse the Cloudinary URL to extract the public ID
                // Example: https://res.cloudinary.com/your-cloud-name/image/upload/v1234567890/fancy-collection/abcdef.jpg

                var uri = new Uri(url);
                var pathSegments = uri.AbsolutePath.Split('/');

                // Check if URL follows Cloudinary pattern
                if (pathSegments.Length < 5)
                    return null;

                // Extract the version part (v1234567890) and remove it
                var versionAndRest = string.Join('/', pathSegments.Skip(4));
                var parts = versionAndRest.Split('/', 2);

                if (parts.Length < 2 || !parts[0].StartsWith("v"))
                    return _folder + "/" + Path.GetFileNameWithoutExtension(uri.AbsolutePath);

                // Return folder/filename without extension as the public ID
                return parts[1].Split('.')[0];
            }
            catch
            {
                return null;
            }
        }
    }
}