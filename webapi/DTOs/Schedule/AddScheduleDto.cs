namespace webapi.DTOs.Schedule
{
    public class AddScheduleDto
    {
        public required string EmployeeEmail { get; set; }
        public required string RestaurantId { get; set; }
        public required DateOnly Day { get; set; }
        public TimeOnly? From { get; set; }
        public TimeOnly? To { get; set; }
        public bool IsWorkDay { get; set; }
        public bool FullDay { get; set; }
        public string? Reason { get; set; }

    }
}
