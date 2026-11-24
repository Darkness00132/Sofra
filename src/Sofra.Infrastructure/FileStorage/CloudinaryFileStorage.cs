using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.Extensions.Options;
using Sofra.Application.Common.Interfaces;
using Sofra.Application.Common.Storage;

namespace Sofra.Infrastructure.FileStorage;

/// <summary>
/// Stores files in Cloudinary. Rule-free by design: which file types and sizes are allowed is
/// decided by feature validators, never here (separation of concerns).
/// </summary>
public class CloudinaryFileStorage(Cloudinary cloudinary, IOptions<CloudinarySettings> options) : IFileStorage
{
    public async Task<StoredFile> UploadAsync(FileDto file, CancellationToken cancellationToken = default)
    {
        var uploadParams = new ImageUploadParams
        {
            File = new FileDescription(file.FileName, file.Content),
            Folder = options.Value.Folder,
            UseFilename = false,
            UniqueFilename = true,
        };

        var result = await cloudinary.UploadAsync(uploadParams, cancellationToken);

        return new StoredFile(result.SecureUrl.ToString(), result.PublicId);
    }

    public async Task DeleteAsync(string storageId, CancellationToken cancellationToken = default) =>
        await cloudinary.DestroyAsync(new DeletionParams(storageId));
}
