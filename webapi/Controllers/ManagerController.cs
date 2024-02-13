using Microsoft.AspNetCore.Mvc;
using webapi.DTOs.Employee;
using webapi.DTOs.Manager;
using webapi.DTOs.Request;
using webapi.DTOs.Restaurant;
using webapi.DTOs.Schedule;
using webapi.Migrations;
using webapi.Models;
using webapi.Services.ClassServices;
using webapi.Services.ModelServices;

namespace webapi.Controllers
{
    /// <summary>
    /// ManagerController manages managers:
    /// CRUD operations for their profiles,
    /// CRUD operations for their restaurants,
    /// Allows them to interact with their employees through requests,
    /// </summary>

    public class ManagerController : Controller
    {
        private readonly ManagerService _managerService;
        private readonly EmployeeService _employeeService;
        private readonly RestaurantService _restaurantService;
        private readonly RequestService _requestService;
        private readonly ScheduleService _scheduleService;

        public ManagerController(EmployeeService employeeService,
                ManagerService managerService,
                RequestService requestService,
                RestaurantService restaurantService,
                ScheduleService scheduleService)
        {
            _managerService = managerService;
            _restaurantService = restaurantService;
            _requestService = requestService;
            _employeeService = employeeService;
            _scheduleService = scheduleService;
        }

        [HttpGet("api/manager/get-manager/{email}")]
        public async Task<ActionResult<ManagerMainViewDto>> GetManager(string email, int lastPageIndex)
        {
            if (await _managerService.CheckManagerExistByEmail(email)) return await GenerateNewManagerDto(email, lastPageIndex);
            else return BadRequest("Потребителят не съществува!");
        }

        [HttpPut("api/manager/update-manager/{email}")]
        public async Task<ActionResult<ManagerMainViewDto>> UpdateManager([FromForm] UpdateManagerDto managerDto, string email)
        {
            Manager manager;
            if (!await _managerService.CheckManagerExistByEmail(email)) return BadRequest("Потребителят не съществува");
            else manager = await _managerService.GetManagerByEmail(email);

            await _managerService.UpdateManager(manager, managerDto);
            await _managerService.SaveChangesAsync();

            return await GenerateNewManagerDto(email, 1);
        }

        [HttpDelete("api/manager/delete-manager/{email}")]
        public async Task<IActionResult> DeleteManager(string email)
        {
            Manager manager;
            if (!await _managerService.CheckManagerExistByEmail(email)) return BadRequest("Потребителят не съществува");
            else manager = await _managerService.GetManagerByEmail(email);

            if (await _managerService.DeleteManager(manager))
            {
                await _managerService.SaveChangesAsync();
                return Ok(new JsonResult(new { title = "Успешно изтриване!", message = "Успешно изтрихте своя акаунт!" }));
            }
            else return BadRequest("Неуспешно изтриване!");
        }

        [HttpPost("api/manager/add-new-restaurant/{email}")]
        public async Task<ActionResult<ManagerMainViewDto>> AddNewRestaurant([FromForm] NewRestaurantDto newRestaurant, string email)
        {
            Manager manager;
            if (!await _managerService.CheckManagerExistByEmail(email)) return BadRequest("Потребителят не съществува");
            else manager = await _managerService.GetManagerByEmail(email);

            await _restaurantService.AddRestaurant(newRestaurant, manager);
            await _restaurantService.SaveChangesAsync();

            return await GenerateNewManagerDto(email, 1);
        }

        [HttpPut("api/manager/update-restaurant/{restaurantId}")]
        public async Task<ActionResult<ManagerMainViewDto>> UpdateRestaurant([FromForm] UpdateRestaurantDto restaurantDto, string restaurantId)
        {
            Restaurant restaurant;
            if (!await _restaurantService.CheckRestaurantExistById(restaurantId)) return BadRequest("Ресторантът не съществува!");
            else restaurant = await _restaurantService.GetRestaurantById(restaurantId);

            string managerEmail;
            if (restaurant.Manager == null || restaurant.Manager.Profile.Email == null) return BadRequest("Ресторантът няма управител!");
            else managerEmail = restaurant.Manager.Profile.Email;

            _restaurantService.UpdateRestaurant(restaurant, restaurantDto);
            await _restaurantService.SaveChangesAsync();

            return await GenerateNewManagerDto(managerEmail, 1);
        }

