namespace webapi.Services
{
    public class PicturesAndIconsService
    {

        private readonly IConfiguration _configuration;
        public PicturesAndIconsService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string SaveImage(IFormFile imageFile)
        {
            string onlinePath = "";
#pragma warning disable CS0168 // Variable is declared but never used
            try
            {
                var path = Path.Combine(Directory.GetCurrentDirectory(), _configuration["Images:Path"] ?? string.Empty);
                if (!Directory.Exists(path)) Directory.CreateDirectory(path);

                var ext = Path.GetExtension(imageFile.FileName);
                var allowedExtensions = new string[] { ".jpg", ".png", ".jpeg" };
                if (!allowedExtensions.Contains(ext)) return onlinePath;

                string uniqueString = Guid.NewGuid().ToString();
                var newFileName = uniqueString + ext;

                var fileWithPath = Path.Combine(path, newFileName);
                onlinePath = "../../../../assets/uploads/pictures" + $"/{newFileName}";
                var stream = new FileStream(fileWithPath, FileMode.Create);
                imageFile.CopyTo(stream);
                stream.Close();

                return onlinePath;
            }
            catch (Exception ex)
            {
                return onlinePath;
            }
#pragma warning restore CS0168 // Variable is declared but never used
        }
        public bool DeleteImage(string imageFileUrl)
        {
#pragma warning disable CS0168 // Variable is declared but never used
            try
            {
                var path = Path.Combine(Directory.GetCurrentDirectory(), _configuration["QrCode:Path"] ?? string.Empty) + imageFileUrl.Split('/').Last();
                if (System.IO.File.Exists(path))
                {
                    System.IO.File.Delete(path);
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                return false;
            }
#pragma warning restore CS0168 // Variable is declared but never used
        }
    }
}
