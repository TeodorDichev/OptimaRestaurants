namespace webapi.DTOs.Schedule
{
    public class ManagerDailyScheduleDto
    {
        public required string ScheduleId { get; set; }
        public required string EmployeeEmail { get; set; }
        public required string EmployeeName { get; set; }
        public required string RestaurantName { get; set; }
        public required bool IsFullDay { get; set; }
        public TimeOnly? From { get; set; }
        public TimeOnly? To { get; set; }

    }
}
