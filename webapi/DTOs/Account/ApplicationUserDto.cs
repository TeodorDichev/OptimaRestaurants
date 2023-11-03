namespace webapi.DTOs.Account
{
    public class ApplicationUserDto
    {
        public string Email { get; set; }
        public string JWT { get; set; }
        public bool IsManager { get; set; } = false;
    }
}
