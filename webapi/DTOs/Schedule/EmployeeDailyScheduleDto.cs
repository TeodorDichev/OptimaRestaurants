namespace webapi.DTOs.Schedule
{
    public class EmployeeDailyScheduleDto
    {
        public required string ScheduleId { get; set; }
        public required string RestaurantName { get; set; }
        public required string RestaurantId { get; set; }
        public required bool IsWorkDay { get; set; }
        public required bool IsFullDay { get; set; }
        public DateTime? From { get; set; }
        public DateTime? To { get; set; }
    }
}
