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
using webapi.Services;

namespace webapi.Controllers
{
    public class ManagerController : Controller
    {
        private readonly OptimaRestaurantContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly PicturesAndIconsService _picturesAndIconsService;

        public ManagerController(OptimaRestaurantContext context,
                UserManager<ApplicationUser> userManager,
                PicturesAndIconsService picturesAndIconsService)
        {
            _context = context;
            _userManager = userManager;
            _picturesAndIconsService = picturesAndIconsService;
        }

        [HttpGet("api/manager/get-manager/{email}")]
        public async Task<IActionResult> GetManager(string email)
        {
            if (await _context.Managers.FirstOrDefaultAsync(e => e.Profile.Email == email) == null
                || await _userManager.FindByEmailAsync(email) == null) return BadRequest("Потребителят не съществува!");

            return Ok(GenerateNewManagerDto(email));
        }

        [HttpPut("api/manager/update-manager/{email}")]
        public async Task<ActionResult<ManagerMainViewDto>> UpdateManager([FromForm] UpdateManagerDto managerDto, string email)
        {
            var profile = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
            if (profile == null) return BadRequest("Потребителят не е намерен!");

            var manager = await _context.Managers.FirstOrDefaultAsync(m => m.Profile.Id == profile.Id);
            if (manager == null) return BadRequest("Потребителят не е намерен!");

            if (managerDto.NewFirstName != null) profile.FirstName = managerDto.NewFirstName;
            if (managerDto.NewLastName != null) profile.LastName = managerDto.NewLastName;
            if (managerDto.NewPhoneNumber != null) profile.PhoneNumber = managerDto.NewPhoneNumber;
            if (managerDto.ProfilePictureFile != null)
            {
                if(profile.ProfilePicturePath == null) profile.ProfilePicturePath = _picturesAndIconsService.SaveImage(managerDto.ProfilePictureFile);
                else
                {
                    _picturesAndIconsService.DeleteImage(profile.ProfilePicturePath);
                    profile.ProfilePicturePath = _picturesAndIconsService.SaveImage(managerDto.ProfilePictureFile);
                }
            }

            _context.Update(manager);
            await _context.SaveChangesAsync();

            return GenerateNewManagerDto(profile.Email ?? string.Empty);
        }

        [HttpDelete("api/manager/delete-manager/{email}")]
        public async Task<IActionResult> DeleteManager(string email)
        {
            var profile = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
            var manager = await _context.Managers.FirstOrDefaultAsync(e => e.Profile.Email == email);

            if (manager == null || profile == null) return BadRequest("Потребителят не съществува!");
            var roles = await _userManager.GetRolesAsync(profile);

            foreach (var restaurant in manager.Restaurants) restaurant.Manager = null;

            if (profile.ProfilePicturePath != null) _picturesAndIconsService.DeleteImage(profile.ProfilePicturePath);
            foreach (var r in _context.Requests.Where(r => r.Sender.Email == email || r.Receiver.Email == email)) _context.Requests.Remove(r);

            _context.Managers.Remove(manager);
            await _userManager.RemoveFromRolesAsync(profile, roles);
            await _userManager.DeleteAsync(profile);
            await _context.SaveChangesAsync();

            return Ok(new JsonResult(new { title = "Успешно изтриване!", message = "Успешно изтрихте своя акаунт!" }));
        }

        [HttpPost("api/manager/add-new-restaurant/{email}")]
        public async Task<ActionResult<ManagerMainViewDto>> AddNewRestaurant([FromForm] NewRestaurantDto newRestaurant, string email)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var profile = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
            var manager = await _context.Managers.FirstOrDefaultAsync(e => e.Profile.Email == email);

            if (manager == null || profile == null) return BadRequest("Потребителят не съществува!");

            Restaurant restaurant = new Restaurant
            {
                Name = newRestaurant.Name,
                Address = newRestaurant.Address,
                City = newRestaurant.City,
                IsWorking = true,
                EmployeeCapacity = newRestaurant.EmployeeCapacity,
                Manager = manager
            };

