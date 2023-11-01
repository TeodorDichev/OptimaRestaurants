namespace webapi.DTOs.Restaurant
{
    public class RestaurantDto
    {
        public required string Id { get; set; }
        public required string Name { get; set; }
        public required string Address { get; set; }
        public required string City { get; set; }
        public required string IconUrl { get; set; }
        public required decimal CuisineAverageRating { get; set; }
        public required decimal AtmosphereAverageRating { get; set; }
        public required decimal EmployeesAverageRating { get; set; }
    }
}
