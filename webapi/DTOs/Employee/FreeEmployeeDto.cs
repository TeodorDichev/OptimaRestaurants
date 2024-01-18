namespace webapi.DTOs.Employee
{
    public class FreeEmployeeDto
    {
        public required string EmployeeEmail { get; set; }
        public required string EmployeeName { get; set; }
        public required string RestaurantName { get; set; }
        public TimeOnly? From { get; set; }
        public TimeOnly? To { get; set; }
    }
}
