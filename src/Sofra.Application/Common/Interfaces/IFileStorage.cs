using Sofra.Application.Common.Storage;
namespace Sofra.Application.Common.Interfaces;

/// <summary>
/// Stores and deletes files in remote file storage (Cloudinary). It knows nothing about
/// business rules — which file types or sizes are allowed is decided by feature validators
/// (see ImageUploadRules), never by this service.
/// </summary>
public interface IFileStorage
{
    /// <summary>Uploads a file and returns its public URL and storage id.</summary>
    Task<StoredFile> UploadAsync(FileDto file, CancellationToken cancellationToken = default);

    /// <summary>Deletes a previously stored file by its storage id.</summary>
    Task DeleteAsync(string storageId, CancellationToken cancellationToken = default);
}
