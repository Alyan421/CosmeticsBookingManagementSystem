namespace CMS.Server.Services
{
    public interface IImageStorageService
    {
        Task<string> UploadImageAsync(IFormFile file);
        Task DeleteImageAsync(string imageUrl);
    }
}
