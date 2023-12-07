namespace webapi.DTOs.Restaurant
{
    public class RestaurantDetailsDto
    {
        public required string Id { get; set; }
        public required string Name { get; set; }
        public required string Address { get; set; }
        public required string City { get; set; }
        public required int EmployeeCapacity { get; set; }
        public required bool IsWorking { get; set; }
        public required decimal CuisineAverageRating { get; set; }
        public required decimal AtmosphereAverageRating { get; set; }
        public required decimal EmployeesAverageRating { get; set; }
        public required decimal RestaurantAverageRating { get; set; }
        public required int RatingsCount { get; set; }
        public string? IconUrl { get; set; }
        public string? ManagerFullName { get; set; }
        public string? ManagerEmail { get; set; }
        public string? TopEmployeeFullName { get; set; }
        public string? TopEmployeeEmail { get; set; }
        public decimal? TopEmployeeRating { get; set; }
    }
}
