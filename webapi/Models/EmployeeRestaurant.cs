using System.ComponentModel.DataAnnotations;

namespace webapi.Models
{
    public class EmployeeRestaurant
    {
        [Key]
        public Guid Id { get; set; }
        public virtual required Employee Employee { get; set; }
        public virtual required Restaurant Restaurant { get; set; }
        public required DateTime StartedOn { get; set; } = DateTime.Now;
        public DateTime? EndedOn { get; set; }
    }
}
