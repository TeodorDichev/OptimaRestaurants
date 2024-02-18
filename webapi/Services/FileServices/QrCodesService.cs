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
            string onlinePath;

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

                    string path = Directory.GetCurrentDirectory();
                    DirectoryInfo pathInfo = Directory.GetParent(path);

                    path = Path.Combine(pathInfo.FullName, _configuration["QrCodes:Path"]);
                    if (!Directory.Exists(path)) Directory.CreateDirectory(path);

                    var ext = ".png";

                    string uniqueString = Guid.NewGuid().ToString();
                    var newFileName = uniqueString + ext;

                    var fileWithPath = Path.Combine(path, newFileName);
                    //onlinePath = "../../../../assets/uploads/qrcodes" + $"/{newFileName}"; //local
                    onlinePath = "assets/uploads/qrcodes" + $"/{newFileName}"; //remote

                    bitmap.Save(fileWithPath, System.Drawing.Imaging.ImageFormat.Png);

                    /* save to stream as PNG */
                    bitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                    byteArray = ms.ToArray();
                }
            }

            return onlinePath;
        }
        public bool DeleteQrCode(string qrCodeFileUrl)
        {
            try
            {
                string path = Directory.GetCurrentDirectory();
                DirectoryInfo pathInfo = Directory.GetParent(path);

                path = Path.Combine(pathInfo.FullName, _configuration["QrCodes:Path"] ?? string.Empty) + qrCodeFileUrl.Split('/').Last();
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