            if (newRestaurant.IconFile != null) restaurant.IconPath = _picturesAndIconsService.SaveImage(newRestaurant.IconFile);

            await _context.Restaurants.AddAsync(restaurant);
            await _context.SaveChangesAsync();

            return GenerateNewManagerDto(email);
        }

        [HttpPut("api/manager/update-restaurant/{restaurantId}")]
        public async Task<ActionResult<ManagerMainViewDto>> UpdateRestaurant([FromForm] UpdateRestaurantDto restaurantDto, string restaurantId)
        {
            var restaurant = await _context.Restaurants.FirstOrDefaultAsync(r => r.Id.ToString() == restaurantId);
            if (restaurant == null) return BadRequest("Ресторантът не съществува!");

            var manager = restaurant.Manager;
            if (manager == null) return BadRequest("Потребителят не съществува!");

            var profile = manager.Profile;
            if (profile == null) return BadRequest("Потребителят не съществува!");

            if (restaurantDto.IsWorking.HasValue) restaurant.IsWorking = restaurantDto.IsWorking.Value;
            if (restaurantDto.Address != null) restaurant.Address = restaurantDto.Address;
            if (restaurantDto.City != null) restaurant.City = restaurantDto.City;
            if (restaurantDto.IconFile != null)
            {
                if (restaurant.IconPath == null) restaurant.IconPath = _picturesAndIconsService.SaveImage(restaurantDto.IconFile);
                else
                {
                    _picturesAndIconsService.DeleteImage(restaurant.IconPath);
                    restaurant.IconPath = _picturesAndIconsService.SaveImage(restaurantDto.IconFile);
                }
            }
            if (restaurantDto.Name != null) restaurant.Name = restaurantDto.Name;
            if (restaurantDto.EmployeeCapacity.HasValue) restaurant.EmployeeCapacity = (int)restaurantDto.EmployeeCapacity;

            _context.Update(restaurant);
            await _context.SaveChangesAsync();
            return GenerateNewManagerDto(profile.Email ?? string.Empty);
        }

        [HttpDelete("api/manager/delete-restaurant/{restaurantId}")]
        public async Task<ActionResult<ManagerMainViewDto>> DeleteRestaurant(string restaurantId)
        {
            var restaurant = await _context.Restaurants.FirstOrDefaultAsync(r => r.Id.ToString() == restaurantId);
            if (restaurant == null) return BadRequest("Ресторантът не съществува!");

            var manager = restaurant.Manager;
            if (manager == null) return BadRequest("Потребителят не съществува!");

            var profile = manager.Profile;
            if (profile == null) return BadRequest("Потребителят не съществува!");

            foreach (var er in restaurant.EmployeesRestaurants) er.EndedOn = DateTime.UtcNow;

            return GenerateNewManagerDto(profile.Email ?? string.Empty);
        }

