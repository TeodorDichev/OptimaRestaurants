namespace webapi.DTOs.Schedule
{
    public class ManagerFullScheduleDto
    {
        public required string ScheduleId { get; set; }
        public required int PeopleAssignedToWork { get; set; }
        public required DateTime Day { get; set; }

    }
}
