using webapi.DTOs.Restaurant;

namespace webapi.DTOs.Manager
{
    public class ManagerMainViewDto
    {
        public required string Email { get; set; }
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public required string PhoneNumber { get; set; }
        public IFormFile? ProfilePicture { get; set; }
        public virtual ICollection<AccountRestaurantDto>? Restaurants { get; set; }
    }
}