        [HttpDelete("api/manager/delete-restaurant/{restaurantId}")]
        public async Task<ActionResult<ManagerMainViewDto>> DeleteRestaurant(string restaurantId)
        {
            Restaurant restaurant;
            if (!await _restaurantService.CheckRestaurantExistById(restaurantId)) return BadRequest("Ресторантът не съществува!");
            else restaurant = await _restaurantService.GetRestaurantById(restaurantId);

            string managerEmail;
            if (restaurant.Manager == null || restaurant.Manager.Profile.Email == null) return BadRequest("Ресторантът няма управител!");
            else managerEmail = restaurant.Manager.Profile.Email;

            _restaurantService.DeleteRestaurant(restaurant);
            await _restaurantService.SaveChangesAsync();

            return await GenerateNewManagerDto(managerEmail, 1);
        }

        [HttpGet("api/manager/browse-employees/looking-for-job")]
        public ActionResult<List<BrowseEmployeeDto>> GetEmployeesLookingForJob(int lastPageIndex)
        {
            return _employeeService.GetEmployeesLookingForJob(lastPageIndex);
        }

        [HttpGet("api/manager/get-restaurant-employees/{restaurantId}")]
        public async Task<ActionResult<List<EmployeeDto>>> GetRestaurantEmployees(string restaurantId)
        {
            Restaurant restaurant;
            if (!await _restaurantService.CheckRestaurantExistById(restaurantId)) return BadRequest("Ресторантът не съществува!");
            else restaurant = await _restaurantService.GetRestaurantById(restaurantId);

            List<EmployeeDto> employees = new List<EmployeeDto>();

            foreach (var emp in _restaurantService.GetEmployeesOfRestaurant(restaurant))
            {
                employees.Add(new EmployeeDto
                {
                    Email = emp.Profile.Email ?? string.Empty,
                    FirstName = emp.Profile.FirstName,
                    LastName = emp.Profile.LastName,
                    ProfilePicturePath = emp.Profile.ProfilePicturePath,
                    EmployeeAverageRating = emp?.EmployeeAverageRating ?? 0
                });
            }

            return employees;
        }

        [HttpPut("api/manager/fire/{employeeEmail}/{restaurantId}")]
        public async Task<ActionResult<List<EmployeeDto>>> FireAnEmployee(string employeeEmail, string restaurantId)
        {
            Restaurant restaurant;
            if (!await _restaurantService.CheckRestaurantExistById(restaurantId)) return BadRequest("Ресторантът не съществува!");
            else restaurant = await _restaurantService.GetRestaurantById(restaurantId);

            Employee employee;
            if (!await _employeeService.CheckEmployeeExistByEmail(employeeEmail)) return BadRequest("Потребителят не съществува");
            else employee = await _employeeService.GetEmployeeByEmail(employeeEmail);

            _restaurantService.FireAnEmployee(restaurant, employee);
            await _restaurantService.SaveChangesAsync();
            return await GetRestaurantEmployees(restaurantId);
        }

        [HttpGet("api/manager/get-all-requests/{email}")]
        public async Task<ActionResult<List<RequestDto>>> GetRequests(string email)
        {
            if (!await _managerService.CheckManagerExistByEmail(email)) return BadRequest("Потребителят не съществува");

            return _requestService.GetManagerRequests(email);
        }

        [HttpPost("api/manager/send-working-request")]
        public async Task<IActionResult> SendWorkingRequest([FromBody] NewEmployeeRequestDto requestDto)
        {
            Employee employee;
            if (!await _employeeService.CheckEmployeeExistByEmail(requestDto.EmployeeEmail)) return BadRequest("Потребителят не съществува");
            else employee = await _employeeService.GetEmployeeByEmail(requestDto.EmployeeEmail);

            Restaurant restaurant;
            if (!await _restaurantService.CheckRestaurantExistById(requestDto.RestaurantId)) return BadRequest("Ресторантът не съществува!");
            else restaurant = await _restaurantService.GetRestaurantById(requestDto.RestaurantId);
            if (!restaurant.IsWorking) return BadRequest("Ресторантът не работи!");
            if (_restaurantService.IsRestaurantAtMaxCapacity(restaurant)) return BadRequest("Ресторантът не наема повече работници!");
            if (_restaurantService.HasRestaurantAManager(restaurant)) return BadRequest("Ресторантът няма мениджър!");

            if (await _requestService.IsRequestAlreadySent(employee.Profile, restaurant)) return BadRequest("Вие вече сте изпратили заявка към този потребител!");
            if (_requestService.IsEmployeeAlreadyWorkingInRestaurant(employee, restaurant)) return BadRequest("Потребителят вече работи в този ресторант!");

            await _requestService.AddRequest(employee, restaurant, false);
            await _restaurantService.SaveChangesAsync();

            return Ok(new JsonResult(new { title = "Успешно изпратена заявка!", message = $"Вашата заявка беше изпратена!" }));
        }

