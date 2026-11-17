namespace Sofra.Application.Common.Interfaces;

/// <summary>
/// Sends transactional emails (e.g. password reset codes). Implemented in Infrastructure
/// over Gmail SMTP with MailKit.
/// </summary>
public interface IEmailSender
{
    Task SendAsync(string to, string subject, string htmlBody, CancellationToken cancellationToken = default);
}
