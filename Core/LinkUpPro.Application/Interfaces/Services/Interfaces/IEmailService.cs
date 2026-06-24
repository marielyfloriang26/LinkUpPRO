using System.Threading.Tasks;

namespace LinkUpPro.Application.Interfaces.Services.Interfaces;

public interface IEmailService
{
    Task SendAsync(string to, string subject, string body);
}