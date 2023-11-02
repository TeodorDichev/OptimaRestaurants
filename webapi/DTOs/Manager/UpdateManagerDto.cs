using System.ComponentModel.DataAnnotations;
using webapi.Models;

namespace webapi.DTOs.Manager
{
    public class UpdateManagerDto
    {
        [RegularExpression("^\\w+@[a-zA-Z_]+?\\.[a-zA-Z]{2,3}$", ErrorMessage = "Invalid email address")]
        public string? NewEmail { get; set; }
        [StringLength(30, MinimumLength = 6, ErrorMessage = "Password must be at least {2}, and maximum {1} characters")]
        public string? NewPassword { get; set; }
        public string? NewFirstName { get; set; }
        public string? NewLastName { get; set; }
        public string? NewPhoneNumber { get; set; }
        public string? NewPictureUrl { get; set; }

        public required string OldEmail { get; set; }
        public required string OldFirstName { get; set; }
        public required string OldLastName { get; set; }
        public required string OldPhoneNumber { get; set; }
        public required string OldPictureUrl { get; set; }
    }
}
