using Microsoft.AspNetCore.Mvc;
using webapi.DTOs.Employee;
using webapi.DTOs.Request;
using webapi.Models;
using webapi.Services.ClassServices;
using webapi.Services.FileServices;
using webapi.Services.ModelServices;

namespace webapi.Controllers
{
    /// <summary>
    /// EmployeeController manages employees:
    /// CRUD operations for their profiles,
    /// Their requests, qr codes, cvs
    /// </summary>
    public class EmployeeController : Controller
    {
        private readonly PdfFilesService _pdfFilesService;
        private readonly EmployeeService _employeeService;
        private readonly ManagerService _managerService;
        private readonly RequestService _requestService;
        private readonly RestaurantService _restaurantService;

        public EmployeeController(PdfFilesService pdfFilesService,
            EmployeeService employeeService,
            RequestService requestService,
            ManagerService managerService,
            RestaurantService restaurantService)
        {
            _pdfFilesService = pdfFilesService;
            _employeeService = employeeService;
            _restaurantService = restaurantService;
            _requestService = requestService;
            _managerService = managerService;
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
            if (!await _employeeService.CheckEmployeeExistByEmail(email)) return BadRequest("Потребителят не съществува");
            else employee = await _employeeService.GetEmployeeByEmail(email);

            _employeeService.UpdateEmployee(employee, updateDto);
            await _employeeService.SaveChangesAsync();

            return await GenerateNewEmployeeDto(email);
        }

        [HttpDelete("api/employee/delete-employee/{email}")]
        public async Task<IActionResult> DeleteEmployeeAccount(string email)
        {
            Employee employee;
            if (!await _employeeService.CheckEmployeeExistByEmail(email)) return BadRequest("Потребителят не съществува");
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
            if (!await _employeeService.CheckEmployeeExistByEmail(email)) return BadRequest("Потребителят не съществува");

            return _requestService.GetEmployeeRequests(email);
        }

        [HttpPost("api/employee/respond-to-request")]
        public async Task<IActionResult> RespondToRequest([FromBody] ResponceToRequestDto requestDto)
        {
            Request request;
            if (!await _requestService.CheckRequestExistById(requestDto.RequestId)) return BadRequest("Заявката не съществува");
            else request = await _requestService.GetRequestById(requestDto.RequestId);
            if (request.ConfirmedOn != null || request.RejectedOn != null) return BadRequest("Заявката вече е отговорена!");

            Employee employee;
            if (!await _employeeService.CheckEmployeeExistByEmail(request.Receiver.Email ?? string.Empty)) return BadRequest("Потребителят не съществува");
            else employee = await _employeeService.GetEmployeeByEmail(request.Receiver.Email ?? string.Empty);

            Manager manager;
            if (!await _managerService.CheckManagerExistByEmail(request.Sender.Email ?? string.Empty)) return BadRequest("Потребителят не съществува");
            else manager = await _managerService.GetManagerByEmail(request.Sender.Email ?? string.Empty);

            Restaurant restaurant;
            if (!await _restaurantService.CheckRestaurantExistById(request.Restaurant.Id.ToString())) return BadRequest("Ресторантът не съществува!");
            else restaurant = await _restaurantService.GetRestaurantById(request.Restaurant.Id.ToString());
            if (!restaurant.IsWorking) return BadRequest("Ресторантът не работи!");

            if (await _requestService.RespondToRequest(employee, restaurant, request, requestDto.Confirmed))
            {
                await _requestService.SaveChangesAsync();
                return Ok(new JsonResult(new { title = "Успешно потвърдена заявка!", message = $"Вече работите в ресторантът {request.Restaurant.Name}, собственост на {request.Sender.FirstName + " " + request.Sender.LastName}" }));
            }
            else
            {
                await _requestService.SaveChangesAsync();
                return Ok(new JsonResult(new { title = "Успешно отхвърлена заявка!", message = $"Заявката на {manager.Profile.FirstName} е отхвърлена!" }));
            }
        }

        [HttpGet("api/employee/download-qrcode/{email}")]
        public async Task<IActionResult> DownloadQrCode(string email)
        {
            Employee employee;
            if (!await _employeeService.CheckEmployeeExistByEmail(email)) return BadRequest("Потребителят не съществува");
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
            if (!await _employeeService.CheckEmployeeExistByEmail(email)) return BadRequest("Потребителят не съществува");
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
                TotalReviewsCount = employee.TotalReviewsCount,
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
