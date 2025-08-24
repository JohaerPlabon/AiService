using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace AiService.Model
{
    public class UserInfo
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [EmailAddress]
        [DisplayName("Email")]
        public string Name { get; set; }

        [DisplayName("Password")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }
    }
}
