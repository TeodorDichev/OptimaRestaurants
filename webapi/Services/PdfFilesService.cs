using iText.IO.Font;
using iText.IO.Font.Constants;
using iText.Kernel.Font;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using System.Drawing;
using System.Text;
using webapi.Data;
using webapi.Models;

namespace webapi.Services
{
    public class PdfFilesService
    {
        private readonly OptimaRestaurantContext _context;
        public PdfFilesService(OptimaRestaurantContext context)
        {
            _context = context;
        }
        
        public byte[] GenerateCv(Employee employee)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                using (PdfWriter writer = new PdfWriter(stream))
                {
                    using (PdfDocument pdf = new PdfDocument(writer))
                    {
                        Document document = new Document(pdf);

                        string fontPath = Directory.GetCurrentDirectory() + "\\wwwroot\\resources\\dejavu-fonts-ttf-2.37\\ttf\\DejaVuSans.ttf";
                        PdfFont font = PdfFontFactory.CreateFont(fontPath, PdfEncodings.IDENTITY_H);
                        Text title = new Text($"{employee.Profile.FirstName} {employee.Profile.LastName}\n").SetFont(font).SetFontSize(32);

                        Text info = new Text(
                            $"Лични данни: \n" +
                            $"Дата на раждане: {employee.BirthDate.ToShortDateString()}\n" +
                            $"Местоживеене: {employee.City}\n" +
                            $"Регистриран на: {employee.Profile.DateCreated.ToShortDateString()}\n" +
                            $"Първа работа започната на: {employee.EmployeesRestaurants.OrderBy(er => er.StartedOn).FirstOrDefault()?.StartedOn.ToString() ?? "Няма такава"}\n\n"
                            ).SetFont(font);

                        Text stats = new Text(
                            $"Оценки и статистика: \n" +
                            $"Средната ми оценка е: {employee.EmployeeAverageRating ?? 0} брой ревюта ({_context.CustomerReviews.Where(cr => cr.Employee == employee).Count() + _context.ManagerReviews.Where(mr => mr.Employee == employee).Count()})\n" +
                            $"Оценката на моята колегиалност е: {employee.CollegialityAverageRating ?? 0}\n" +
                            $"Оценката на отношението ми към гости е: {employee.AttitudeAverageRating ?? 0}\n" +
                            $"Оценката на моята точност е: {employee.PunctualityAverageRating ?? 0}\n" +
                            $"Оценката на моята бързина и отзивчивост е: {employee.SpeedAverageRating ?? 0}\n\n")
                            .SetFont(font);

                        StringBuilder workplacestr = new StringBuilder();
                        foreach (var r in employee.EmployeesRestaurants.Where(er => er.EndedOn == null).Select(er => er.Restaurant))
                        {
                            workplacestr.AppendLine($"Име: {r.Name} Населено място: {r.City} Адрес: {r.Address}");
                            workplacestr.AppendLine(
                                $"Средната оценка на ресторанта е: {r.RestaurantAverageRating ?? 0}" +
                                $"Средната оценка на работниците в ресторанта е: {r.EmployeesAverageRating ?? 0}" +
                                $"Средната оценка на храната в ресторанта е: {r.CuisineAverageRating ?? 0}" +
                                $"Средната оценка на обстановката в ресторанта е: {r.AtmosphereAverageRating ?? 0}\n");
                        }

                        Text workplace = new Text(
                            $"Сега работя в: \n" + 
                            $"{workplacestr}\n"
                            ).SetFont(font);

                        StringBuilder historystr = new StringBuilder();
                        foreach (var r in employee.EmployeesRestaurants.Where(er => er.EndedOn != null).Select(er => er.Restaurant))
                        {
                            historystr.AppendLine($"Име: {r.Name} Населено място: {r.City} Адрес: {r.Address}");
                            historystr.AppendLine(
                                $"Средната оценка на ресторанта е: {r.RestaurantAverageRating ?? 0}" +
                                $"Средната оценка на работниците в ресторанта е: {r.EmployeesAverageRating ?? 0}" +
                                $"Средната оценка на храната в ресторанта е: {r.CuisineAverageRating ?? 0}" +
                                $"Средната оценка на обстановката в ресторанта е: {r.AtmosphereAverageRating ?? 0}\n");
                        }

                        Text history = new Text(
                            $"Работил съм в: \n" +
                            $"{historystr}\n"
                            ).SetFont(font);
                        document.Add(new Paragraph(title).Add(info).Add(stats).Add(workplace).Add(history));
                        document.Close();
                    }
                }

                return stream.ToArray();
            }
        }
    }
}
