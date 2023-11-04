using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using webapi.Data;
using webapi.DTOs.Account;
using webapi.DTOs.Employee;
using webapi.DTOs.Manager;
using webapi.DTOs.Restaurant;
using webapi.Models;

namespace webapi.Controllers
{
    [Authorize(Roles = "Manager")]
    public class ManagerController : Controller
    {
        private readonly OptimaRestaurantContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public ManagerController(OptimaRestaurantContext context,
                RestaurantController restaurantController,
                AccountController accountController,
                UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpDelete("api/manager/{email}")]
        public async Task<IActionResult> DeleteManagerAccount(string email)
        {
            var manager = await _context.Managers.FirstOrDefaultAsync(e => e.Profile.Email == email);

            foreach (var restaurant in manager.Restaurants) restaurant.Manager = null;

            var user = manager.Profile;

            _context.Remove(user);
            _context.Remove(manager);
            await _context.SaveChangesAsync();

            return Ok("You have successfully deleted your account");
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
            var manager = await _context.Managers
            .FirstOrDefaultAsync(e => e.Profile.Email == email);

            ICollection<RestaurantDto> restaurants = new List<RestaurantDto>();

            foreach (var restaurant in _context.Restaurants
                .Where(r => r.Manager.Profile.Email.ToString() == email)
                .OrderBy(r => r.Name)
                .ToList())
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

            if (manager == null) return NotFound();

            var managerMainViewDto = new ManagerMainViewDto
            {
                Email = email,
                FirstName = manager.Profile.FirstName,
                LastName = manager.Profile.LastName,
                ProfilePictureUrl = manager.Profile?.ProfilePictureUrl ?? string.Empty,
                Restaurants = restaurants
            };

            return Ok(managerMainViewDto);
        }

        [HttpPost("api/manager/{email}")]
        public async Task<IActionResult> AddNewRestaurant([FromBody] NewRestaurantDto newRestaurant, string managerEmail)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var manager = await _context.Managers
                .FirstOrDefaultAsync(e => e.Profile.Email == managerEmail);

            if (manager == null) return NotFound();

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

            return Ok("You have successfully added a new restaurant");
        }
    }
}
