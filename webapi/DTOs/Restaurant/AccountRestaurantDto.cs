namespace webapi.DTOs.Restaurant
{
    public class AccountRestaurantDto
    {
        public required string Id { get; set; }
        public required string Name { get; set; }
        public required string Address { get; set; }
        public required string City { get; set; }
        public string? IconUrl { get; set; }
        public required decimal CuisineAverageRating { get; set; }
        public required decimal AtmosphereAverageRating { get; set; }
        public required decimal EmployeesAverageRating { get; set; }
        public required decimal RestaurantAverageRating { get; set; }
    }
}