        [HttpGet("api/manager/browse-employees/looking-for-job")]
        public async Task<ActionResult<List<BrowseEmployeeDto>>> GetEmployeesLookingForJob()
        {
            List<Employee> employees = await _context.Employees.ToListAsync();
            List<BrowseEmployeeDto> employeesDto = new List<BrowseEmployeeDto>();

            foreach (var employee in employees.Where(e => e.IsLookingForJob))
            {
                employeesDto.Add(new BrowseEmployeeDto
                {
                    Email = employee.Profile.Email ?? string.Empty,
                    FirstName = employee.Profile.FirstName ?? string.Empty,
                    LastName = employee.Profile.LastName ?? string.Empty,
                    PhoneNumber = employee.Profile.PhoneNumber ?? string.Empty,
                    ProfilePictureUrl = employee.Profile.ProfilePicturePath ?? string.Empty,
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
            List<EmployeeDto> employees = new List<EmployeeDto>();

            foreach (var emp in await _context.EmployeesRestaurants
                .Where(er => er.Restaurant.Id.ToString() == restaurantId)
                .Select(e => e.Employee).ToListAsync())
            {
                employees.Add(new EmployeeDto
                {
                    Email = emp.Profile.Email ?? string.Empty,
                    FirstName = emp.Profile.FirstName,
                    LastName = emp.Profile.LastName,
                    ProfilePictureUrl = emp.Profile.ProfilePicturePath,
                    EmployeeAverageRating = emp?.EmployeeAverageRating ?? -1
                });
            }

            return employees;
        }
        
        [HttpPut("api/manager/fire/{employeeEmail}/{restaurantId}")]
        public async Task<ActionResult<List<EmployeeDto>>> FireAnEmployee(string employeeEmail, string restaurantId)
        {
            var restaurant = await _context.Restaurants.FirstOrDefaultAsync(r => r.Id.ToString() == restaurantId);
            if (restaurant == null) return BadRequest("Ресторантът не съществува!");

            foreach (var er in restaurant.EmployeesRestaurants.Where(er => er.Employee.Profile.Email == employeeEmail))
            {
                er.EndedOn = DateTime.UtcNow;
                _context.EmployeesRestaurants.Update(er);
            }
            await _context.SaveChangesAsync();
            return await GetRestaurantEmployees(restaurantId);
        }

        [HttpGet("api/manager/get-all-requests/{email}")]
        public async Task<ActionResult<List<RequestDto>>> GetRequests(string email)
        {
            if (await _userManager.FindByEmailAsync(email) == null) { return BadRequest("Потребителят не съществува!"); }

            List<RequestDto> requests = new List<RequestDto>();
            foreach (var r in _context.Requests.Where(r => r.Receiver.Email == email).OrderBy(r => r.SentOn))
            {
                bool? confirmed = null;
                if (r.ConfirmedOn != null) confirmed = true;
                if (r.RejectedOn != null) confirmed = false;

                var request = new RequestDto
                {
                    Id = r.Id.ToString(),
                    RestaurantId = r.Restaurant.Id.ToString(),
                    SenderEmail = r.Sender.Email ?? string.Empty,
                    SentOn = r.SentOn,
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

        private ManagerMainViewDto GenerateNewManagerDto(string email)
        {
            var manager = _context.Managers.FirstOrDefault(e => e.Profile.Email == email);
            var profile = _context.Users.FirstOrDefault(u => u.Email == email);

            if (manager == null || profile == null) throw new ArgumentNullException("Потребителят не съществува!");

            List<Restaurant> restaurants = _context.Restaurants.Where(r => r.Manager == manager).ToList();
            ICollection<AccountRestaurantDto> restaurantsDto = new List<AccountRestaurantDto>();

            foreach (var restaurant in manager.Restaurants)
            {
                restaurantsDto.Add(new AccountRestaurantDto
                {
                    Id = restaurant.Id.ToString(),
                    Name = restaurant.Name,
                    Address = restaurant.Address,
                    City = restaurant.City,
                    AtmosphereAverageRating = restaurant?.CuisineAverageRating ?? 0,
                    CuisineAverageRating = restaurant?.CuisineAverageRating ?? 0,
                    EmployeesAverageRating = restaurant?.EmployeesAverageRating ?? 0,
                    RestaurantAverageRating = restaurant?.RestaurantAverageRating ?? 0,
                    IconUrl = restaurant?.IconPath,
                });
            }

            var managerMainViewDto = new ManagerMainViewDto
            {
                Email = email,
                FirstName = manager.Profile.FirstName,
                LastName = manager.Profile.LastName,
                ProfilePictureUrl = manager.Profile.ProfilePicturePath,
                Restaurants = restaurants.IsNullOrEmpty() ? new List<AccountRestaurantDto>() : restaurantsDto
            };

            return managerMainViewDto;
        }
    }
}
