using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using webapi.Data;
using webapi.DTOs.Employee;
using webapi.DTOs.Request;
using webapi.DTOs.Restaurant;
using webapi.Models;
using webapi.Services.ClassServices;
using webapi.Services.FileServices;

namespace webapi.Controllers
{
    /// <summary>
    /// This class manages all employee related functions
    /// Edit - city, birthdate
    /// Add/Delete a request to a manager's restaurant
    /// </summary>
    public class EmployeeController : Controller
    {
        private readonly OptimaRestaurantContext _context;
        private readonly PdfFilesService _pdfFilesService;
        private readonly EmployeeService _employeeService;
        private readonly RestaurantService _restaurantService;

        public EmployeeController(OptimaRestaurantContext context,
            PdfFilesService pdfFilesService,
            EmployeeService employeeService,
            RestaurantService restaurantService)
        {
            _context = context;
            _pdfFilesService = pdfFilesService;
            _employeeService = employeeService;
            _restaurantService = restaurantService;
        }

        [HttpGet("api/employee/get-employee/{email}")]
        [HttpGet("api/manager/browse-employees/details/{email}")]
        public async Task<ActionResult<EmployeeMainViewDto>> GetEmployee(string email)
        {
            if (await _employeeService.CheckEmployeeExistByEmail(email)) return await GenerateNewEmployeeDto(email);
            else return BadRequest("Потребителят не съществува!");
        }

        [HttpPut("api/employee/update-employee/{email}")]
        public async Task<ActionResult<EmployeeMainViewDto>> UpdateEmployeeAccount([FromForm] UpdateEmployeeDto updateDto, string email)
        {
            Employee employee;
            if (! await _employeeService.CheckEmployeeExistByEmail(email)) return BadRequest("Потребителят не съществува");
            else employee = await _employeeService.GetEmployeeByEmail(email);

            _employeeService.UpdateEmployee(employee, updateDto);
            await _employeeService.SaveChangesAsync();

            return await GenerateNewEmployeeDto(email);
        }

        [HttpDelete("api/employee/delete-employee/{email}")]
        public async Task<IActionResult> DeleteEmployeeAccount(string email)
        {
            Employee employee;
            if (! await _employeeService.CheckEmployeeExistByEmail(email)) return BadRequest("Потребителят не съществува");
            else employee = await _employeeService.GetEmployeeByEmail(email);

            if (await _employeeService.DeleteEmployee(employee))
            {
                await _employeeService.SaveChangesAsync();
                return Ok(new JsonResult(new { title = "Успешно изтриване!", message = "Успешно изтрихте своя акаунт!" }));
            }
            else return BadRequest("Неуспешно изтриване!");
        }

        [HttpGet("api/employee/get-all-requests/{email}")]
        public async Task<ActionResult<List<RequestDto>>> GetRequests(string email)
        {
            if (! await _employeeService.CheckEmployeeExistByEmail(email)) return BadRequest("Потребителят не съществува");

            List<RequestDto> requests = new List<RequestDto>();
            foreach (var r in _context.Requests.Where(r => r.Receiver.Email == email).OrderBy(x => x.SentOn))
            {
                bool? confirmed = null;
                if (r.ConfirmedOn != null) confirmed = true;
                if (r.RejectedOn != null) confirmed = false;

                var request = new RequestDto
                {
                    Id = r.Id.ToString(),
                    RestaurantId = r.Restaurant.Id.ToString(),
                    SenderEmail = r.Sender.Email ?? string.Empty,
                    SentOn = r.SentOn.ToString(),
                    Confirmed = confirmed,
                    Text = $"Работите ли в ресторантът {r.Restaurant.Name}, собственост на {r.Sender.FirstName + " " + r.Sender.LastName}?"
                };

                requests.Add(request);
            }

            return Ok(requests);
        }

