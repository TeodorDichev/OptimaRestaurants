using Microsoft.AspNetCore.Mvc;
using webapi.DTOs.Employee;
using webapi.DTOs.Manager;
using webapi.DTOs.Request;
using webapi.DTOs.Restaurant;
using webapi.Models;
using webapi.Services.ClassServices;
using webapi.Services.ModelServices;

namespace webapi.Controllers
{
    public class ManagerController : Controller
    {
        private readonly ManagerService _managerService;
        private readonly EmployeeService _employeeService;
        private readonly RestaurantService _restaurantService;
        private readonly RequestService _requestService;

        public ManagerController(EmployeeService employeeService,
                ManagerService managerService,
                RequestService requestService,
                RestaurantService restaurantService)
        {
            _managerService = managerService;
            _restaurantService = restaurantService;
            _requestService = requestService;
            _employeeService = employeeService;
        }

        [HttpGet("api/manager/get-manager/{email}")]
        public async Task<ActionResult<ManagerMainViewDto>> GetManager(string email)
        {
            if (await _managerService.CheckManagerExistByEmail(email)) return await GenerateNewManagerDto(email);
            else return BadRequest("Потребителят не съществува!");
        }

        [HttpPut("api/manager/update-manager/{email}")]
        public async Task<ActionResult<ManagerMainViewDto>> UpdateManager([FromForm] UpdateManagerDto managerDto, string email)
        {
            Manager manager;
            if (!await _managerService.CheckManagerExistByEmail(email)) return BadRequest("Потребителят не съществува");
            else manager = await _managerService.GetManagerByEmail(email);

            _managerService.UpdateManager(manager, managerDto);
            await _managerService.SaveChangesAsync();

            return await GenerateNewManagerDto(email);
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

            return await GenerateNewManagerDto(email);
        }

        [HttpPut("api/manager/update-restaurant/{restaurantId}")]
        public async Task<ActionResult<ManagerMainViewDto>> UpdateRestaurant([FromForm] UpdateRestaurantDto restaurantDto, string restaurantId)
        {
            Restaurant restaurant;
            if (await _restaurantService.CheckRestaurantExistById(restaurantId)) return BadRequest("Ресторантът не съществува!");
            else restaurant = await _restaurantService.GetRestaurantById(restaurantId);

            string managerEmail;
            if (restaurant.Manager == null || restaurant.Manager.Profile.Email == null) return BadRequest("Ресторантът няма управител!");
            else managerEmail = restaurant.Manager.Profile.Email;

            _restaurantService.UpdateRestaurant(restaurant, restaurantDto);
            await _restaurantService.SaveChangesAsync();

            return await GenerateNewManagerDto(managerEmail);
        }

        [HttpDelete("api/manager/delete-restaurant/{restaurantId}")]
        public async Task<ActionResult<ManagerMainViewDto>> DeleteRestaurant(string restaurantId)
        {
            Restaurant restaurant;
            if (await _restaurantService.CheckRestaurantExistById(restaurantId)) return BadRequest("Ресторантът не съществува!");
            else restaurant = await _restaurantService.GetRestaurantById(restaurantId);

            string managerEmail;
            if (restaurant.Manager == null || restaurant.Manager.Profile.Email == null) return BadRequest("Ресторантът няма управител!");
            else managerEmail = restaurant.Manager.Profile.Email;

            _restaurantService.DeleteRestaurant(restaurant);
            await _restaurantService.SaveChangesAsync();

            return await GenerateNewManagerDto(managerEmail);
        }

        [HttpGet("api/manager/browse-employees/looking-for-job")]
        public ActionResult<List<BrowseEmployeeDto>> GetEmployeesLookingForJob()
        {
            return _employeeService.GetEmployeesLookingForJob();
        }

        [HttpGet("api/manager/get-restaurant-employees/{restaurantId}")]
        public async Task<ActionResult<List<EmployeeDto>>> GetRestaurantEmployees(string restaurantId)
        {
            Restaurant restaurant;
            if (await _restaurantService.CheckRestaurantExistById(restaurantId)) return BadRequest("Ресторантът не съществува!");
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
            if (await _restaurantService.CheckRestaurantExistById(restaurantId)) return BadRequest("Ресторантът не съществува!");
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
            if (!await _employeeService.CheckEmployeeExistByEmail(email)) return BadRequest("Потребителят не съществува");

            return _requestService.GetManagerRequests(email);
        }

        [HttpPost("api/manager/send-working-request")]
        public async Task<IActionResult> SendWorkingRequest([FromBody] NewEmployeeRequestDto requestDto)
        {
            Employee employee;
            if (!await _employeeService.CheckEmployeeExistByEmail(requestDto.EmployeeEmail)) return BadRequest("Потребителят не съществува");
            else employee = await _employeeService.GetEmployeeByEmail(requestDto.EmployeeEmail);

            Restaurant restaurant;
            if (await _restaurantService.CheckRestaurantExistById(requestDto.RestaurantId)) return BadRequest("Ресторантът не съществува!");
            else restaurant = await _restaurantService.GetRestaurantById(requestDto.RestaurantId);
            if (!restaurant.IsWorking) return BadRequest("Ресторантът не работи!");
            if (_restaurantService.IsRestaurantAtMaxCapacity(restaurant)) return BadRequest("Ресторантът не наема повече работници!");
            if (_restaurantService.HasRestaurantAManager(restaurant)) return BadRequest("Ресторантът няма мениджър!");

            if (await _requestService.IsRequestAlreadySent(restaurant.Manager.Profile, restaurant)) return BadRequest("Вие вече сте изпратили заявка към този потребител!");
            if (_requestService.IsEmployeeAlreadyWorkingInRestaurant(employee, restaurant)) return BadRequest("Потребителят вече работи в този ресторант!");

            await _requestService.AddRequest(employee, restaurant);
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
            if (await _restaurantService.CheckRestaurantExistById(request.Restaurant.Id.ToString())) return BadRequest("Ресторантът не съществува!");
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

        private async Task<ManagerMainViewDto> GenerateNewManagerDto(string email)
        {
            Manager manager = await _managerService.GetManagerByEmail(email);

            var managerMainViewDto = new ManagerMainViewDto
            {
                Email = email,
                FirstName = manager.Profile.FirstName,
                LastName = manager.Profile.LastName,
                PhoneNumber = manager.Profile.PhoneNumber ?? " ",
                ProfilePicturePath = manager.Profile.ProfilePicturePath,
                Restaurants = _restaurantService.GetRestaurantsOfManager(manager)
            };

            return managerMainViewDto;
        }
    }
}
