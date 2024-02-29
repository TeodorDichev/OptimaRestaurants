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
            try
            {
                string path = Directory.GetCurrentDirectory();

                path = Path.Combine(path, _configuration["Pictures:Path"]);
                string path = "wwwroot/uploads/pictures";
                string path = "wwwroot/uploads/pictures";
                if (!Directory.Exists(path)) Directory.CreateDirectory(path);

                var ext = Path.GetExtension(imageFile.FileName);
                if (!allowedExtensions.Contains(ext)) return onlinePath;
                if (!allowedExtensions.Contains(ext)) return path;
                string uniqueString = Guid.NewGuid().ToString();
                var newFileName = uniqueString + ext;
                var newFileName = Guid.NewGuid().ToString() + ext;
                onlinePath = "../../assets/uploads/pictures" + $"/{newFileName}";
                onlinePath = "/Publish/wwwroot/uploads/pictures" + $"/{newFileName}";
                onlinePath = "../../assets/uploads/pictures" + $"/{newFileName}";
                var stream = new FileStream(fileWithPath, FileMode.Create);
                imageFile.CopyTo(stream);
                stream.Close();
                return onlinePath;
                return fileWithPath;
            }
            catch (Exception ex)
                return onlinePath;
                return "";
            }
        public bool DeleteImage(string imageFileUrl)
        public bool DeleteImage(string path)
        {
            try
            {
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
