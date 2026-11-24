namespace Sofra.Application.Common.Storage;

/// <summary>A file heading for storage — carried from the API layer into the storage service.</summary>
/// <param name="FileName">The original file name, extension included.</param>
/// <param name="ContentType">The mime type reported by the uploader.</param>
/// <param name="Length">The file size in bytes.</param>
/// <param name="Content">The file stream, positioned at the start.</param>
public record FileDto(
    string FileName,
    string ContentType,
    long Length,
    Stream Content);
