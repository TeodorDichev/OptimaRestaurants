using Microsoft.EntityFrameworkCore;

namespace webapi.Models
{
    public class ManagerReview : Review
    {
        [Precision(2, 2)]
        public decimal? PunctualityRating { get; set; }
        [Precision(2, 2)]
        public decimal? CollegialityRating { get; set; }
    }
}
