using Microsoft.EntityFrameworkCore;

namespace webapi.Models
{
    public class CustomerReview : Review
    {
        [Precision(4, 2)]
        public decimal? SpeedRating { get; set; }
        [Precision(4, 2)]
        public decimal? AttitudeRating { get; set; }
        [Precision(4, 2)]
        public decimal? CuisineRating { get; set; }
        [Precision(4, 2)]
        public decimal? AtmosphereRating { get; set; }
    }
}
