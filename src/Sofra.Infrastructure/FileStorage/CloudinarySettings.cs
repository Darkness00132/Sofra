namespace Sofra.Infrastructure.FileStorage;

/// <summary>Cloudinary credentials bound from the "Cloudinary" configuration section.</summary>
public class CloudinarySettings
{
    public const string SectionName = "Cloudinary";

    public string CloudName { get; set; } = string.Empty;

    public string ApiKey { get; set; } = string.Empty;

    public string ApiSecret { get; set; } = string.Empty;

    /// <summary>Cloudinary sub-folder all Sofra files are stored under.</summary>
    public string Folder { get; set; } = "sofra";
}
