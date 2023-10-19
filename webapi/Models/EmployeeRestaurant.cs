using System.ComponentModel.DataAnnotations;

namespace webapi.Models
{
    public class EmployeeRestaurant
    {
        [Key]
        public Guid Id { get; set; }
        public virtual required Employee Employee { get; set; }
        public virtual required Restaurant Restaurant { get; set; }
        public required DateTime SentOn { get; set; }
        public DateTime? ConfirmedOn { get; set; }
        public DateTime? EndedOn { get; set; }

    }
}
