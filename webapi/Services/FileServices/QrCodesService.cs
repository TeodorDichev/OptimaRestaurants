using ZXing.QrCode;

namespace webapi.Services.FileServices
{
    /// <summary>
    /// The service creates QR codes for employees using the ZXing package
    /// Which are unique for every employee
    /// </summary>

    public class QrCodesService
    {
        private readonly IConfiguration _configuration;
        public QrCodesService(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public string GenerateQrCode(string url)
        {
            byte[] byteArray;

            /* Configuring QR code dimensions */
            var width = 250;
            var height = 250;
            var margin = 0;

            var qrCodeWriter = new ZXing.BarcodeWriterPixelData
            {
                Format = ZXing.BarcodeFormat.QR_CODE,
                Options = new QrCodeEncodingOptions
                {
                    Height = height,
                    Width = width,
                    Margin = margin
                }
            };
            var pixelData = qrCodeWriter.Write(url);

            /* creating a bitmap from the raw pixel data; if only black and white colors are used it makes
             * no difference that the pixel data ist BGRA oriented and the bitmap is initialized with RGB */
            var newFileName = Guid.NewGuid().ToString() + ".png";
            using (var bitmap = new System.Drawing.Bitmap(pixelData.Width, pixelData.Height, System.Drawing.Imaging.PixelFormat.Format32bppRgb))
            {
                using (var ms = new MemoryStream())
                {
                    var bitmapData = bitmap.LockBits(
                        new System.Drawing.Rectangle(0, 0, pixelData.Width, pixelData.Height), 
                        System.Drawing.Imaging.ImageLockMode.WriteOnly, 
                        System.Drawing.Imaging.PixelFormat.Format32bppRgb);
                    try
                    {
                        System.Runtime.InteropServices.Marshal.Copy(pixelData.Pixels, 0, bitmapData.Scan0, pixelData.Pixels.Length);
                    }
                    finally
                    {
                        bitmap.UnlockBits(bitmapData);
                    }

                    string localPath = Path.Combine(Directory.GetParent(Directory.GetCurrentDirectory()).FullName, _configuration["QrCodes:LocalPath"]);
                    if (!Directory.Exists(localPath)) Directory.CreateDirectory(localPath);

                    bitmap.Save(localPath + newFileName, System.Drawing.Imaging.ImageFormat.Png);
                    bitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                    byteArray = ms.ToArray();
                }
            }

            return _configuration["QrCodes:OnlinePath"] + newFileName;
        }
        public bool DeleteQrCode(string path)
        {
            string localPath = Path.Combine(Directory.GetParent(Directory.GetCurrentDirectory()).FullName, _configuration["Pictures:LocalPath"]);
            localPath = Path.Combine(localPath, path.Split('/').Last());

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
