namespace webapi.DTOs.Employee
{
    public class EmployeeDto
    {
        public required string Email { get; set; }
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public string? ProfilePicturePath { get; set; }
        public required decimal EmployeeAverageRating { get; set; }

    }
}
