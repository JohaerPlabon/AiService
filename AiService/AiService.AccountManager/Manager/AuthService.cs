using AiService.AccountManager.Repository.DTO;
using AiService.AccountManager.Repository.Interfaces;
using AiService.DataManager;
using AiService.Domains.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AiService.AccountManager.Manager
{
    public class AuthService : IAuthService
    {
        private readonly ApplicationDbContext _db;
        private readonly IUserRepository _users;
        private readonly IPasswordHasher _hasher;
        private readonly IEmailService _emailService;
        private RegisterRequest _registerRequest;
        private CancellationToken _ct;

        public AuthService(IUserRepository users, IPasswordHasher hasher, ApplicationDbContext db, IEmailService emailService)
        {
            _db = db;
            _users = users;
            _hasher = hasher;
            _emailService = emailService;
        }

        public async Task<AuthResult> RegisterAsync(RegisterRequest request, CancellationToken ct = default)
        {
            var (hash, salt) = _hasher.Hash(request.Password);
            var user = ApplicationUser.CreateRegistered(request.Email, request.UserName, hash, salt);
            await _users.AddAsync(user, ct);
            await _users.SaveChangesAsync(ct);
            return AuthResult.Success(user.Id, user.UserName, isGuest: false);
        }

        public async Task<AuthResult> LoginAsync(LoginRequest request, CancellationToken ct = default)
        {
            var user = await _users.GetByEmailAsync(request.Email, ct);
            if (user is null || user.IsGuest)
                return AuthResult.Fail("Invalid credentials.");

            var valid = _hasher.Verify(request.Password, user.PasswordHash, user.PasswordSalt);
            if (!valid) return AuthResult.Fail("Invalid credentials.");

            return AuthResult.Success(user.Id, user.UserName, isGuest: false);
        }

        public async Task<AuthResult> LoginAsGuestAsync(CancellationToken ct = default)
        {
            var guestName = $"Guest-{Guid.NewGuid().ToString()[..6]}";
            var guest = ApplicationUser.CreateGuest(guestName);
            await _users.AddAsync(guest, ct);
            await _users.SaveChangesAsync(ct);
            return AuthResult.Success(guest.Id, guest.UserName, isGuest: true);
        }

        public bool VerifyGmailAccount(string token)
        {
            var user = _db.Users.FirstOrDefaultAsync(u => u.VerificationToken == token).Result;
            if (user == null) return false;

            user.IsEmailVerified = true;
            user.VerificationToken = null;
            _db.SaveChangesAsync();

            return true;
        }

        Task<bool> IAuthService.VerifyGmailAccount(string token)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> IsEmailExists(RegisterRequest request, CancellationToken ct = default)
        {
            _registerRequest = request;
            _ct = ct;

            if (await _users.EmailExistsAsync(request.Email, ct))
            {
                AuthResult.Fail("Email already registered.");
                return true;
            }

            false;
        }
    }
}
