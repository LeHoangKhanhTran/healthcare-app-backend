using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using HealthAppAPI.Enums;

public class CloudinaryUploader : ICloudinaryUploader
{
    private Cloudinary cloudinary;
    public CloudinaryUploader(Cloudinary cloudinary)
    {
        this.cloudinary = cloudinary;
    }

    public async Task<ImageUploadResult> UploadImage(IFormFile imageFile, string? folder)
    {
        ImageUploadResult uploadResult;
        using (var stream = imageFile.OpenReadStream()) 
        {
            var uploadParams = new ImageUploadParams()
            {
                File = new FileDescription(imageFile.FileName, stream),
                UseFilename = true,
                UniqueFilename = true,
                Overwrite = true,
                Folder = folder
            };
            uploadResult = await cloudinary.UploadAsync(uploadParams);
        }
        return uploadResult;
    }
    public async Task<DelResResult> DeleteImage(List<string> publicIds)
    {
        var delParams = new DelResParams()
        {
            PublicIds = publicIds,
            ResourceType = ResourceType.Image
        };
        return await cloudinary.DeleteResourcesAsync(delParams);
    }

    public async Task<RawUploadResult> UploadFile(IFormFile file, string? folder)
    {
        var uploadParams = new RawUploadParams()
        {
            File = new FileDescription(file.FileName, file.OpenReadStream()),
            Folder = folder
        };
        var uploadResult = await cloudinary.UploadAsync(uploadParams);
        return uploadResult;
    }

    public async Task<DeletionResult> DeleteFile(string publicId, DocumentFormat documentFormat)
    {
        var deleteParams = new DeletionParams(publicId)
        {
            ResourceType = documentFormat.ToString() == "jpg" || documentFormat.ToString() == "png" ? ResourceType.Image : ResourceType.Raw
        };
        var deletionResult = await cloudinary.DestroyAsync(deleteParams);
        
        if (deletionResult.Result == "ok")
        {
            Console.WriteLine("Succeeded");
        }
        else
        {
            Console.WriteLine($"Deletion failed: {deletionResult.Error.Message}");
        }
        return deletionResult;
    }
}