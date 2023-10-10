using System.ComponentModel.DataAnnotations;

namespace webapi.Models
{
    public class ShiftType
    {
        [Key]
        public Guid Id { get; set; }
        public required string Name { get; set;}
    }
}
