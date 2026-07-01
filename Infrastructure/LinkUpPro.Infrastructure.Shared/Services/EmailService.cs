using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using Microsoft.Extensions.Configuration;
using LinkUpPro.Application.Interfaces.Shared;

public class EmailService : IEmailService
{
    private readonly IConfiguration _config;

    public EmailService(IConfiguration config)
    {
        _config = config;
    }

    public async Task SendAsync(string to, string subject, string body)
    {
        var email = new MimeMessage();
        email.From.Add(MailboxAddress.Parse(_config["EmailSettings:From"]));
        email.To.Add(MailboxAddress.Parse(to));
        email.Subject = subject;
        email.Body = new TextPart(MimeKit.Text.TextFormat.Html) { Text = body };

        using var smtp = new SmtpClient();
        var smtpHost = _config["EmailSettings:Host"] ?? "smtp.gmail.com";
        var smtpPort = int.TryParse(_config["EmailSettings:Port"], out var p) ? p : 587;
        var smtpUser = _config["EmailSettings:User"] ?? _config["EmailSettings:Username"];

        await smtp.ConnectAsync(smtpHost, smtpPort, SecureSocketOptions.StartTls);
        if (!string.IsNullOrEmpty(smtpUser))
        {
            await smtp.AuthenticateAsync(smtpUser, _config["EmailSettings:Password"]);
        }
        
        await smtp.SendAsync(email);
        await smtp.DisconnectAsync(true);
    }
}