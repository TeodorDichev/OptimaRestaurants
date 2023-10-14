using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace webapi.Models
{
    public class ApplicationUser : IdentityUser
    {
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public string? ProfilePictureUrl { get; set; }
        public DateTime DateCreated {  get; set; } = DateTime.UtcNow;
    }
}
