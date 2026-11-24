namespace Sofra.Application.Common.Storage;

/// <summary>A file that was successfully stored.</summary>
/// <param name="Url">The public URL the file can be reached at.</param>
/// <param name="StorageId">The storage-side id needed to delete the file later.</param>
public record StoredFile(
    string Url,
    string StorageId);
