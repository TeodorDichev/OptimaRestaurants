using webapi.DTOs.Restaurant;

namespace webapi.DTOs.Review
{
    public class ReviewDto
    {
        public required string EmployeeEmail { get; set; }
        public required string JwtToken { get; set; }
        public List<BrowseRestaurantDto> RestaurantDtos { get; set; } = new List<BrowseRestaurantDto>();
    }
}