        [HttpPost("api/manager/respond-to-request")]
        public async Task<IActionResult> RespondToRequest([FromBody] ResponceToRequestDto requestDto)
        {
            Request request;
            if (!await _requestService.CheckRequestExistById(requestDto.RequestId)) return BadRequest("Заявката не съществува");
            else request = await _requestService.GetRequestById(requestDto.RequestId);
            if (request.ConfirmedOn != null || request.RejectedOn != null) return BadRequest("Заявката вече е отговорена!");

            Employee employee;
            if (!await _employeeService.CheckEmployeeExistByEmail(request.Sender.Email ?? string.Empty)) return BadRequest("Потребителят не съществува");
            else employee = await _employeeService.GetEmployeeByEmail(request.Sender.Email ?? string.Empty);

            Manager manager;
            if (!await _managerService.CheckManagerExistByEmail(request.Receiver.Email ?? string.Empty)) return BadRequest("Потребителят изпратил заявката не съществува!");
            else manager = await _managerService.GetManagerByEmail(request.Receiver.Email ?? string.Empty);

            Restaurant restaurant;
            if (!await _restaurantService.CheckRestaurantExistById(request.Restaurant.Id.ToString())) return BadRequest("Ресторантът не съществува!");
            else restaurant = await _restaurantService.GetRestaurantById(request.Restaurant.Id.ToString());
            if (!restaurant.IsWorking) return BadRequest("Ресторантът не работи!");
            if (_restaurantService.IsRestaurantAtMaxCapacity(restaurant)) return BadRequest("Ресторантът не наема повече работници");

            if (await _requestService.RespondToRequest(employee, restaurant, request, requestDto.Confirmed))
            {
                await _requestService.SaveChangesAsync();
                return Ok(new JsonResult(new { title = "Успешно потвърдена заявка!", message = $"{employee.Profile.FirstName} вече работи за вас!" }));
            }
            else
            {
                await _requestService.SaveChangesAsync();
                return Ok(new JsonResult(new { title = "Успешно отхвърлена заявка!", message = $"Заявката на {employee.Profile.FirstName} е отхвърлена!" }));
            }
        }

        [HttpPost("api/manager/schedule/add-assignment")]
        public async Task<ActionResult<List<ManagerDailyScheduleDto>>> AddAssignment([FromBody] ScheduleDto scheduleDto)
        {
            Employee employee;
            if (!await _employeeService.CheckEmployeeExistByEmail(scheduleDto.EmployeeEmail)) return BadRequest("Потребителят не съществува");
            else employee = await _employeeService.GetEmployeeByEmail(scheduleDto.EmployeeEmail);

            Restaurant restaurant;
            if (!await _restaurantService.CheckRestaurantExistById(scheduleDto.RestaurantId)) return BadRequest("Ресторантът не съществува!");
            else restaurant = await _restaurantService.GetRestaurantById(scheduleDto.RestaurantId);
            if (!restaurant.IsWorking) return BadRequest("Ресторантът не работи!");

            /* Manager can add both working and leisure days */

            if (await _scheduleService.IsEmployeeFreeToWork(employee, scheduleDto.Day, scheduleDto.From, scheduleDto.To))
            {
                await _scheduleService.AddAssignmentToSchedule(scheduleDto);
                await _scheduleService.SaveChangesAsync();
                return _scheduleService.GetManagerDailySchedule(restaurant, scheduleDto.Day);
            }
            else return BadRequest("Вече имате запазен друг ангажимент!");
        }

        [HttpPut("api/manager/schedule/edit-assignment")]
        public async Task<ActionResult<List<ManagerDailyScheduleDto>>> EditAssignment([FromBody] ScheduleDto scheduleDto)
        {
            if (!await _scheduleService.DoesScheduleExistsById(scheduleDto.ScheduleId)) return BadRequest("Тази задача от графика не съществува");
            if (!await _scheduleService.IsAssignmentForWork(scheduleDto.ScheduleId)) return BadRequest("Не може да променяте за почивен ден, защото той е бил добавен с вярно предизвестие!");

            /* Deleting the old assignment temporarily */
            if (!await _scheduleService.DeleteAssignment(scheduleDto.ScheduleId)) return BadRequest("Неуспешно изтрита задача! Моля опитайте отново!");
            await _scheduleService.SaveChangesAsync();

            Employee employee;
            if (!await _employeeService.CheckEmployeeExistByEmail(scheduleDto.EmployeeEmail)) return BadRequest("Потребителят не съществува");
            else employee = await _employeeService.GetEmployeeByEmail(scheduleDto.EmployeeEmail);

            Restaurant restaurant;
            if (!await _restaurantService.CheckRestaurantExistById(scheduleDto.RestaurantId)) return BadRequest("Ресторантът не съществува!");
            else restaurant = await _restaurantService.GetRestaurantById(scheduleDto.RestaurantId);
            if (!restaurant.IsWorking) return BadRequest("Ресторантът не работи!");

            /* Checking if it can fit in the schedule */
            if (await _scheduleService.IsEmployeeFreeToWork(employee, scheduleDto.Day, scheduleDto.From, scheduleDto.To))
            {
                await _scheduleService.EditScheduleAssignment(scheduleDto);
                await _scheduleService.SaveChangesAsync();

                return _scheduleService.GetManagerDailySchedule(restaurant, scheduleDto.Day);
            }
            else
            {
                /* Adding the old assignment back because the updated one did not fit */
                await _scheduleService.AddAssignmentToSchedule(scheduleDto);
                await _scheduleService.SaveChangesAsync();
                return BadRequest("Вече съществува запазен друг ангажимент и не можете да промените графика!");
            }
        }

