namespace webapi.Services
{
    public class PicturesAndIconsService
    {
        public string SaveImage(IFormFile imageFile)
        {
            string fileName = "";
            try
            {
                var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\Uploads\\PicturesAndIcons");
                if (!Directory.Exists(path)) Directory.CreateDirectory(path);

                var ext = Path.GetExtension(imageFile.FileName);
                var allowedExtensions = new string[] { ".jpg", ".png", ".jpeg" };
                if (!allowedExtensions.Contains(ext)) return fileName;

                string uniqueString = Guid.NewGuid().ToString();
                var newFileName = uniqueString + ext;

                var fileWithPath = Path.Combine(path, newFileName);
                var stream = new FileStream(fileWithPath, FileMode.Create);
                imageFile.CopyTo(stream);
                stream.Close();

                return fileWithPath;
            }
            catch (Exception ex)
            {
                return fileName;
            }
        }
        public bool DeleteImage(string imageFileUrl)
        {
            try
            {
                var path = Path.Combine(imageFileUrl);
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
        }
    }
}
