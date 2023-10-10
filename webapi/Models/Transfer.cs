using System.ComponentModel.DataAnnotations;

namespace webapi.Models
{
    public class Transfer
    {
        [Key]
        public Guid Id { get; set; }
        public required DateTime Date { get; set; }
        public decimal FixedSalary { get; set; }
        public decimal RatingBonuses { get; set; }
        public decimal OverWorkingBonuses { get; set; }
        public required virtual Employee Employee { get; set; }
    }
}