        [HttpDelete("api/manager/schedule/delete-assignment/{scheduleId}")]
        public async Task<IActionResult> DeleteAssignment(string scheduleId)
        {
            if (!await _scheduleService.DoesScheduleExistsById(scheduleId)) return BadRequest("Тази задача от графика не съществува");
            if (!await _scheduleService.IsAssignmentForWork(scheduleId)) return BadRequest("Не може да променяте за почивен ден, защото той е бил добавен с вярно предизвестие!");
            if (await _scheduleService.DeleteAssignment(scheduleId))
            {
                await _scheduleService.SaveChangesAsync();
                return Ok(new JsonResult(new { title = "Успешно изтрита задача!", message = "Успешно изтрихте задачата от графика!" }));
            }
            return BadRequest("Неуспешно изтрита задача! Моля опитайте отново!");
        }

        [HttpGet("api/manager/schedule/get-free-employee/{restaurantId}/{day}")]
        public async Task<ActionResult<List<FreeEmployeeDto>>> GetFreeEmployees(string restaurantId, DateOnly day)
        {
            Restaurant restaurant;
            if (!await _restaurantService.CheckRestaurantExistById(restaurantId)) return BadRequest("Ресторантът не съществува!");
            else restaurant = await _restaurantService.GetRestaurantById(restaurantId);
            if (!restaurant.IsWorking) return BadRequest("Ресторантът не работи!");

            List<FreeEmployeeDto> freeEmployees = await _scheduleService.GetFreeEmployees(restaurant, day);
            if (freeEmployees == null) return BadRequest("Няма свободни работници!");
            return freeEmployees;
        }

        [HttpGet("api/manager/schedule/full-schedule/{restaurantId}/{month}")]
        public async Task<ActionResult<List<ManagerFullScheduleDto>>> GetManagerFullSchedule(string restaurantId, int month)
        {
            Restaurant restaurant;
            if (!await _restaurantService.CheckRestaurantExistById(restaurantId)) return BadRequest("Ресторантът не съществува!");
            else restaurant = await _restaurantService.GetRestaurantById(restaurantId);
            if (!restaurant.IsWorking) return BadRequest("Ресторантът не работи!");

            return await _scheduleService.GetManagerFullSchedule(restaurant, month);
        }

        [HttpGet("api/manager/schedule/get-daily-schedule/{restaurantId}/{day}")]
        public async Task<ActionResult<List<ManagerDailyScheduleDto>>> GetDailyWorkingSchedule(string restaurantId, DateOnly day)
        {
            Restaurant restaurant;
            if (!await _restaurantService.CheckRestaurantExistById(restaurantId)) return BadRequest("Ресторантът не съществува!");
            else restaurant = await _restaurantService.GetRestaurantById(restaurantId);
            if (!restaurant.IsWorking) return BadRequest("Ресторантът не работи!");

            return _scheduleService.GetManagerDailySchedule(restaurant, day);
        }

        private async Task<ManagerMainViewDto> GenerateNewManagerDto(string email, int lastPageIndex)
        {
            Manager manager = await _managerService.GetManagerByEmail(email);

            var managerMainViewDto = new ManagerMainViewDto
            {
                Email = email,
                FirstName = manager.Profile.FirstName,
                LastName = manager.Profile.LastName,
                PhoneNumber = manager.Profile.PhoneNumber ?? " ",
                ProfilePicturePath = manager.Profile.ProfilePicturePath,
                Restaurants = _restaurantService.GetRestaurantsOfManager(manager, lastPageIndex)
            };

            return managerMainViewDto;
        }
    }
}
