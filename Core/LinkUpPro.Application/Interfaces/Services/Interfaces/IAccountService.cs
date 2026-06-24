using System.Threading.Tasks;
using LinkUpPro.Application.DTOs.Account;

namespace LinkUpPro.Application.Interfaces.Services.Interfaces;

public interface IAccountService
{
    Task<AuthenticationResponse> AuthenticateAsync(AuthenticationRequest request);
    Task<RegisterResponse> RegisterUserAsync(RegisterRequest request);
    Task<string> ConfirmAccountAsync(string userId, string token);
    Task<string> ForgotPasswordAsync(ForgotPasswordRequest request);
    Task<string> ResetPasswordAsync(ResetPasswordRequest request);
    Task<string> ResendVerificationEmailAsync(ResendVerificationEmailRequest request);
    Task SignOutAsync();
}