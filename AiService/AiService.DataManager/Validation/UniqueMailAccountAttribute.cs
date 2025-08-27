using System.ComponentModel.DataAnnotations;

namespace AiService.DataManager.Validation
{
    public class UniqueMailAccountAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            try
            {
                if (value == null) return new ValidationResult("Email account is required.");

                var dbContext = (ApplicationDbContext)validationContext.GetService(typeof(ApplicationDbContext));
                var email = value.ToString();

                // Check if username exists
                bool exists = dbContext.Users.Any(u => u.Email == email);

                if (exists)
                {
                    return new ValidationResult("This email is already taken. Please choose another.");
                }

                return ValidationResult.Success;
            }
            catch (Exception ex)
            {
                var message = ex.Message;
            }

            return new ValidationResult("Unknown Error");
        }
    }
}
