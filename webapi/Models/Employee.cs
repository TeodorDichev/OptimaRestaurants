using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace webapi.Models
{
    public class Employee
    {
        [Key]
        public Guid Id { get; set; }
        [MaxLength(50)]
        public required string City { get; set; }
        public required string QrCodePath { get; set; }
        public bool IsLookingForJob { get; set; } = true;
        public string? ResumePath { get; set; }
        public required int TotalReviewsCount { get; set; }
        public required DateOnly BirthDate { get; set; }
        [Precision(4, 2)]
        public decimal? SpeedAverageRating { get; set; }
        [Precision(4, 2)]
        public decimal? AttitudeAverageRating { get; set; }
        [Precision(4, 2)]
        public decimal? PunctualityAverageRating { get; set; }
        [Precision(4, 2)]
        public decimal? CollegialityAverageRating { get; set; }
        [Precision(4, 2)]
        public decimal? EmployeeAverageRating { get; set; }
        public required virtual ApplicationUser Profile { get; set; }
        public virtual ICollection<EmployeeRestaurant> EmployeesRestaurants { get; set; } = new List<EmployeeRestaurant>();
        public virtual ICollection<Schedule> Schedules { get; set; } = new List<Schedule>();
    }
}
