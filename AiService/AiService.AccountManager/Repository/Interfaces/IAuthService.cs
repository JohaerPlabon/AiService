using AiService.AccountManager.Repository.DTO;

namespace AiService.AccountManager.Repository.Interfaces
{
    public interface IAuthService
    {
        Task<bool> VerifyGmailAccount(string token);
        Task<AuthResult> RegisterAsync();
        Task<AuthResult> LoginAsync(LoginRequest request, CancellationToken ct = default);
        Task<AuthResult> LoginAsGuestAsync(CancellationToken ct = default);
        Task SendVerificationEmailAsync(RegisterRequest request);
    }
}
