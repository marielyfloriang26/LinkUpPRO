namespace LinkUpPro.Application.Interfaces.Shared;

public interface IEmailService
{
    Task SendAsync(string to, string subject, string body);
}