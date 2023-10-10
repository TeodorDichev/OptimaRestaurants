using Microsoft.AspNetCore.Identity;

namespace webapi.Models
{
    public class ApplicationUser : IdentityUser
    {
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public required string PhoneNumber { get; set; }
        public string? ProfileImageUrl { get; set; }
    }
}
