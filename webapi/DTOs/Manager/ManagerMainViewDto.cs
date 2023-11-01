using webapi.DTOs.Restaurant;

namespace webapi.DTOs.Manager
{
    public class ManagerMainViewDto
    {
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public required string ProfilePictureUrl { get; set; }
        public virtual ICollection<RestaurantDto>? Restaurants { get; set; }
    }
}
