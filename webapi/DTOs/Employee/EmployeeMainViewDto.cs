using webapi.DTOs.Restaurant;
using webapi.Models;

namespace webapi.DTOs.Employee
{
    public class EmployeeMainViewDto
    {
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public required string ProfilePictureUrl { get; set; }
        public required decimal SpeedAverageRating { get; set; }
        public required decimal AttitudeAverageRating { get; set; }
        public required decimal PunctualityAverageRating { get; set; }
        public required decimal CollegialityAverageRating { get; set; }
        public required decimal EmployeeAverageRating { get; set; }
        public virtual ICollection<RestaurantDto>? Restaurants { get; set; }
    }
}
