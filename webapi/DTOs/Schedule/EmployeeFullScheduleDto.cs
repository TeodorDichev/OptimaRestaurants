namespace webapi.DTOs.Schedule
{
    public class EmployeeFullScheduleDto
    {
        public required string ScheduleId { get; set; }
        public required bool IsWorkDay { get; set; }
        public required DateOnly Day { get; set; }
    }
}
