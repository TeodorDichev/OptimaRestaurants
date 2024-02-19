using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace webapi.DTOs.Restaurant
{
    public class NewRestaurantDto
    {
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Restaurant name must be at least {2}, and maximum {1} characters")]
        public required string Name { get; set; }

        [Precision(9, 6)]
        public required decimal Longitude { get; set; }
        [Precision(9, 6)]
        public required decimal Latitude { get; set; }
        public required string Address1 { get; set; }
        public required string Address2 { get; set; }
        public required string City { get; set; }
        public required int EmployeeCapacity { get; set; }
        public IFormFile? IconFile { get; set; }
    }
}
