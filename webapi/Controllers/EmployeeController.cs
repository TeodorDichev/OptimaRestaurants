using Microsoft.AspNetCore.Mvc;
using webapi.DTOs.Employee;
using webapi.DTOs.Request;
using webapi.DTOs.Schedule;
using webapi.Models;
using webapi.Services.ClassServices;
using webapi.Services.FileServices;
using webapi.Services.ModelServices;

namespace webapi.Controllers
{
    /// <summary>
    /// EmployeeController manages employees:
    /// CRUD operations for their profiles,
    /// Their schedules, requests, qr codes, cvs
    /// </summary>
    public class EmployeeController : Controller
    {
        private readonly PdfFilesService _pdfFilesService;
        private readonly EmployeeService _employeeService;
        private readonly ManagerService _managerService;
        private readonly RequestService _requestService;
        private readonly RestaurantService _restaurantService;
        private readonly ScheduleService _scheduleService;
        public EmployeeController(PdfFilesService pdfFilesService,
            EmployeeService employeeService,
            RequestService requestService,
            ManagerService managerService,
            RestaurantService restaurantService,
            ScheduleService scheduleService)
        {
            _pdfFilesService = pdfFilesService;
            _employeeService = employeeService;
            _restaurantService = restaurantService;
            _requestService = requestService;
            _managerService = managerService;
            _scheduleService = scheduleService;
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

            await _employeeService.UpdateEmployee(employee, updateDto);
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

        [HttpGet("api/employee/get-restaurant-schedule/{email}/{restaurantId}/{month}")]
        public async Task<ActionResult<List<EmployeeFullScheduleDto>>> GetEmployeeRestaurantSchedule(string email, string restaurantId, int month)
        {
            Employee employee;
            if (!await _employeeService.CheckEmployeeExistByEmail(email)) return BadRequest("Потребителят не съществува");
            else employee = await _employeeService.GetEmployeeByEmail(email);

            Restaurant restaurant;
            if (!await _restaurantService.CheckRestaurantExistById(restaurantId)) return BadRequest("Ресторантът не съществува!");
            else restaurant = await _restaurantService.GetRestaurantById(restaurantId);

            return await _scheduleService.GetAssignedDaysOfEmployeeInRestaurant(employee, restaurant, month);
        }

        [HttpGet("api/employee/get-full-schedule/{email}/{month}")]
        public async Task<ActionResult<List<EmployeeFullScheduleDto>>> GetEmployeeFullSchedule(string email, int month)
        {
            Employee employee;
            if (!await _employeeService.CheckEmployeeExistByEmail(email)) return BadRequest("Потребителят не съществува");
            else employee = await _employeeService.GetEmployeeByEmail(email);

            return await _scheduleService.GetAssignedDaysOfEmployee(employee, month);
        }

        [HttpGet("api/employee/get-day-schedule/{email}/{day}")]
        public async Task<ActionResult<List<EmployeeDailyScheduleDto>>> GetDailySchedule(string email, DateTime day)
        {
            Employee employee;
            if (!await _employeeService.CheckEmployeeExistByEmail(email)) return BadRequest("Потребителят не съществува");
            else employee = await _employeeService.GetEmployeeByEmail(email);

            return _scheduleService.GetEmployeeDailySchedule(employee, day.ToLocalTime());
        }

        /// <summary>
        /// Cannot be added when employee is on all restaurants
        /// Employee can only add/edit/delete non-working assignments with minimum one week notice
        /// </summary>
        /// <param name="scheduleDto"> Adding an assignment to their schedule </param>
        /// <returns> The schedule for the day </returns>

        [HttpPost("api/employee/schedule/add-assignment")]
        public async Task<ActionResult<List<EmployeeDailyScheduleDto>>> AddAssignment([FromBody] CreateScheduleDto scheduleDto)
        {
            scheduleDto.Day = scheduleDto.Day.ToLocalTime();
            if (scheduleDto.From.HasValue) scheduleDto.From = scheduleDto.From.Value.ToLocalTime();
            if (scheduleDto.To.HasValue) scheduleDto.To = scheduleDto.To.Value.ToLocalTime();

            if (scheduleDto.Day.Subtract(DateTime.Now.Date).Days < 7) return BadRequest("Добавянето на почивни дни трябва да става със седемдневно предизвестие!");

            Employee employee;
            if (!await _employeeService.CheckEmployeeExistByEmail(scheduleDto.EmployeeEmail)) return BadRequest("Потребителят не съществува");
            else employee = await _employeeService.GetEmployeeByEmail(scheduleDto.EmployeeEmail);

            Restaurant restaurant;
            if (!await _restaurantService.CheckRestaurantExistById(scheduleDto.RestaurantId)) return BadRequest("Ресторантът не съществува!");
            else restaurant = await _restaurantService.GetRestaurantById(scheduleDto.RestaurantId);
            if (!restaurant.IsWorking) return BadRequest("Ресторантът не работи!");

            /* Employee can only add leisure days */
            scheduleDto.IsWorkDay = false;

            if (await _scheduleService.CanEmployeeTakeVacationOn(employee, restaurant, scheduleDto.Day, scheduleDto.From, scheduleDto.To))
            {
                await _scheduleService.AddAssignmentToSchedule(scheduleDto);
                await _scheduleService.SaveChangesAsync();
                return await GetDailySchedule(scheduleDto.EmployeeEmail, scheduleDto.Day);
            }
            else return BadRequest("Вече имате запазен друг ангажимент!");
        }

        [HttpPut("api/employee/schedule/edit-assignment")]
        public async Task<ActionResult<List<EmployeeDailyScheduleDto>>> EditAssignment([FromBody] ScheduleDto scheduleDto)
        {
            scheduleDto.Day = scheduleDto.Day.ToLocalTime();
            if (scheduleDto.From.HasValue) scheduleDto.From = scheduleDto.From.Value.ToLocalTime();
            if (scheduleDto.To.HasValue) scheduleDto.To = scheduleDto.To.Value.ToLocalTime();

            if (scheduleDto.Day.Subtract(DateTime.Now.Date).Days < 7) return BadRequest("Промяната на графика трябва да става със седемдневно предизвестие!");

            if (!await _scheduleService.DoesScheduleExistsById(scheduleDto.ScheduleId)) return BadRequest("Тази задача от графика не съществува");
            if (await _scheduleService.IsAssignmentForWork(scheduleDto.ScheduleId)) return BadRequest("Не може да променяте графика за работен ден! Моля свържете се с мениджъра Ви!");

            /* Deleting the old assignment temporarily but saving its data first */
            CreateScheduleDto oldSchedule = await _scheduleService.CreateScheduleDto(scheduleDto.ScheduleId);
            if (!await _scheduleService.DeleteAssignment(scheduleDto.ScheduleId)) return BadRequest("Неуспешно изтрита задача! Моля опитайте отново!");
            await _scheduleService.SaveChangesAsync();

            Employee employee;
            if (!await _employeeService.CheckEmployeeExistByEmail(scheduleDto.EmployeeEmail)) return BadRequest("Потребителят не съществува");
            else employee = await _employeeService.GetEmployeeByEmail(scheduleDto.EmployeeEmail);

            Restaurant restaurant;
            if (!await _restaurantService.CheckRestaurantExistById(scheduleDto.RestaurantId)) return BadRequest("Ресторантът не съществува!");
            else restaurant = await _restaurantService.GetRestaurantById(scheduleDto.RestaurantId);
            if (!restaurant.IsWorking) return BadRequest("Ресторантът не работи!");

            /* Checking if the changed assignment can fit in the schedule */
            if (await _scheduleService.CanEmployeeTakeVacationOn(employee, restaurant, scheduleDto.Day, scheduleDto.From, scheduleDto.To))
            {
                await _scheduleService.AddAssignmentToSchedule(scheduleDto);
                await _scheduleService.SaveChangesAsync();
                return await GetDailySchedule(scheduleDto.EmployeeEmail, scheduleDto.Day);
            }
            else
            {
                /* Adding the old assignment back because the updated one did not fit */
                await _scheduleService.AddAssignmentToSchedule(oldSchedule);
                await _scheduleService.SaveChangesAsync();
                return BadRequest("Вече имате запазен друг ангажимент и не можете да промените графика си!");
            }
        }

        [HttpDelete("api/employee/schedule/delete-assignment/{scheduleId}")]
        public async Task<IActionResult> DeleteAssignment(string scheduleId)
        {
            if (!await _scheduleService.DoesScheduleExistsById(scheduleId)) return BadRequest("Тази задача от графика не съществува");
            if (await _scheduleService.IsAssignmentForWork(scheduleId)) return BadRequest("Не може да променяте графика за работен ден! Моля свържете се с мениджъра Ви!");
            if (await _scheduleService.DeleteAssignment(scheduleId))
            {
                await _scheduleService.SaveChangesAsync();
                return Ok(new JsonResult(new { title = "Успешно изтрита задача!", message = "Успешно изтрихте задачата от графика си!" }));
            }
            return BadRequest("Неуспешно изтрита задача! Моля опитайте отново!");
        }

        [HttpGet("api/employee/regen-qrcode/{email}")]
        public async Task<IActionResult> RegenerateQrCode(string email)
        {
            Employee employee;
            if (!await _employeeService.CheckEmployeeExistByEmail(email)) return BadRequest("Потребителят не съществува");
            else employee = await _employeeService.GetEmployeeByEmail(email);

            if (await _employeeService.UpdateQrCode(employee))
            {
                return Ok(new JsonResult(new { title = "Успешно обновен QR код!", message = "Вие успешно обновихте QR кода си!" }));
            }
            else
            {
                return BadRequest("Неуспешно обновяване на QR кода!");
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
