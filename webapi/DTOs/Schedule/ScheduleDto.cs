namespace webapi.DTOs.Schedule
{
    public class ScheduleDto
    {
        public required string ScheduleId { get; set; }
        public required string EmployeeEmail { get; set; }
        public required string RestaurantId { get; set; }
        public required DateTime Day { get; set; }
        public DateTime? From { get; set; }
        public DateTime? To { get; set; }
        public bool IsWorkDay { get; set; }
        public bool FullDay { get; set; }
    }
}
