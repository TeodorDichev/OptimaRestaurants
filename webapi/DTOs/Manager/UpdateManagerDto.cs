using System.ComponentModel.DataAnnotations;

namespace webapi.DTOs.Manager
{
    public class UpdateManagerDto
    {
        [StringLength(50, MinimumLength = 2, ErrorMessage = "First name must be at least {2}, and maximum {1} characters")]
        public string? NewFirstName { get; set; }
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Last name must be at least {2}, and maximum {1} characters")]
        public string? NewLastName { get; set; }
        [StringLength(50, MinimumLength = 10, ErrorMessage = "Phone number must be exactly {1} characters")]
        public string? NewPhoneNumber { get; set; }
        public IFormFile? ProfilePictureFile { get; set; }
    }
}
