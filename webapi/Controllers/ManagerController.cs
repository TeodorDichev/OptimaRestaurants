using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using webapi.Data;
using webapi.DTOs.Employee;
using webapi.DTOs.Manager;
using webapi.DTOs.Restaurant;
using webapi.Models;
using webapi.Services;

namespace webapi.Controllers
{
    public class ManagerController : Controller
    {
        private readonly OptimaRestaurantContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly PictureAndIconService _pictureService;

        public ManagerController(OptimaRestaurantContext context,
                UserManager<ApplicationUser> userManager,
                PictureAndIconService pictureService)
        {
            _context = context;
            _userManager = userManager;
            _pictureService = pictureService;
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
            if (managerDto.ProfilePictureFile != null) await _pictureService.UploadProfilePictureAsync(managerDto.ProfilePictureFile, email);

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
            if (restaurantDto.IconFile != null) await _pictureService.UploadRestaurantIconAsync(restaurantDto.IconFile, restaurantId);
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
                    ProfilePictureData = emp.Profile.ProfilePictureData,
                    EmployeeAverageRating = emp?.EmployeeAverageRating ?? -1
                });
            }

            return employees;
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
                    IconData = restaurant?.IconData,
                });
            }

            var managerMainViewDto = new ManagerMainViewDto
            {
                Email = email,
                FirstName = manager.Profile.FirstName,
                LastName = manager.Profile.LastName,
                ProfilePictureData = manager.Profile.ProfilePictureData,
                Restaurants = restaurants.IsNullOrEmpty() ? new List<RestaurantDto>() : restaurantsDto
            };

            return managerMainViewDto;
        }
    }
}
