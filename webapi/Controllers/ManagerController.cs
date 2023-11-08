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

        [HttpDelete("api/manager/{email}")]
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

            return Ok("Успешно изтрихте своя акаунт!");
        }

        [HttpGet("{restaurantId}")] // pass the restaurant id to show the employees there
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

        [HttpGet("api/manager/{email}")] // pass either email from register or username from login
        public async Task<IActionResult> GetManager(string email)
        {
            if (_context.Managers.FirstOrDefault(e => e.Profile.Email == email) == null) return BadRequest("No manager with such email");

            return Ok(GenerateNewManagerDto(email));
        }

        [HttpPost("api/manager/{email}")]
        public async Task<ActionResult<ManagerMainViewDto>> AddNewRestaurant([FromBody] NewRestaurantDto newRestaurant, string managerEmail)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var manager = await _context.Managers
                .FirstOrDefaultAsync(e => e.Profile.Email == managerEmail);

            var managerProfile = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == managerEmail); // for some reason this loads manager.Profile

            if (manager == null) return BadRequest("No manager with such email");

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

            return GenerateNewManagerDto(managerEmail);
        }

        private ManagerMainViewDto GenerateNewManagerDto(string email)
        {
            var manager = _context.Managers
                .FirstOrDefault(e => e.Profile.Email == email);

            var managerProfile = _context.Users
                .FirstOrDefault(u => u.Email == email); // for some reason this loads manager.Profile

            ICollection<RestaurantDto> restaurants = new List<RestaurantDto>();

            foreach (var restaurant in manager.Restaurants)
            {
                restaurants.Add(new RestaurantDto
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
                Restaurants = restaurants.IsNullOrEmpty() ? restaurants : new List<RestaurantDto>()
            };

            return managerMainViewDto;
        }
    }
}
