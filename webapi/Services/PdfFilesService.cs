using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using webapi.Models;

namespace webapi.Services
{
    public class PdfFilesService
    {
        public byte[] GenerateCv(Employee employee)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                using (PdfWriter writer = new PdfWriter(stream))
                {
                    using (PdfDocument pdf = new PdfDocument(writer))
                    {
                        Document document = new Document(pdf);
                        document.Add(new Paragraph("Hello, this is your PDF content!"));
                        document.Close();
                    }
                }

                return stream.ToArray();
            }
        }
    }
}
