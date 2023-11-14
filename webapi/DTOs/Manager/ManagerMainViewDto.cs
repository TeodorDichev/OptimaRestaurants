using webapi.DTOs.Restaurant;

namespace webapi.DTOs.Manager
{
    public class ManagerMainViewDto
    {
        public required string Email { get; set; }
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public string? ProfilePictureUrl { get; set; }
        public virtual ICollection<RestaurantDto>? Restaurants { get; set; }
    }
}
