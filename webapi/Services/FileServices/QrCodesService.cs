using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
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
            string path;

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

            using (var image = new Image<Rgba32>(pixelData.Width, pixelData.Height))
            {
                for (int y = 0; y < pixelData.Height; y++)
                {
                    for (int x = 0; x < pixelData.Width; x++)
                    {
                        image[x, y] = new Rgba32(pixelData.Pixels[y * pixelData.Width + x]);
                    }
                }

                path = _configuration["QrCodes:Path"] + Guid.NewGuid();
                using (var outputStream = File.OpenWrite(path))
                {
                    image.SaveAsPng(outputStream);
                }
            }

            return path;
        }
        public bool DeleteQrCode(string path)
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
