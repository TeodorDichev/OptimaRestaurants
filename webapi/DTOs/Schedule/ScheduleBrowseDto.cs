namespace webapi.DTOs.Schedule
{
    public class ScheduleBrowseDto
    {
        public required string ScheduleId { get; set; }
        public required string EmployeeEmail { get; set; }
        public required string RestaurantId { get; set; }
        public required DateOnly Day { get; set; }
        public bool IsWorkDay { get; set; }
    }
}
