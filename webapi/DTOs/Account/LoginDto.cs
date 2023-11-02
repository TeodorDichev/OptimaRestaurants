namespace webapi.DTOs.Account
{
    public class LoginDto
    {
        public required string UserName { get; set; } // same as email in our case
        public required string Password { get; set; }
        public bool IsManager { get; set; } = false;
        public string JWT { get; set; }
    }
}
