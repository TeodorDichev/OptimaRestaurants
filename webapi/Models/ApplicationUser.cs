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
        [MaxLength(10), MinLength(10)]
        public override string? PhoneNumber { get => base.PhoneNumber; set => base.PhoneNumber = value; }
        public string? ProfilePicturePath { get; set; }
        public DateOnly DateCreated { get; set; } = DateOnly.FromDateTime(DateTime.Now);
    }
}
