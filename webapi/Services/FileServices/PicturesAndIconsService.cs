
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
                string path = Path.Combine(Directory.GetCurrentDirectory(), _configuration["Pictures:Path"]);
                if (!Directory.Exists(path)) Directory.CreateDirectory(path);

                var ext = Path.GetExtension(imageFile.FileName);
                var newFileName = Guid.NewGuid().ToString() + ext;

                var stream = new FileStream(path, FileMode.Create);
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
