using System.ComponentModel.DataAnnotations;

namespace webapi.Models
{
    public class Request
    {
        [Key]
        public Guid Id { get; set; }
        public required virtual ApplicationUser Sender { get; set; }
        public required DateTime SentOn { get; set; }
        public DateTime? ConfirmedOn { get; set; }
        public DateTime? RejectedOn { get; set; }
    }
}
