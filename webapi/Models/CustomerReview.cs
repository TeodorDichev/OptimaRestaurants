using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace webapi.Models
{
    public class CustomerReview
    {
        [Key]
        public Guid Id { get; set; }
        public virtual required Employee Employee { get; set; }
        public virtual required Restaurant Restaurant { get; set; }
        public required DateTime DateTime { get; set; }
        public string? Comment { get; set; }
        [Precision(2,2)]
        public decimal? SpeedRating { get; set; }
        [Precision(2, 2)]
        public decimal? AttitudeRating { get; set; }
        [Precision(2, 2)]
        public decimal? CuisineRating { get; set; }
        [Precision(2, 2)]
        public decimal? AtmosphereRating { get; set; }
    }
}
