using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using Sofra.Application.Common.Interfaces;

namespace Sofra.Infrastructure.Emailing;

/// <summary>Sends emails through Gmail's SMTP server (smtp.gmail.com) using MailKit.</summary>
public class GmailEmailSender(IOptions<SmtpOptions> options) : IEmailSender
{
    public async Task SendAsync(string to, string subject, string htmlBody, CancellationToken cancellationToken = default)
    {
        var smtp = options.Value;

        var message = new MimeMessage();
        message.From.Add(new MailboxAddress(smtp.FromName, smtp.From));
        message.To.Add(MailboxAddress.Parse(to));
        message.Subject = subject;
        message.Body = new BodyBuilder { HtmlBody = htmlBody }.ToMessageBody();

        using var client = new SmtpClient();
        await client.ConnectAsync(smtp.Host, smtp.Port, SecureSocketOptions.StartTls, cancellationToken);
        await client.AuthenticateAsync(smtp.Username, smtp.Password, cancellationToken);
        await client.SendAsync(message, cancellationToken);
        await client.DisconnectAsync(true, cancellationToken);
    }
}
