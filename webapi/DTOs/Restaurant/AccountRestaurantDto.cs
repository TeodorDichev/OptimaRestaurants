namespace webapi.DTOs.Restaurant
{
    public class AccountRestaurantDto
    {
        public required string Id { get; set; }
        public required string Name { get; set; }
        public required string Address { get; set; }
        public required string City { get; set; }
        public required int EmployeeCapacity { get; set; }
        public required bool IsWorking { get; set; }
        public string? IconPath { get; set; }
        public required int TotalReviewsCount { get; set; }
        public required decimal CuisineAverageRating { get; set; }
        public required decimal AtmosphereAverageRating { get; set; }
        public required decimal EmployeesAverageRating { get; set; }
        public required decimal RestaurantAverageRating { get; set; }
        public required string ManagerEmail { get; set; }
        public required string ManagerName { get; set; }
        public required string ManagerPhone { get; set; }
    }
}
