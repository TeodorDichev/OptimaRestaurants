namespace webapi.DTOs.Restaurant
{
    public class NewRestaurantDto
    {
        public required string Name { get; set; }
        public required string Address { get; set; }
        public required string City { get; set; }
        public required int EmployeeCapacity { get; set; }
        public string? IconUrl { get; set; }

    }
}
