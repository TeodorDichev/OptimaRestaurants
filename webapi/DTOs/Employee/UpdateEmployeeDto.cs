namespace webapi.DTOs.Employee
{
    public class UpdateEmployeeDto
    {
        public string? NewFirstName { get; set; }
        public string? NewLastName { get; set; }
        public string? NewPhoneNumber { get; set; }
        public IFormFile? ProfilePictureFile { get; set; }
        public DateTime? NewBirthDate { get; set; }
        public string? NewCity { get; set; }
        public bool IsLookingForJob { get; set; }
    }
}
