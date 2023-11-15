namespace webapi.DTOs.Restaurant
{
    public class BrowseRestaurantDto
    {
        public required string Id { get; set; }
        public required string Name { get; set; }
        public required string Address { get; set; }
        public required string City { get; set; }
        public required bool IsWorking { get; set; }
        public required decimal RestaurantAverageRating { get; set; }
        public string? IconUrl { get; set; }
    }
}
