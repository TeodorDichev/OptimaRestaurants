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

        public ManagerController(OptimaRestaurantContext context,
                UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpGet("api/manager/get-manager/{email}")]
        public async Task<IActionResult> GetManager(string email)
        {
            if (await _context.Managers.FirstOrDefaultAsync(e => e.Profile.Email == email) == null
                || await _userManager.FindByEmailAsync(email) == null) return BadRequest("Потребителят не съществува!");

            return Ok(GenerateNewManagerDto(email));
        }

        [HttpPut("api/manager/update-manager/{email}")]
        public async Task<ActionResult<ManagerMainViewDto>> UpdateManager([FromBody] UpdateManagerDto managerDto, string email)
        {
            var profile = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
            var manager = await _context.Managers.FirstOrDefaultAsync(m => m.Profile.Id == profile.Id);

            if (manager == null || profile == null) return BadRequest("Потребителят не е намерен!");

            if (!managerDto.NewFirstName.IsNullOrEmpty()) profile.FirstName = managerDto.NewFirstName;
            if (!managerDto.NewLastName.IsNullOrEmpty()) profile.LastName = managerDto.NewLastName;
            if (!managerDto.NewPhoneNumber.IsNullOrEmpty()) profile.PhoneNumber = managerDto.NewPhoneNumber;
            if (managerDto.ProfilePictureFile != null) //service doing some work

            _context.Update(manager);
            await _context.SaveChangesAsync();

            return GenerateNewManagerDto(profile.Email);
        }

        [HttpDelete("api/manager/delete-manager/{email}")]
        public async Task<IActionResult> DeleteManager(string email)
        {
            var profile = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
            var manager = await _context.Managers.FirstOrDefaultAsync(e => e.Profile.Email == email);

            if (manager == null || profile == null) return BadRequest("Потребителят не съществува!");
            var roles = await _userManager.GetRolesAsync(profile);

            foreach (var restaurant in manager.Restaurants) restaurant.Manager = null;

            _context.Managers.Remove(manager);
            await _userManager.RemoveFromRolesAsync(profile, roles);
            await _userManager.DeleteAsync(profile);
            await _context.SaveChangesAsync();

            return Ok(new JsonResult(new { title = "Успешно изтриване!", message = "Успешно изтрихте своя акаунт!" }));
        }

        [HttpPost("api/manager/add-new-restaurant/{email}")]
        public async Task<ActionResult<ManagerMainViewDto>> AddNewRestaurant([FromBody] NewRestaurantDto newRestaurant, string email)
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

            await _context.Restaurants.AddAsync(restaurant);
            await _context.SaveChangesAsync();

            return GenerateNewManagerDto(email);
        }

        [HttpPut("api/manager/update-restaurant/{restaurantId}")]
        public async Task<ActionResult<ManagerMainViewDto>> UpdateRestaurant([FromBody] UpdateRestaurantDto restaurantDto, string restaurantId)
        {
            var restaurant = await _context.Restaurants.FirstOrDefaultAsync(r => r.Id.ToString() == restaurantId);

            if (restaurant == null) return BadRequest("Ресторантът не съществува!");

            if (restaurantDto.IsWorking.HasValue) restaurant.IsWorking = restaurantDto.IsWorking.Value;
            if (!restaurantDto.Address.IsNullOrEmpty()) restaurant.Address = restaurantDto.Address;
            if (!restaurantDto.City.IsNullOrEmpty()) restaurant.City = restaurantDto.City;
            if (restaurantDto.IconFile != null) //service doing some work
            if (!restaurantDto.Name.IsNullOrEmpty()) restaurant.Name = restaurantDto.Name;
            if (restaurantDto.EmployeeCapacity.HasValue) restaurant.EmployeeCapacity = (int)restaurantDto.EmployeeCapacity;

            _context.Update(restaurant);
            await _context.SaveChangesAsync();
            return GenerateNewManagerDto(restaurant.Manager.Profile.Email);
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
                    Email = emp.Profile.Email,
                    FirstName = emp.Profile.FirstName,
                    LastName = emp.Profile.LastName,
                    ProfilePictureUrl = emp.Profile.ProfilePictureUrl,
                    EmployeeAverageRating = emp?.EmployeeAverageRating ?? -1
                });
            }

            return employees;
        }

        [HttpGet("api/manager/get-all-requests/{email}")]
        public async Task<ActionResult<List<RequestDto>>> GetRequests(string email)
        {
            var profile = await _userManager.FindByEmailAsync(email);
            if (profile == null) { return BadRequest("Потребителят не съществува!"); }

            List<RequestDto> requests = new List<RequestDto>();
            foreach (var r in profile.Requests)
            {
                bool? confirmed = null;
                if (r.ConfirmedOn != null) confirmed = true;
                if (r.RejectedOn != null) confirmed = false;

                var request = new RequestDto
                {
                    Id = r.Id.ToString(),
                    RestaurantName = r.Restaurant.Name,
                    SenderEmail = r.Sender.Email,
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
            var profile = await _userManager.FindByEmailAsync(requestDto.CurrentUserEmail);
            var manager = await _context.Managers.FirstOrDefaultAsync(m => m.Profile.Email == profile.Email);
            if (profile == null) { return BadRequest("Потребителят не съществува!"); }

            var request = profile.Requests.FirstOrDefault(r => r.Id.ToString() == requestDto.RequestId);
            if (request == null) return BadRequest("Заявката не съществува!");
            if (request.ConfirmedOn != null || request.RejectedOn != null) return BadRequest("Заявката вече е отговорена!");

            var senderProfile = request.Sender;
            var employee = await _context.Employees.FirstOrDefaultAsync(e => e.Profile.Email == senderProfile.Email);
            if (senderProfile == null || employee == null) return BadRequest("Потребителят изпратил заявката не съществува!");

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
            ICollection<RestaurantDto> restaurantsDto = new List<RestaurantDto>();

            foreach (var restaurant in manager.Restaurants)
            {
                restaurantsDto.Add(new RestaurantDto
                {
                    Id = restaurant.Id.ToString(),
                    Name = restaurant.Name,
                    Address = restaurant.Address,
                    City = restaurant.City,
                    AtmosphereAverageRating = restaurant?.CuisineAverageRating ?? -1, // in front-end if -1 "no reviews yet"
                    CuisineAverageRating = restaurant?.CuisineAverageRating ?? -1,
                    EmployeesAverageRating = restaurant?.EmployeesAverageRating ?? -1,
                    IconUrl = restaurant?.IconUrl,
                });
            }

            var managerMainViewDto = new ManagerMainViewDto
            {
                Email = email,
                FirstName = manager.Profile.FirstName,
                LastName = manager.Profile.LastName,
                ProfilePictureUrl = manager.Profile.ProfilePictureUrl,
                Restaurants = restaurants.IsNullOrEmpty() ? new List<RestaurantDto>() : restaurantsDto
            };

            return managerMainViewDto;
        }
    }
}
