using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using webapi.Data;
using webapi.DTOs.Employee;
using webapi.DTOs.Manager;
using webapi.DTOs.Restaurant;
using webapi.Models;

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

        //confirm request, reject request -> sent requests when browsing employees looking for jobs

        [HttpPut("api/manager/update-restaurant/{restaurantId}")]
        public async Task<IActionResult> UpdateRestaurant([FromBody] UpdateRestaurantDto restaurantDto, string restaurantId)
        {
            var restaurant = await _context.Restaurants.FirstOrDefaultAsync(r => r.Id.ToString() == restaurantId);
            if (restaurant == null) return BadRequest("Ресторантът не съществува!");

            if (restaurantDto.IsWorking.HasValue) restaurant.IsWorking = restaurantDto.IsWorking.Value;
            if (!restaurantDto.Address.IsNullOrEmpty()) restaurant.Address = restaurantDto.Address;
            if (!restaurantDto.City.IsNullOrEmpty()) restaurant.City = restaurantDto.City;
            if (!restaurantDto.IconUrl.IsNullOrEmpty()) restaurant.IconUrl = restaurantDto.IconUrl;
            if (!restaurantDto.Name.IsNullOrEmpty()) restaurant.Name = restaurantDto.Name;
            if (restaurantDto.EmployeeCapacity.HasValue) restaurant.EmployeeCapacity = (int)restaurantDto.EmployeeCapacity;

            _context.Update(restaurant);
            await _context.SaveChangesAsync();
            return Ok(new JsonResult(new { title = "Успешно обновяване!", message = "Вашият ресторант беше успешно обновен!" }));
        }

        [HttpDelete("api/manager/delete-manager/{email}")]
        public async Task<IActionResult> DeleteManagerAccount(string email)
        {
            var manager = await _context.Managers
                .FirstOrDefaultAsync(e => e.Profile.Email == email);

            var managerProfile = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == email); // for some reason this loads manager.Profile

            foreach (var restaurant in manager.Restaurants) restaurant.Manager = null;

            var user = manager.Profile;

            _context.Remove(user);
            _context.Remove(manager);
            await _context.SaveChangesAsync();

            return Ok(new JsonResult(new { title = "Успешно изтриване!", message = "Успешно изтрихте своя акаунт!" }));
        }

        [HttpGet("api/manager/get-restaurant-employees/{restaurantId}")] // pass the restaurant id to show the employees there
        public async Task<IActionResult> GetRestaurantEmployees(string restaurantId)
        {
            ICollection<EmployeeDto> employees = new List<EmployeeDto>();

            foreach (var emp in _context.EmployeesRestaurants
                .Where(er => er.Restaurant.Id.ToString() == restaurantId)
                .Select(e => e.Employee))
            {
                employees.Add(new EmployeeDto
                {
                    Email = emp.Profile.Email,
                    FirstName = emp.Profile.FirstName,
                    LastName = emp.Profile.LastName,
                    ProfilePictureUrl = emp.Profile?.ProfilePictureUrl ?? string.Empty,
                    EmployeeAverageRating = emp?.EmployeeAverageRating ?? -1 // in front-end if -1 "no reviews yet"
                });
            }

            return Ok(employees);
        }

        [HttpGet("api/manager/get-manager/{email}")] // pass either email from register or username from login
        public async Task<IActionResult> GetManager(string email)
        {
            var manager = await _context.Managers
                .FirstOrDefaultAsync(e => e.Profile.Email == email);

            if (manager == null) return BadRequest("Няма регистриран менъджър с този имейл!");

            return Ok(GenerateNewManagerDto(email));
        }

        [HttpPost("api/manager/add-new-restaurant/{email}")]
        public async Task<ActionResult<ManagerMainViewDto>> AddNewRestaurant([FromBody] NewRestaurantDto newRestaurant, string email)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var manager = await _context.Managers
                .FirstOrDefaultAsync(e => e.Profile.Email == email);

            var managerProfile = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == email); // for some reason this loads manager.Profile

            if (manager == null) return BadRequest("Няма регистриран менъджър с този имейл!");

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

        private ManagerMainViewDto GenerateNewManagerDto(string email)
        {
            var manager = _context.Managers
                .FirstOrDefault(e => e.Profile.Email == email);

            List<Restaurant> restaurants = _context.Restaurants.Where(r => r.Manager == manager).ToList();

            var managerProfile = _context.Users
                .FirstOrDefault(u => u.Email == email); // for some reason this loads manager.Profile

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
                    IconUrl = restaurant?.IconUrl ?? string.Empty,
                });
            }

            var managerMainViewDto = new ManagerMainViewDto
            {
                Email = email,
                FirstName = manager.Profile.FirstName,
                LastName = manager.Profile.LastName,
                ProfilePictureUrl = manager.Profile?.ProfilePictureUrl ?? string.Empty,
                Restaurants = restaurants.IsNullOrEmpty() ? new List<RestaurantDto>() : restaurantsDto
            };

            return managerMainViewDto;
        }
    }
}
