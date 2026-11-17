namespace Sofra.Infrastructure.Emailing;

/// <summary>SMTP settings bound from the "Smtp" configuration section (Gmail SMTP).</summary>
public class SmtpOptions
{
    public const string SectionName = "Smtp";

    public string Host { get; set; } = "smtp.gmail.com";

    public int Port { get; set; } = 587;

    /// <summary>The Gmail address used to sign in (a Google App Password is required).</summary>
    public string Username { get; set; } = string.Empty;

    public string Password { get; set; } = string.Empty;

    public string From { get; set; } = string.Empty;

    public string FromName { get; set; } = "Sofra";
}
