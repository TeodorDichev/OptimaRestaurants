namespace webapi.DTOs.Restaurant
{
    public class RestaurantDetailsDto
    {
        public required string Id { get; set; }
        public required string Name { get; set; }
        public required string Address { get; set; }
        public required string City { get; set; }
        public int EmployeeCapacity { get; set; }
        public bool IsWorking { get; set; }
        public string? IconUrl { get; set; }
        public decimal CuisineAverageRating { get; set; }
        public decimal AtmosphereAverageRating { get; set; }
        public decimal EmployeesAverageRating { get; set; }
        public decimal RestaurantAverageRating { get; set; }
        public required string ManagerFullName { get; set; }
        public string? ManagerEmail { get; set; }
        public required string TopEmployeeFullName { get; set; }
        public string? TopEmployeeEmail { get; set; }
        public required decimal TopEmployeeRating { get; set; }
    }
}
