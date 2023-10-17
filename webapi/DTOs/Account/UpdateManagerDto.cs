using System.ComponentModel.DataAnnotations;

namespace webapi.DTOs.Account
{
    public class UpdateManagerDto
    {
        public required string Token { get; set; }
        public string NewFirstName { get; set; }
        public string NewLastName { get; set; }
        public string NewPassword { get; set; }

        [RegularExpression("^\\w+@[a-zA-Z_]+?\\.[a-zA-Z]{2,3}$", ErrorMessage = "Invalid email address")]
        public string NewEmail { get; set; }
        public string NewPictureUrl { get; set; }
    }
}
