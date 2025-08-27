using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace AiService.Domains.Entities
{
    public class ApplicationUser
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        [DisplayName("Email")]
        public string Email { get; set; }
        public string UserName { get; private set; } = string.Empty;
        public string PasswordHash { get; private set; } = string.Empty;
        public string PasswordSalt { get; private set; } = string.Empty;

        public bool IsGuest { get; private set; }

        public static ApplicationUser CreateRegistered(string email, string userName, string passwordHash, string passwordSalt)
            => new ApplicationUser
            {
                Email = email,
                UserName = userName,
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt,
                IsGuest = false,
                VerificationToken = Guid.NewGuid().ToString(),
                TokenExpiryTime = DateTime.UtcNow.AddHours(24)
            };

        public static ApplicationUser CreateGuest(string userName)
            => new ApplicationUser
            {
                Email = $"guest-{Guid.NewGuid():N}@guest.local",
                UserName = userName,
                PasswordHash = string.Empty,
                PasswordSalt = string.Empty,
                IsGuest = true
            };

        public bool IsEmailVerified { get; set; } = false;
        public string? VerificationToken { get; set; }
        public DateTime? TokenGeneratedAt { get; set; }
        public DateTime? TokenExpiryTime { get; set; }
    }
}