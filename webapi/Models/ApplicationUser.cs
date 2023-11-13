using Microsoft.AspNetCore.Identity;

namespace webapi.Models
{
    public class ApplicationUser : IdentityUser
    {
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public byte[]? ProfilePictureData { get; set; }
        public string? ProfilePictureMimeType { get; set; }
        public DateTime DateCreated { get; set; } = DateTime.UtcNow;
        public virtual ICollection<Request> Requests { get; set; } = new List<Request>();
    }
}
