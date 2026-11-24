namespace Sofra.Application.Common.Validation;

/// <summary>
/// The business rules for uploaded pictures (shared by every picture upload feature): which file types and how large. Validators
/// enforce them; the file storage service itself stays rule-free.
/// </summary>
public static class ImageUploadRules
{
    public static readonly string[] AllowedExtensions = [".jpg", ".jpeg", ".png", ".webp"];

    public static readonly string[] AllowedContentTypes =
        ["image/jpeg", "image/png", "image/webp"];

    /// <summary>Maximum picture size: 5 MB.</summary>
    public const long MaxSizeBytes = 5 * 1024 * 1024;

    public static bool IsExtensionAllowed(string fileName)
    {
        var extension = Path.GetExtension(fileName).ToLowerInvariant();
        return AllowedExtensions.Contains(extension);
    }
}
