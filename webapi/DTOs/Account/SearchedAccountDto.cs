namespace webapi.DTOs.Account
{
    public class SearchedAccountDto
    {
        public required string Username { get; set; }
        public required string Role { get; set; }
        public string? PictureUrl { get; set; }
    }
}
