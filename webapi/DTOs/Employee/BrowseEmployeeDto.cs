using webapi.DTOs.Restaurant;

namespace webapi.DTOs.Employee
{
    public class BrowseEmployeeDto
    {
        public required string Email { get; set; }
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public string? ProfilePicturePath { get; set; }
        public required string PhoneNumber { get; set; }
        public required string City { get; set; }
        public required bool IsLookingForJob { get; set; }
        public required decimal EmployeeAverageRating { get; set; }
        public required int TotalReviewsCount { get; set; }
        public required int RestaurantsCount { get; set; }
    }
}
