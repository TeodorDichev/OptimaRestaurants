using System.ComponentModel.DataAnnotations;

namespace webapi.Models
{
    public class Schedule
    {
        [Key]
        public Guid Id { get; set; }
        public virtual required Employee Employee { get; set; }
        public virtual required Restaurant Restaurant { get; set; }
        public required DateOnly Day { get; set; }
        public TimeOnly? From { get; set; }
        public TimeOnly? To { get; set; }
        public bool IsWorkDay { get; set; }
        public bool FullDay { get; set; }
        [MaxLength(300)]
        public string? Reason { get; set; }
    }
}
