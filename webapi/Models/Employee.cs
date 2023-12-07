using System.ComponentModel.DataAnnotations;

namespace webapi.Models
{
    public class Employee
    {
        [Key]
        public Guid Id { get; set; }
        public required virtual ApplicationUser Profile { get; set; }
        public required string City { get; set; }
        public required string QrCodePath { get; set; }
        public string? ResumePath { get; set; }
        public required DateTime BirthDate { get; set; }
        public decimal? SpeedAverageRating { get; set; }
        public decimal? AttitudeAverageRating { get; set; }
        public decimal? PunctualityAverageRating { get; set; }
        public decimal? CollegialityAverageRating { get; set; }
        public decimal? EmployeeAverageRating { get; set; }
        public bool IsLookingForJob { get; set; } = true;
        public virtual ICollection<EmployeeRestaurant> EmployeesRestaurants { get; set; } = new List<EmployeeRestaurant>();
        public virtual ICollection<Shift> Shifts { get; set; } = new List<Shift>();
    }
}
