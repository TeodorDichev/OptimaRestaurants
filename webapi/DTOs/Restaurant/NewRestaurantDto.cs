using System.ComponentModel.DataAnnotations;

namespace webapi.DTOs.Restaurant
{
    public class NewRestaurantDto
    {
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Restaurant name must be at least {2}, and maximum {1} characters")]
        public required string Name { get; set; }

        [StringLength(50, MinimumLength = 2, ErrorMessage = "Restaurant address must be at least {2}, and maximum {1} characters")]
        public required string Address { get; set; }

        [StringLength(50, MinimumLength = 2, ErrorMessage = "Restaurant city must be at least {2}, and maximum {1} characters")]
        public required string City { get; set; }
        public required int EmployeeCapacity { get; set; }
        public IFormFile? IconFile { get; set; }
    }
}
