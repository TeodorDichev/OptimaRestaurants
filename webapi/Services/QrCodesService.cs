using System;
using System.Drawing;
using ZXing;
using ZXing.Common;
using ZXing.QrCode;

namespace webapi.Services
{
    public class QrCodesService
    {
        private readonly IConfiguration _configuration;
        public QrCodesService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string SaveQrCode(string url)
        {
            string outputPath = "qrcode.svg"; // Replace with your desired output path and file extension

            // Create a ZXing QR code writer with SvgBarcodeWriter
            BarcodeWriterSvg barcodeWriter = new BarcodeWriterSvg
            {
                Format = BarcodeFormat.QR_CODE,
                Options = new ZXing.Common.EncodingOptions
                {
                    Width = 300, // Adjust the width of the QR code as needed
                    Height = 300, // Adjust the height of the QR code as needed
                    Margin = 10 // Adjust the margin of the QR code as needed
                }
            };

            // Generate the QR code as SVG string
            string svgCode = barcodeWriter.Write(url).ToString();

            // Save the SVG code to a file
            File.WriteAllText(outputPath, svgCode);

            return outputPath;
        }
    }
}
