using System.ComponentModel.DataAnnotations;

namespace webapi.DTOs.Employee
{
    public class UpdateEmployeeDto
    {
        [StringLength(50, MinimumLength = 2, ErrorMessage = "First name must be at least {2}, and maximum {1} characters")]
        public string? NewFirstName { get; set; }
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Last name must be at least {2}, and maximum {1} characters")]
        public string? NewLastName { get; set; }
        [StringLength(10, MinimumLength = 10, ErrorMessage = "Phone number must be exactly {1} characters")]
        public string? NewPhoneNumber { get; set; }
        public string? OldPassword { get; set; }
        public string? NewPassword { get; set; }
        public IFormFile? ProfilePictureFile { get; set; }
        public DateOnly? NewBirthDate { get; set; }
        [StringLength(50, MinimumLength = 2, ErrorMessage = "City must be at least {2}, and maximum {1} characters")]
        public string? NewCity { get; set; }
        public bool IsLookingForJob { get; set; }
    }
}
