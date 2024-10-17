using CloudinaryDotNet.Actions;
using HealthAppAPI.Enums;

public interface ICloudinaryUploader 
{
    Task<ImageUploadResult> UploadImage(IFormFile imageFile, string? folder);
    Task<DelResResult> DeleteImage(List<string> publicIds);
    Task<RawUploadResult> UploadFile(IFormFile file, string? folder);
    Task<DeletionResult> DeleteFile(string publicId, DocumentFormat documentFormat);
}