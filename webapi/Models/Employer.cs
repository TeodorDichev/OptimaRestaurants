using System.ComponentModel.DataAnnotations;

namespace webapi.Models
{
    public class Employer
    {
        [Key]
        public Guid Id { get; set; }
        public required virtual ApplicationUser User { get; set; }
        public ICollection<Restaurant> Restaurants { get; set;}
    }
}
