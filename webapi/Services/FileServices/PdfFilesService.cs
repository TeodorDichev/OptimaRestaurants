using iText.IO.Font;
using iText.IO.Image;
using iText.Kernel.Font;
using iText.Kernel.Pdf;
using iText.Layout.Borders;
using iText.Layout.Element;
using iText.Layout.Properties;
using System.Globalization;
using webapi.Data;
using webapi.Models;

namespace webapi.Services.FileServices
{
    /// <summary>
    /// The service creates pdf CVs for employees using the iText package
    /// </summary>
    public class PdfFilesService
    {
        private readonly OptimaRestaurantContext _context;
        private readonly IConfiguration _configuration;
        public PdfFilesService(OptimaRestaurantContext context,
            IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public byte[] GenerateCv(Employee employee)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                using (PdfWriter writer = new PdfWriter(stream))
                {
                    using (PdfDocument pdf = new PdfDocument(writer))
                    {
                        iText.Layout.Document document = new iText.Layout.Document(pdf);

                        string fontPath = Directory.GetCurrentDirectory() + "\\wwwroot\\resources\\dejavu-fonts-ttf-2.37\\ttf\\DejaVuSans.ttf";
                        PdfFont font = PdfFontFactory.CreateFont(fontPath, PdfEncodings.IDENTITY_H);

                        /* Heading - the name of the employee */
                        document.Add(new Paragraph(
                            $"{employee.Profile.FirstName} {employee.Profile.LastName}\n")
                            .SetFont(font).SetFontSize(32));

                        /* Add the tables to the document */
                        document.Add(GenerateTableWithPersonalInfo(employee, font));
                        document.Add(GenerateTableWithStatistics(employee, font));
                        document.Add(new Paragraph(new Text("Настоящи работни места: ").SetFont(font).SetFontSize(16)));
                        if (employee.EmployeesRestaurants.Where(er => er.EndedOn == null).Count() == 0)
                        {
                            document.Add(new Paragraph(new Text("Липсва настояща заетост").SetFont(font).SetFontSize(12)));
                        }
                        else
                        {
                            foreach (var r in employee.EmployeesRestaurants.Where(er => er.EndedOn == null).Select(er => er.Restaurant))
                            {
                                document.Add(GenerateTableWithRestaurantStatistics(r, font));
                            }
                        }
                        document.Add(new Paragraph(new Text("Предишни работни места: ").SetFont(font).SetFontSize(16)));
                        if (employee.EmployeesRestaurants.Where(er => er.EndedOn != null).Count() == 0)
                        {
                            document.Add(new Paragraph(new Text("Липсва предишна заетост").SetFont(font).SetFontSize(12)));
                        }
                        else
                        {
                            foreach (var r in employee.EmployeesRestaurants.Where(er => er.EndedOn != null).Select(er => er.Restaurant))
                            {
                                document.Add(GenerateTableWithRestaurantStatistics(r, font));
                            }
                        }

                        document.Close();
                    }
                }

                return stream.ToArray();
            }

        }

