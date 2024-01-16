namespace webapi.DTOs.Schedule
{
    public class EmployeeDailyScheduleDto
    {
        public required string ScheduleId { get; set; }
        public required string RestaurantName { get; set; }
        public required bool IsWorkDay { get; set; }
        public required bool IsFullDay { get; set; }
        public TimeOnly? From { get; set; }
        public TimeOnly? To { get; set;}
    }
}
