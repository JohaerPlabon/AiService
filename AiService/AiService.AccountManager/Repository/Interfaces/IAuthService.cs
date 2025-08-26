using AiService.AccountManager.Repository.DTO;

namespace AiService.AccountManager.Repository.Interfaces
{
    public interface IAuthService
    {
        Task<bool> VerifyGmailAccount(string token);
        Task<bool> IsEmailExists(RegisterRequest request, CancellationToken ct = default);
        Task<AuthResult> RegisterAsync(RegisterRequest request, CancellationToken ct = default);
        Task<AuthResult> LoginAsync(LoginRequest request, CancellationToken ct = default);
        Task<AuthResult> LoginAsGuestAsync(CancellationToken ct = default);
    }
}
