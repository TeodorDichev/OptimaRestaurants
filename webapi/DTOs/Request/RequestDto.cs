namespace webapi.DTOs.Request
{
    public class RequestDto
    {
        public required string Id { get; set; }
        public required string Text { get; set; }
        public required string SenderEmail { get; set; }
        public required string RestaurantId { get; set; }
        public required string SentOn { get; set; }
        public bool? Confirmed { get; set; }
    }
}
