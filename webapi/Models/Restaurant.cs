﻿using System.ComponentModel.DataAnnotations;

namespace webapi.Models
{
    public class Restaurant
    {
        [Key]
        public Guid Id { get; set; }
        public required string Name { get; set; }
        public required string Address { get; set; }
        public int EmployeeCapacity { get; set; }
        public bool IsWorking { get; set; }
        public string IconUrl { get; set; }
        public decimal CuisineAverageRating { get; set; }
        public decimal AtmosphereAverageRating { get; set; }
        public decimal EmployeesAverageRating { get; set; }
        public decimal RestaurantAverageRating { get; set; }
        public decimal StandartMontlyPayment { get; set; }
        public int MinRatingForBonuses { get; set; }
        public decimal RatingBonusesAmount { get; set; }
        public int OverWorkingHours { get; set; }
        public decimal OverWorkingAmountPerHour { get; set; }
        public bool UsePercentageGrowth { get; set; }

        public required virtual Employer Employer { get; set; }
        public virtual ICollection<EmployeeRestaurant> EmployeesRestaurants { get; set; }
    }
}
