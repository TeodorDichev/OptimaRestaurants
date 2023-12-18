using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using webapi.Data;
using webapi.DTOs.Employee;
using webapi.DTOs.Manager;
using webapi.DTOs.Request;
using webapi.DTOs.Restaurant;
using webapi.Models;
using webapi.Services.ClassServices;
using webapi.Services.FileServices;

namespace webapi.Controllers
{
    public class ManagerController : Controller
    {
        private readonly OptimaRestaurantContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ManagerService _managerService;
        private readonly EmployeeService _employeeService;
        private readonly RestaurantService _restaurantService;

        public ManagerController(OptimaRestaurantContext context,
                UserManager<ApplicationUser> userManager,
                EmployeeService employeeService,
                ManagerService managerService, 
                RestaurantService restaurantService)
        {
            _context = context;
            _userManager = userManager;
            _managerService = managerService;
            _restaurantService = restaurantService;
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
            if (! await _managerService.CheckManagerExistByEmail(email)) return BadRequest("Потребителят не съществува");
            else manager = await _managerService.GetManagerByEmail(email);

            _managerService.UpdateManager(manager, managerDto);
            await _managerService.SaveChangesAsync();

            return await GenerateNewManagerDto(email);
        }

        [HttpDelete("api/manager/delete-manager/{email}")]
        public async Task<IActionResult> DeleteManager(string email)
        {
            Manager manager;
            if (! await _managerService.CheckManagerExistByEmail(email)) return BadRequest("Потребителят не съществува");
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
            if (! await _managerService.CheckManagerExistByEmail(email)) return BadRequest("Потребителят не съществува");
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
            List<BrowseEmployeeDto> employeesDto = new List<BrowseEmployeeDto>();

            foreach (var employee in _employeeService.GetEmployeesLookingForJob())
            {
                employeesDto.Add(new BrowseEmployeeDto
                {
                    Email = employee.Profile.Email ?? string.Empty,
                    FirstName = employee.Profile.FirstName ?? string.Empty,
                    LastName = employee.Profile.LastName ?? string.Empty,
                    PhoneNumber = employee.Profile.PhoneNumber ?? string.Empty,
                    ProfilePicturePath = employee.Profile.ProfilePicturePath ?? string.Empty,
                    EmployeeAverageRating = employee.EmployeeAverageRating ?? 0,
                    IsLookingForJob = employee.IsLookingForJob,
                    City = employee.City,
                    RestaurantsCount = employee.EmployeesRestaurants.Where(er => er.EndedOn == null).Count(),
                });
            }

            return employeesDto;
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
            if (await _userManager.FindByEmailAsync(email) == null) { return BadRequest("Потребителят не съществува!"); }

            List<RequestDto> requests = new List<RequestDto>();
            foreach (var r in _context.Requests.Where(r => r.Receiver.Email == email).OrderByDescending(r => r.SentOn))
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
                    Text = $"Работи ли {r.Sender.FirstName + " " + r.Sender.LastName} в {r.Restaurant.Name}?"
                };

                requests.Add(request);
            }

            return Ok(requests);
        }

        [HttpPost("api/manager/respond-to-request")]
        public async Task<IActionResult> RespondToRequest([FromBody] ResponceToRequestDto requestDto)
        {
            var request = _context.Requests.FirstOrDefault(r => r.Id.ToString() == requestDto.RequestId);
            if (request == null) return BadRequest("Заявката не съществува!");
            if (request.ConfirmedOn != null || request.RejectedOn != null) return BadRequest("Заявката вече е отговорена!");


            var managerProfile = request.Receiver;
            var manager = await _context.Managers.FirstOrDefaultAsync(m => m.Profile.Email == managerProfile.Email);
            if (managerProfile == null) return BadRequest("Потребителят не съществува!");

            var employeeProfile = request.Sender;
            var employee = await _context.Employees.FirstOrDefaultAsync(e => e.Profile.Email == employeeProfile.Email);
            if (employeeProfile == null || employee == null) return BadRequest("Потребителят изпратил заявката не съществува!");

            var restaurant = await _context.Restaurants.FirstOrDefaultAsync(r => r.Id.ToString() == requestDto.RestaurantId);
            if (restaurant == null) return BadRequest("Ресторантът не съществува!");
            if (!restaurant.IsWorking) return BadRequest("Ресторантът не работи!");

            if (restaurant.EmployeeCapacity <= _context.EmployeesRestaurants
                .Where(er => er.Restaurant.Id.ToString() == requestDto.RestaurantId).Count())
                    return BadRequest("Ресторантът не наема повече работници!");

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
                return Ok(new JsonResult(new { title = "Успешно потвърдена заявка!", message = $"{employee.Profile.FirstName} вече работи за вас!" }));
            }
            else
            {
                request.RejectedOn = DateTime.UtcNow;
                await _context.SaveChangesAsync();
                return Ok(new JsonResult(new { title = "Успешно отхвърлена заявка!", message = $"Заявката на {employee.Profile.FirstName} е отхвърлена!" }));
            }
        }

        private async Task<ManagerMainViewDto> GenerateNewManagerDto(string email)
        {
            Manager manager = await _managerService.GetManagerByEmail(email);

            List<Restaurant> restaurants = _restaurantService.GetRestaurantsOfManager(manager);
            ICollection<AccountRestaurantDto> restaurantsDto = new List<AccountRestaurantDto>();

            foreach (var restaurant in manager.Restaurants)
            {
                restaurantsDto.Add(new AccountRestaurantDto
                {
                    Id = restaurant.Id.ToString(),
                    Name = restaurant.Name,
                    Address = restaurant.Address,
                    City = restaurant.City,
                    EmployeeCapacity = restaurant.EmployeeCapacity,
                    AtmosphereAverageRating = restaurant?.CuisineAverageRating ?? 0,
                    CuisineAverageRating = restaurant?.CuisineAverageRating ?? 0,
                    EmployeesAverageRating = restaurant?.EmployeesAverageRating ?? 0,
                    RestaurantAverageRating = restaurant?.RestaurantAverageRating ?? 0,
                    IconPath = restaurant?.IconPath,
                });
            }

            var managerMainViewDto = new ManagerMainViewDto
            {
                Email = email,
                FirstName = manager.Profile.FirstName,
                LastName = manager.Profile.LastName,
                PhoneNumber = manager.Profile.PhoneNumber ?? " ",
                ProfilePicturePath = manager.Profile.ProfilePicturePath,
                Restaurants = restaurants.IsNullOrEmpty() ? new List<AccountRestaurantDto>() : restaurantsDto
            };

            return managerMainViewDto;
        }
    }
}
