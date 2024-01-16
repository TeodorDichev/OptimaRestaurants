namespace webapi.DTOs.Schedule
{
    public class ManagerFullScheduleDto
    {
        public required string ScheduleId { get; set; }
        public required bool IsAnyoneAssigned { get; set; }
        public required DateOnly Day { get; set; }

    }
}
