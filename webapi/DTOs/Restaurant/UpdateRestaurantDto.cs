namespace webapi.DTOs.Restaurant
{
    public class UpdateRestaurantDto
    {
        public string? Name { get; set; }
        public string? Address { get; set; }
        public string? City { get; set; }
        public int? EmployeeCapacity { get; set; }
        public bool? IsWorking { get; set; }
        public string? IconUrl { get; set; }
    }
}
