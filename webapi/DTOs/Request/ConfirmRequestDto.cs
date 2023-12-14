namespace webapi.DTOs.Request
{
    public class ResponceToRequestDto
    {
        public required string RequestId { get; set; }
        public required string RestaurantId { get; set; }
        public required bool Confirmed { get; set; }
    }
}
