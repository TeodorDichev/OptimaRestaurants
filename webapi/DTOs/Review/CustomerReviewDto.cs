using System.ComponentModel.DataAnnotations;

namespace webapi.DTOs.Review
{
    public class CustomerReviewDto
    {
        public required string RestaurantId { get; set; }
        public required string EmployeeEmail { get; set; }

        [MaxLength(300)]
        public string? Comment { get; set; }
        public decimal? SpeedRating { get; set; }
        public decimal? AttitudeRating { get; set; }
        public decimal? CuisineRating { get; set; }
        public decimal? AtmosphereRating { get; set; }
    }
}
