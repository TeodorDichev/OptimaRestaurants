using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace webapi.DTOs.Restaurant
{
    public class UpdateRestaurantDto
    {
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Restaurant name must be at least {2}, and maximum {1} characters")]
        public string? Name { get; set; }
        [Precision(9, 6)]
        public decimal? Longitude { get; set; }
        [Precision(9, 6)]
        public decimal? Latitude { get; set; }
        public string? Address1 { get; set; }
        public string? Address2 { get; set; }
        public string? City { get; set; }
        public int? EmployeeCapacity { get; set; }
        public bool? IsWorking { get; set; }
        public IFormFile? IconFile { get; set; }
    }
}
