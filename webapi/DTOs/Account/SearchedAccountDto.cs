namespace webapi.DTOs.Account
{
    public class SearchedAccountDto
    {
        public required string FullName { get; set; }
        public required string Email { get; set; }
        public required string Role { get; set; }
        public IFormFile? Picture { get; set; }
    }
}
