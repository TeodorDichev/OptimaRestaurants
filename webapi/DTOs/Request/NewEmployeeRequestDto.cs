namespace webapi.DTOs.Request
{
    public class NewEmployeeRequestDto
    {
        public required string RestaurantId { get; set; }
        public required string EmployeeEmail { get; set; }
    }
}
