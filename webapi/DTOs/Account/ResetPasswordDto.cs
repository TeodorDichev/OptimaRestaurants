using System.ComponentModel.DataAnnotations;

namespace webapi.DTOs.Account
{
    public class ResetPasswordDto
    {
        public required string Token { get; set; }
        [RegularExpression("^\\w+@[a-zA-Z_]+?\\.[a-zA-Z]{2,3}$", ErrorMessage = "Invalid email address")]
        public required string Email { get; set; }
        [StringLength(30, MinimumLength = 6, ErrorMessage = "Password must be at least {2}, and maximum {1} characters")]
        public required string Password { get; set; }
    }
}
