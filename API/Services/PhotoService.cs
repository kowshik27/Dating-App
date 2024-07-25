using API.Helpers;
using API.Interfaces;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.Extensions.Options;

namespace API.Services;

public class PhotoService : IPhotoService
{


    private readonly Cloudinary _cloudinary;
    public PhotoService(IOptions<CloudinarySettings> config)
    {
        var confVal = config.Value;
        var acc = new Account(confVal.CloudName, confVal.ApiKey, confVal.ApiSecret);

        _cloudinary = new Cloudinary(acc);
    }

    public async Task<ImageUploadResult> AddPhotoAsync(IFormFile imgFile)
    {
        var uploadResult = new ImageUploadResult();

        if (imgFile.Length > 0)
        {

            using var stream = imgFile.OpenReadStream();
            var uploadParams = new ImageUploadParams
            {
                File = new FileDescription(imgFile.FileName, stream),
                Transformation = new Transformation()
                .Height(500).Width(500).Crop("fill").Gravity("face"),
                Folder = "DatingApp-net8"
            };

            uploadResult = await _cloudinary.UploadAsync(uploadParams);
        }

        return uploadResult;
    }

    public async Task<DeletionResult> DeletePhotoAsync(string publicId)
    {
        var deleteParams = new DeletionParams(publicId);
        return await _cloudinary.DestroyAsync(deleteParams);
    }
}
