using AiService.AccountManager.Repository.DTO;
using AiService.Domains.Entities;

namespace AiService.AccountManager.Repository.Interfaces
{
    public interface IAuthService
    {
        Task<bool> UpdateGmailVerificationStatus(string token);
        Task<AuthResult> RegisterAsync();
        Task<AuthResult> LoginAsync(LoginRequest request, CancellationToken ct = default);
        Task<AuthResult> LoginAsGuestAsync(CancellationToken ct = default);
        Task SendVerificationEmailAsync(RegisterRequest request);
        Task<ApplicationUser> UpdateGoogleUserStatus(string email, string? name);
    }
}
