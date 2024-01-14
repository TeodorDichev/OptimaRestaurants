using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace webapi.Models
{
    public class ApplicationUser : IdentityUser
    {
        [MaxLength(50)]
        public required string FirstName { get; set; }
        [MaxLength(50)]
        public required string LastName { get; set; }
        public string? ProfilePicturePath { get; set; }
        public DateOnly DateCreated { get; set; } = DateOnly.FromDateTime(DateTime.Now);
    }
}
