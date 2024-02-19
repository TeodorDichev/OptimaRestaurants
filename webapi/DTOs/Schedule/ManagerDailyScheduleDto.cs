namespace webapi.DTOs.Schedule
{
    public class ManagerDailyScheduleDto
    {
        public required string ScheduleId { get; set; }
        public required string EmployeeEmail { get; set; }
        public required string EmployeeName { get; set; }
        public required string RestaurantName { get; set; }
        public required bool IsFullDay { get; set; }
        public required bool IsWorkDay { get; set; }
        public DateTime? From { get; set; }
        public DateTime? To { get; set; }

    }
}
