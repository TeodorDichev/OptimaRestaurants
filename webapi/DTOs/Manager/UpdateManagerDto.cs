namespace webapi.DTOs.Manager
{
    public class UpdateManagerDto
    {
        public string? NewFirstName { get; set; }
        public string? NewLastName { get; set; }
        public string? NewPhoneNumber { get; set; }
        public IFormFile? ProfilePictureFile { get; set; }
    }
}
