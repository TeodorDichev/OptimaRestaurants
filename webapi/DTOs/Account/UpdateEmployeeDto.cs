using System.ComponentModel.DataAnnotations;
using webapi.Models;

namespace webapi.DTOs.Account
{
    public class UpdateEmployeeDto
    {
        public virtual Employee CurrentEmployee { get; set; } // fill in by the current user in their account
        public string NewFirstName { get; set; }
        public string NewLastName { get; set; }
        public string NewPhoneNumber { get; set; }

        [RegularExpression("^\\w+@[a-zA-Z_]+?\\.[a-zA-Z]{2,3}$", ErrorMessage = "Invalid email address")]
        public string NewEmail { get; set; }
        public string NewPictureUrl { get; set; }
        public DateTime NewBirthDate { get; set; }
        public string NewCity { get; set; }
    }
}
