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
        private ApplicationUser _user;

        public AuthService(IUserRepository users, IPasswordHasher hasher, ApplicationDbContext db, IEmailService emailService)
        {
            _db = db;
            _users = users;
            _hasher = hasher;
            _emailService = emailService;
        }

        public async Task<AuthResult> RegisterAsync()
        {
            if (await _users.EmailExistsAsync(_user.Email))
            {
                return AuthResult.Fail("Email already registered.");
            }

            await _users.AddAsync(_user);
            await _users.SaveChangesAsync();
            return AuthResult.Success(_user.Id, _user.UserName, isGuest: false);
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

        async Task<bool> IAuthService.VerifyGmailAccount(string token)
        {
            var user = await _db.Users.FirstOrDefaultAsync(u => u.VerificationToken == token);

            if (user == null || user.TokenExpiryTime < DateTime.UtcNow)
                return false;

            user.IsEmailVerified = true;
            user.VerificationToken = null;
            user.TokenExpiryTime = null;

            await _db.SaveChangesAsync();

            return true;
        }

        public async Task SendVerificationEmailAsync(RegisterRequest request)
        {
            var (hash, salt) = _hasher.Hash(request.Password);
            _user = ApplicationUser.CreateRegistered(request.Email, request.UserName, hash, salt);

            var link = $"https://localhost:7012/Auth/VerifyEmail?token={_user.VerificationToken}";
            await _emailService.SendEmailAsync(_user.Email, "Verify your account",
                $"<p>Click <a href='{link}'>here</a> to verify your email.</p>");
        }
    }
}
