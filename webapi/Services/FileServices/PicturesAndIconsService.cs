namespace webapi.Services.FileServices
{
    /// <summary>
    /// The service takes care of all pictures related to restaurant and users
    /// </summary>
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
            try
            {
                string path = Directory.GetCurrentDirectory();

                path = Path.Combine(path, _configuration["Pictures:Path"]);
                if (!Directory.Exists(path)) Directory.CreateDirectory(path);

                var ext = Path.GetExtension(imageFile.FileName);
                var allowedExtensions = new string[] { ".jpg", ".png", ".jpeg" };
                if (!allowedExtensions.Contains(ext)) return onlinePath;

                string uniqueString = Guid.NewGuid().ToString();
                var newFileName = uniqueString + ext;

                var fileWithPath = Path.Combine(path, newFileName);
                onlinePath = "../../assets/uploads/pictures" + $"/{newFileName}";
                var stream = new FileStream(fileWithPath, FileMode.Create);
                imageFile.CopyTo(stream);
                stream.Close();

                return onlinePath;
            }
            catch (Exception ex)
            {
                return onlinePath;
            }
        }
        public bool DeleteImage(string imageFileUrl)
        {
            try
            {
                string path = Directory.GetCurrentDirectory();
                DirectoryInfo pathInfo = Directory.GetParent(path);

                path = Path.Combine(pathInfo.FullName, _configuration["Pictures:Path"]) + imageFileUrl.Split('/').Last();
                if (File.Exists(path))
                {
                    File.Delete(path);
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
