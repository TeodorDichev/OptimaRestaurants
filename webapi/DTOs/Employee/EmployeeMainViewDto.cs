﻿using webapi.DTOs.Restaurant;

namespace webapi.DTOs.Employee
{
    public class EmployeeMainViewDto
    {
        public required string Email { get; set; }
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public required string PhoneNumber { get; set; }
        public string? QrCodePath { get; set; }
        public required string BirthDate { get; set; }
        public required string City { get; set; }
        public string? ProfilePicturePath { get; set; }
        public required bool IsLookingForJob { get; set; }
        public required int TotalReviewsCount { get; set; }
        public required decimal SpeedAverageRating { get; set; }
        public required decimal AttitudeAverageRating { get; set; }
        public required decimal PunctualityAverageRating { get; set; }
        public required decimal CollegialityAverageRating { get; set; }
        public required decimal EmployeeAverageRating { get; set; }
        public virtual ICollection<AccountRestaurantDto>? Restaurants { get; set; }
    }
}
