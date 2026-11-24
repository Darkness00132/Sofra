using Sofra.Application.Common.Storage;

namespace Sofra.Api.Helper;

/// <summary>Converts API-layer form files into the Application layer's storage contracts.</summary>
public static class FormFileExtensions
{
    /// <summary>Converts an uploaded form file into a <see cref="FileDto"/> for the Application layer.</summary>
    public static FileDto ToFileDto(this IFormFile file) =>
        new(file.FileName, file.ContentType, file.Length, file.OpenReadStream());
}