        private Table GenerateTableWithPersonalInfo(Employee employee, PdfFont font)
        {
            /* Creating the image */
            string path = Directory.GetCurrentDirectory();
            DirectoryInfo pathInfo = Directory.GetParent(path); 

            if (employee.Profile.ProfilePicturePath != null) path = Path.Combine(pathInfo.FullName, _configuration["Pictures:Path"]) + employee.Profile.ProfilePicturePath.Split('/').Last();
            else path = Path.Combine(pathInfo.FullName, _configuration["Images:Path"], "logo-bw-with-bg.png");

            Image image = new Image(ImageDataFactory.Create(path));

            float maxWidth = 150f;
            float maxHeight = 150f;
            image.SetMaxWidth(maxWidth).SetMaxHeight(maxHeight);

            /* Creating the table */
            Table table = new Table(new UnitValue[] { UnitValue.CreatePercentValue(70), UnitValue.CreatePercentValue(30) });
            table.SetWidth(UnitValue.CreatePercentValue(100));

            /* Creating the nested table */
            Table nestedTable = new Table(1);
            nestedTable.AddCell(new Cell().Add(new Paragraph($"Лични данни: \n").SetFont(font).SetFontSize(16)).SetBorder(Border.NO_BORDER));
            nestedTable.AddCell(new Cell().Add(new Paragraph($"Дата на раждане: {employee.BirthDate.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture)}").SetFont(font).SetFontSize(12)).SetBorder(Border.NO_BORDER));
            nestedTable.AddCell(new Cell().Add(new Paragraph($"Ел. поща: {employee.Profile.Email}").SetFont(font).SetFontSize(12)).SetBorder(Border.NO_BORDER));
            nestedTable.AddCell(new Cell().Add(new Paragraph($"Телефон: {employee.Profile.PhoneNumber ?? "липсва"}").SetFont(font).SetFontSize(12)).SetBorder(Border.NO_BORDER));
            nestedTable.AddCell(new Cell().Add(new Paragraph($"Местоживеене: {employee.City}").SetFont(font).SetFontSize(12)).SetBorder(Border.NO_BORDER));
            nestedTable.AddCell(new Cell().Add(new Paragraph($"Регистриран на: {employee.Profile.DateCreated.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture)}").SetFont(font).SetFontSize(12)).SetBorder(Border.NO_BORDER));
            nestedTable.AddCell(new Cell().Add(new Paragraph($"Първа работа започната на: {employee.EmployeesRestaurants.OrderBy(er => er.StartedOn).FirstOrDefault()?.StartedOn.ToString() ?? "Няма такава"}").SetFont(font).SetFontSize(12)).SetBorder(Border.NO_BORDER));

            table.AddCell(new Cell().Add(nestedTable).SetBorder(Border.NO_BORDER));
            table.AddCell(new Cell().Add(image).SetBorder(Border.NO_BORDER));

            return table;
        }
        private Table GenerateTableWithStatistics(Employee employee, PdfFont font)
        {
            /* Creating the image */
            string path = Directory.GetCurrentDirectory();
            DirectoryInfo pathInfo = Directory.GetParent(path);

            path = Path.Combine(pathInfo.FullName, _configuration["Images:Path"], "logo-bw-with-bg.png");

            Image image = new Image(ImageDataFactory.Create(path));

            float maxWidth = 150f;
            float maxHeight = 150f;
            image.SetMaxWidth(maxWidth).SetMaxHeight(maxHeight);

            /* Creating the table */
            Table table = new Table(new UnitValue[] { UnitValue.CreatePercentValue(70), UnitValue.CreatePercentValue(30) });
            table.SetWidth(UnitValue.CreatePercentValue(100));

            /* Creating the nested table */
            Table nestedTable = new Table(1);
            nestedTable.AddCell(new Cell().Add(new Paragraph($"Оценки и статистика: \n").SetFont(font).SetFontSize(16)).SetBorder(Border.NO_BORDER));
            nestedTable.AddCell(new Cell().Add(new Paragraph($"Средната ми оценка е: {employee.EmployeeAverageRating ?? 0}/5 ({employee.TotalReviewsCount})\n").SetFont(font).SetFontSize(12)).SetBorder(Border.NO_BORDER));
            nestedTable.AddCell(new Cell().Add(new Paragraph($"Оценката на моята колегиалност е: {employee.CollegialityAverageRating ?? 0}/5\n").SetFont(font).SetFontSize(12)).SetBorder(Border.NO_BORDER));
            nestedTable.AddCell(new Cell().Add(new Paragraph($"Оценката на отношението ми към гости е: {employee.AttitudeAverageRating ?? 0}/5\n").SetFont(font).SetFontSize(12)).SetBorder(Border.NO_BORDER));
            nestedTable.AddCell(new Cell().Add(new Paragraph($"Оценката на моята точност е: {employee.PunctualityAverageRating ?? 0}/5\n").SetFont(font).SetFontSize(12)).SetBorder(Border.NO_BORDER));
            nestedTable.AddCell(new Cell().Add(new Paragraph($"Оценката на моята бързина и отзивчивост е: {employee.SpeedAverageRating ?? 0}/5\n\n").SetFont(font).SetFontSize(12)).SetBorder(Border.NO_BORDER));

            table.AddCell(new Cell().Add(nestedTable).SetBorder(Border.NO_BORDER));
            table.AddCell(new Cell().Add(image).SetBorder(Border.NO_BORDER));

            return table;
        }
        private Table GenerateTableWithRestaurantStatistics(Restaurant restaurant, PdfFont font)
        {

            /* Creating the image */
            string path = Directory.GetCurrentDirectory();
            DirectoryInfo pathInfo = Directory.GetParent(path);

            if (restaurant.IconPath != null) path = Path.Combine(pathInfo.FullName, _configuration["Pictures:Path"]) + restaurant.IconPath.Split('/').Last();
            else path = Path.Combine(pathInfo.FullName, _configuration["Images:Path"], "logo-bw-with-bg.png");

            Image image = new Image(ImageDataFactory.Create(path));

            float maxWidth = 150f;
            float maxHeight = 150f;
            image.SetMaxWidth(maxWidth).SetMaxHeight(maxHeight);

            /* Creating the table */
            Table table = new Table(new UnitValue[] { UnitValue.CreatePercentValue(70), UnitValue.CreatePercentValue(30) });
            table.SetWidth(UnitValue.CreatePercentValue(100));

            /* Creating the nested table */
            Table nestedTable = new Table(1);
            nestedTable.AddCell(new Cell().Add(new Paragraph($"Име: {restaurant.Name} ({restaurant.City})\n").SetFont(font).SetFontSize(12).SetBold()).SetBorder(Border.NO_BORDER));
            nestedTable.AddCell(new Cell().Add(new Paragraph($"Средната оценка: {restaurant.RestaurantAverageRating ?? 0}/5 ({restaurant.TotalReviewsCount})\n").SetFont(font).SetFontSize(12)).SetBorder(Border.NO_BORDER));
            nestedTable.AddCell(new Cell().Add(new Paragraph($"Служители: {restaurant.EmployeesAverageRating ?? 0}/5\n").SetFont(font).SetFontSize(12)).SetBorder(Border.NO_BORDER));
            nestedTable.AddCell(new Cell().Add(new Paragraph($"Ястия: {restaurant.CuisineAverageRating ?? 0}/5\n").SetFont(font).SetFontSize(12)).SetBorder(Border.NO_BORDER));
            nestedTable.AddCell(new Cell().Add(new Paragraph($"Обстановката: {restaurant.AtmosphereAverageRating ?? 0}/5\n").SetFont(font).SetFontSize(12)).SetBorder(Border.NO_BORDER));
            
            table.AddCell(new Cell().Add(nestedTable).SetBorder(Border.NO_BORDER));
            table.AddCell(new Cell().Add(image).SetBorder(Border.NO_BORDER));

            return table;
        }
    }
}
