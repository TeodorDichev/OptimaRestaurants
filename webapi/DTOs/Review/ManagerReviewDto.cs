using System.ComponentModel.DataAnnotations;

namespace webapi.DTOs.Review
{
    public class ManagerReviewDto
    {
        public required string RestaurantId { get; set; }
        public required string EmployeeEmail { get; set; }

        [MaxLength(300)]
        public string? Comment { get; set; }
        public decimal? PunctualityRating { get; set; }
        public decimal? CollegialityRating { get; set; }
    }
}
