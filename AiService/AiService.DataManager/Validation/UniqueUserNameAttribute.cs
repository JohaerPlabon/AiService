using System.ComponentModel.DataAnnotations;

namespace AiService.DataManager.Validation
{
    public class UniqueUserNameAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            try
            {
                if (value == null) return new ValidationResult("Username is required.");

                var dbContext = (ApplicationDbContext)validationContext.GetService(typeof(ApplicationDbContext));
                var username = value.ToString();

                // Check if username exists
                bool exists = dbContext.Users.Any(u => u.UserName == username);

                if (exists)
                {
                    return new ValidationResult("This username is already taken. Please choose another.");
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
