using System.ComponentModel.DataAnnotations;

namespace webapi.DTOs.Review
{
    public class OldReviewDto
    {
        public required string RestaurantName { get; set; }
        public required string RestaurantCity { get; set; }
        public required string ReviewType { get; set; }
        public required string ReviewDate { get; set; }
        [MaxLength(255)]
        public string? Comment { get; set; }
        public decimal? PunctualityRating { get; set; }
        public decimal? CollegialityRating { get; set; }
        public decimal? SpeedRating { get; set; }
        public decimal? AttitudeRating { get; set; }
        public decimal? CuisineRating { get; set; }
        public decimal? AtmosphereRating { get; set; }
    }
}
