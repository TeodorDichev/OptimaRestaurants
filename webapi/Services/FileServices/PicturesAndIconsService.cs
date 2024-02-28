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

        public IFormFile GetImageFile(string path)
        {
            using (var stream = System.IO.File.OpenRead(path))
            {
                try
                {
                    FormFile file = new FormFile(stream, 0, stream.Length, null, Path.GetFileName(stream.Name));
                    return file;
                }
                catch (Exception)
                {
                    return null;
                }
            };
        }

        public string SaveImage(IFormFile imageFile)
        {
            try
            {
                string path = _configuration["Pictures:Path"];
                if (!Directory.Exists(path)) Directory.CreateDirectory(path);

                var ext = Path.GetExtension(imageFile.FileName);
                var allowedExtensions = new string[] { ".jpg", ".png", ".jpeg" };
                if (!allowedExtensions.Contains(ext)) return path;

                var newFileName = Guid.NewGuid().ToString() + ext;

                var fileWithPath = Path.Combine(path, newFileName);
                var stream = new FileStream(fileWithPath, FileMode.Create);
                imageFile.CopyTo(stream);
                stream.Close();

                return path;
            }
            catch (Exception ex)
            {
                return "";
            }
        }

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
