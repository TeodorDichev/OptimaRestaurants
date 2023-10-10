using System.ComponentModel.DataAnnotations;

namespace webapi.Models
{
    public class Shift
    {
        [Key]
        public Guid Id { get; set; }
        public virtual required ShiftType ShiftType { get; set; }
        public virtual required Restaurant Restaurant { get; set; } //so as to know in which restaurant will work on the correspodent date
        public required DateTime ScheduledOn { get; set; }
        public required DateTime ScheduledFor { get; set; }
        public DateTime UpdatedOn { get; set; }

    }
}
