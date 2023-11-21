namespace webapi.Services
{
    public class PicturesAndIconsService
    {
        public string SaveImage(IFormFile imageFile)
        {
            string fileName = "";
#pragma warning disable CS0168 // Variable is declared but never used
            try
            {
                var path = Path.Combine(Directory.GetCurrentDirectory(), "D:\\Repos\\OptimaRestaurant\\angularapp\\src\\assets\\uploads");
                var returnPath = Path.Combine("../../../../assets/uploads");
                if (!Directory.Exists(path)) Directory.CreateDirectory(path);

                var ext = Path.GetExtension(imageFile.FileName);
                var allowedExtensions = new string[] { ".jpg", ".png", ".jpeg" };
                if (!allowedExtensions.Contains(ext)) return fileName;

                string uniqueString = Guid.NewGuid().ToString();
                var newFileName = uniqueString + ext;

                var fileWithPath = Path.Combine(path, newFileName);
                var returnFileWithPath = returnPath + $"/{newFileName}";
                var stream = new FileStream(fileWithPath, FileMode.Create);
                imageFile.CopyTo(stream);
                stream.Close();

                return returnFileWithPath;
            }
            catch (Exception ex)
            {
                return fileName;
            }
#pragma warning restore CS0168 // Variable is declared but never used
        }
        public bool DeleteImage(string imageFileUrl)
        {
#pragma warning disable CS0168 // Variable is declared but never used
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
#pragma warning restore CS0168 // Variable is declared but never used
        }
    }
}
