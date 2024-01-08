using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace webapi.Models
{
    public class Employee
    {
        [Key]
        public Guid Id { get; set; }
        public required string City { get; set; }
        public required string QrCodePath { get; set; }
        public decimal? EmployeeAverageRating { get; set; }
        public bool IsLookingForJob { get; set; } = true;
        public string? ResumePath { get; set; }
        public required int TotalReviewsCount { get; set; }
        public required DateTime BirthDate { get; set; }
        [Precision(2, 2)]
        public decimal? SpeedAverageRating { get; set; }
        [Precision(2, 2)]
        public decimal? AttitudeAverageRating { get; set; }
        [Precision(2, 2)]
        public decimal? PunctualityAverageRating { get; set; }
        [Precision(2, 2)]
        public decimal? CollegialityAverageRating { get; set; }
        [Precision(2, 2)]
        public required virtual ApplicationUser Profile { get; set; }
        public virtual ICollection<EmployeeRestaurant> EmployeesRestaurants { get; set; } = new List<EmployeeRestaurant>();
        public virtual ICollection<Shift> Shifts { get; set; } = new List<Shift>();
    }
}
