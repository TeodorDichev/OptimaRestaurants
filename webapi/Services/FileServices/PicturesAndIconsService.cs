
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
                string path = Path.Combine(Directory.GetParent(Directory.GetCurrentDirectory()).FullName, _configuration["Pictures:LocalPath"]);
                if (!Directory.Exists(path)) Directory.CreateDirectory(path);

                string[] allowedExtensions = { ".jpg", ".png", ".jpeg" };
                var ext = Path.GetExtension(imageFile.FileName);
                if (!allowedExtensions.Contains(ext)) throw new ArgumentException("Incorrect extension!");

                var newFileName = Guid.NewGuid().ToString() + ext;
                path = Path.Combine(path, newFileName);

                var stream = new FileStream(path, FileMode.Create);
                imageFile.CopyTo(stream);
                stream.Close();

                return _configuration["Pictures:OnlinePath"] + newFileName;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public bool DeleteImage(string path)
        {
            string localPath = Path.Combine(Directory.GetParent(Directory.GetCurrentDirectory()).FullName, _configuration["Pictures:LocalPath"]);
            localPath = Path.Combine(localPath, path.Split('/').Last());

            try
            {
                if (File.Exists(localPath))
                {
                    File.Delete(localPath);
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
