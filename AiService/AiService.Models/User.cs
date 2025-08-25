using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace AiService.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [EmailAddress]
        [DisplayName("Email")]
        public string Email { get; set; }

        [DisplayName("Password")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }

        [Display(Name = "Remember me?")]
        public string ConfirmPassword { get; set; }
    }
}
