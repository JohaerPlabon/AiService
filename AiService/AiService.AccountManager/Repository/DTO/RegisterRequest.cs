using AiService.DataManager.Validation;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace AiService.AccountManager.Repository.DTO
{
    public class RegisterRequest
    {
        [Required, EmailAddress]
        [UniqueMailAccount]
        public string Email { get; set; } = string.Empty;

        [Required, MinLength(3)]
        [UniqueUserName]
        public string UserName { get; set; } = string.Empty;

        [Required]
        [MinLength(8, ErrorMessage = "Password must be at least 8 characters long")]
        [DisplayName("Password")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Confirm Password is required")]
        [Compare("Password", ErrorMessage = "Passwords do not match")]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}
