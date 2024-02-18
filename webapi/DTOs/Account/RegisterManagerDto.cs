using System.ComponentModel.DataAnnotations;

namespace webapi.DTOs.Account
{
    public class RegisterManagerDto
    {
        [StringLength(50, MinimumLength = 2, ErrorMessage = "First name must be at least {2}, and maximum {1} characters")]
        public required string FirstName { get; set; }
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Last name must be at least {2}, and maximum {1} characters")]
        public required string LastName { get; set; }
        [RegularExpression("^\\w+@[a-zA-Z_]+?\\.[a-zA-Z]{2,3}$", ErrorMessage = "Invalid email address")]
        public required string Email { get; set; }
        [StringLength(50, MinimumLength = 6, ErrorMessage = "Password must be at least {2}, and maximum {1} characters")]
        public required string Password { get; set; }
    }
}
