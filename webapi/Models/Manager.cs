using System.ComponentModel.DataAnnotations;

namespace webapi.Models
{
    public class Manager
    {
        [Key]
        public Guid Id { get; set; }
        public required virtual ApplicationUser Profile { get; set; }
        public ICollection<Restaurant> Restaurants { get; set; } = new List<Restaurant>();
    }
}