        [HttpPost("api/employee/respond-to-request")]
        public async Task<IActionResult> RespondToRequest([FromBody] ResponceToRequestDto requestDto)
        {
            var request = _context.Requests.FirstOrDefault(r => r.Id.ToString() == requestDto.RequestId);
            if (request == null) return BadRequest("Заявката не съществува!");
            if (request.ConfirmedOn != null || request.RejectedOn != null) return BadRequest("Заявката вече е отговорена!");


            var employeeProfile = request.Receiver;
            var employee = await _context.Employees.FirstOrDefaultAsync(m => m.Profile.Email == employeeProfile.Email);
            if (employeeProfile == null || employee == null) return BadRequest("Потребителят не съществува!");

            var managerProfile = request.Sender;
            var manager = await _context.Managers.FirstOrDefaultAsync(e => e.Profile.Email == managerProfile.Email);
            if (managerProfile == null || manager == null) return BadRequest("Потребителят изпратил заявката не съществува!");

            var restaurant = await _context.Restaurants.FirstOrDefaultAsync(r => r.Id.ToString() == requestDto.RestaurantId);
            if (restaurant == null) return BadRequest("Ресторантът не съществува!");
            if (!restaurant.IsWorking) return BadRequest("Ресторантът не работи!");

            if (requestDto.Confirmed)
            {
                request.ConfirmedOn = DateTime.UtcNow;
                EmployeeRestaurant er = new EmployeeRestaurant
                {
                    Employee = employee,
                    Restaurant = restaurant,
                    StartedOn = DateTime.UtcNow,
                };
                employee.EmployeesRestaurants.Add(er);
                restaurant.EmployeesRestaurants.Add(er);
                await _context.EmployeesRestaurants.AddAsync(er);
                await _context.SaveChangesAsync();
                return Ok(new JsonResult(new { title = "Успешно потвърдена заявка!", message = $"Вече работите в ресторантът {request.Restaurant.Name}, собственост на {request.Sender.FirstName + " " + request.Sender.LastName}" }));
            }
            else
            {
                request.RejectedOn = DateTime.Now;
                await _context.SaveChangesAsync();
                return Ok(new JsonResult(new { title = "Успешно отхвърлена заявка!", message = $"Заявката на {manager.Profile.FirstName} е отхвърлена!" }));
            }
        }

        [HttpGet("api/employee/download-qrcode/{email}")]
        public async Task<IActionResult> DownloadQrCode(string email)
        {
            Employee employee;
            if (! await _employeeService.CheckEmployeeExistByEmail(email)) return BadRequest("Потребителят не съществува");
            else employee = await _employeeService.GetEmployeeByEmail(email);

            string qrCodePath = _employeeService.GetEmployeeQrCodePath(employee);
            if (System.IO.File.Exists(qrCodePath))
            {
                return PhysicalFile(qrCodePath, "image/png", $"{employee.Profile.FirstName}_qrcode.png");
            }
            else
            {
                return BadRequest("Вашият QrCode не беше намерен!");
            }
        }

        [HttpGet("api/employee/download-cv/{email}")]
        public async Task<IActionResult> DownloadCV(string email)
        {
            Employee employee;
            if (! await _employeeService.CheckEmployeeExistByEmail(email)) return BadRequest("Потребителят не съществува");
            else employee = await _employeeService.GetEmployeeByEmail(email);

            return File(_pdfFilesService.GenerateCv(employee), "application/pdf", $"{employee.Profile.FirstName}_cv.pdf");
        }
        private async Task<EmployeeMainViewDto> GenerateNewEmployeeDto(string email)
        {
            var employee = await _employeeService.GetEmployeeByEmail(email);

            var employeeMainViewDto = new EmployeeMainViewDto
            {
                Email = email,
                FirstName = employee.Profile.FirstName,
                LastName = employee.Profile.LastName,
                ProfilePicturePath = employee.Profile.ProfilePicturePath,
                QrCodePath = employee.QrCodePath,
                PhoneNumber = employee.Profile.PhoneNumber ?? string.Empty,
                City = employee.City,
                BirthDate = employee.BirthDate.ToShortDateString(),
                AttitudeAverageRating = employee.AttitudeAverageRating ?? 0,
                CollegialityAverageRating = employee.CollegialityAverageRating ?? 0,
                SpeedAverageRating = employee.SpeedAverageRating ?? 0,
                PunctualityAverageRating = employee.PunctualityAverageRating ?? 0,
                EmployeeAverageRating = employee.EmployeeAverageRating ?? 0,
                Restaurants = _restaurantService.GetRestaurantsOfEmployee(employee),
                IsLookingForJob = employee.IsLookingForJob,
            };

            return employeeMainViewDto;
        }
    }
}
