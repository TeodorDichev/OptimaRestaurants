using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using webapi.Data;
using webapi.DTOs.Employee;
using webapi.DTOs.Restaurant;
using webapi.Models;
using webapi.Services;

namespace webapi.Controllers
{
    /// <summary>
    /// This class manages all employee related functions
    /// Edit - city, birthdate
    /// Add/Delete a request to a manager's restaurant
    /// </summary>
    public class EmployeeController : Controller
    {
        private readonly OptimaRestaurantContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly PictureAndIconService _pictureService;

        public EmployeeController(OptimaRestaurantContext context,
            UserManager<ApplicationUser> userManager,
            PictureAndIconService pictureService)
        {
            _context = context;
            _userManager = userManager;
            _pictureService = pictureService;
        }

        [HttpGet("api/employee/get-employee/{email}")]
        public async Task<ActionResult<EmployeeMainViewDto>> GetEmployee(string email)
        {
            if (await _context.Employees.FirstOrDefaultAsync(e => e.Profile.Email == email) == null
                || await _userManager.FindByEmailAsync(email) == null) return BadRequest("Потребителят не съществува!");

            return GenerateNewEmployeeDto(email);
        }

        [HttpPut("api/employee/update-employee/{email}")]
        public async Task<ActionResult<EmployeeMainViewDto>> UpdateEmployeeAccount([FromBody] UpdateEmployeeDto employeeDto, string email)
        {
            var profile = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
            var employee = await _context.Employees.FirstOrDefaultAsync(m => m.Profile.Id == profile.Id);

            if (employee == null || profile == null) return BadRequest("Потребителят не е намерен!");

            if (!employeeDto.NewFirstName.IsNullOrEmpty()) profile.FirstName = employeeDto.NewFirstName;
            if (!employeeDto.NewLastName.IsNullOrEmpty()) profile.LastName = employeeDto.NewLastName;
            if (!employeeDto.NewPhoneNumber.IsNullOrEmpty()) profile.PhoneNumber = employeeDto.NewPhoneNumber;
            if (employeeDto.ProfilePictureFile != null) await _pictureService.UploadProfilePictureAsync(employeeDto.ProfilePictureFile, email);

            _context.Update(employee);
            await _context.SaveChangesAsync();

            return GenerateNewEmployeeDto(profile.Email);
        }

        [HttpDelete("api/employee/delete-employee{email}")]
        public async Task<IActionResult> DeleteEmployeeAccount(string email)
        {
            var profile = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
            var employee = await _context.Employees.FirstOrDefaultAsync(e => e.Profile.Email == email);

            if (employee == null || profile == null) return BadRequest("Потребителят не съществува!");
            var roles = await _userManager.GetRolesAsync(profile);

            foreach (var er in employee.EmployeesRestaurants) _context.EmployeesRestaurants.Remove(er);

            _context.Employees.Remove(employee);
            await _userManager.RemoveFromRolesAsync(profile, roles);
            await _userManager.DeleteAsync(profile);
            await _context.SaveChangesAsync();

            return Ok(new JsonResult(new { title = "Успешно изтриване!", message = "Успешно изтрихте своя акаунт!" }));
        }

        private EmployeeMainViewDto GenerateNewEmployeeDto(string email)
        {
            var employee = _context.Employees.FirstOrDefault(e => e.Profile.Email == email);
            var profile = _context.Users.FirstOrDefault(u => u.Email == email);

            if (employee == null || profile == null) throw new ArgumentNullException("Потребителят не съществува!");

            ICollection<RestaurantDto> restaurants = new List<RestaurantDto>();

            foreach (var restaurant in employee.EmployeesRestaurants
                .Where(er => !er.EndedOn.HasValue)
                .Select(er => er.Restaurant))
            {
                restaurants.Add(new RestaurantDto
                {
                    Id = restaurant.Id.ToString(),
                    Name = restaurant.Name,
                    Address = restaurant.Address,
                    City = restaurant.City,
                    AtmosphereAverageRating = restaurant?.CuisineAverageRating ?? -1,
                    CuisineAverageRating = restaurant?.CuisineAverageRating ?? -1,
                    EmployeesAverageRating = restaurant?.EmployeesAverageRating ?? -1,
                    IconData = restaurant?.IconData
                });
            }

            var employeeMainViewDto = new EmployeeMainViewDto
            {
                Email = email,
                FirstName = employee.Profile.FirstName,
                LastName = employee.Profile.LastName,
                ProfilePictureData = employee.Profile.ProfilePictureData,
                PhoneNumber = employee.Profile?.PhoneNumber ?? string.Empty,
                City = employee?.City ?? string.Empty,
                BirthDate = employee?.BirthDate ?? DateTime.Now,
                AttitudeAverageRating = employee?.AttitudeAverageRating ?? -1,
                CollegialityAverageRating = employee?.CollegialityAverageRating ?? -1,
                SpeedAverageRating = employee?.SpeedAverageRating ?? -1,
                PunctualityAverageRating = employee?.PunctualityAverageRating ?? -1,
                EmployeeAverageRating = employee?.EmployeeAverageRating ?? -1,
                Restaurants = restaurants
            };

            return employeeMainViewDto;
        }
    }
}
