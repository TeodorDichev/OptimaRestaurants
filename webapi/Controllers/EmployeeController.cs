using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using webapi.Data;
using webapi.DTOs.Employee;
using webapi.DTOs.Request;
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
        private readonly PicturesAndIconsService _picturesAndIconsService;

        public EmployeeController(OptimaRestaurantContext context,
            UserManager<ApplicationUser> userManager,
            PicturesAndIconsService picturesAndIconsService)
        {
            _context = context;
            _userManager = userManager;
            _picturesAndIconsService = picturesAndIconsService;
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
            if (employeeDto.ProfilePictureFile == null)
            {
                if (profile.ProfilePictureUrl == null) _picturesAndIconsService.SaveImage(employeeDto.ProfilePictureFile);
                else
                {
                    _picturesAndIconsService.DeleteImage(profile.ProfilePictureUrl);
                    _picturesAndIconsService.SaveImage(employeeDto.ProfilePictureFile);
                }
            }

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

            if (profile.ProfilePictureUrl != null) _picturesAndIconsService.DeleteImage(profile.ProfilePictureUrl);

            _context.Employees.Remove(employee);
            await _userManager.RemoveFromRolesAsync(profile, roles);
            await _userManager.DeleteAsync(profile);
            await _context.SaveChangesAsync();

            return Ok(new JsonResult(new { title = "Успешно изтриване!", message = "Успешно изтрихте своя акаунт!" }));
        }

        [HttpGet("api/employee/get-all-requests/{email}")]
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
                    Text = $"Работите ли в ресторантът {r.Restaurant.Name}, собственост на {r.Sender.FirstName + " " + r.Sender.LastName}?"
                };

                requests.Add(request);
            }

            return Ok(requests);
        }

        [HttpPost("api/employee/respond-to-request")]
        public async Task<IActionResult> RespondToRequest([FromBody] ResponceToRequestDto requestDto)
        {
            var profile = await _userManager.FindByEmailAsync(requestDto.CurrentUserEmail);
            var employee = await _context.Employees.FirstOrDefaultAsync(m => m.Profile.Email == profile.Email);
            if (profile == null) { return BadRequest("Потребителят не съществува!"); }

            var request = profile.Requests.FirstOrDefault(r => r.Id.ToString() == requestDto.RequestId);
            if (request == null) return BadRequest("Заявката не съществува!");
            if (request.ConfirmedOn != null || request.RejectedOn != null) return BadRequest("Заявката вече е отговорена!");

            var senderProfile = request.Sender;
            var manager = await _context.Managers.FirstOrDefaultAsync(e => e.Profile.Email == senderProfile.Email);
            if (senderProfile == null || manager == null) return BadRequest("Потребителят изпратил заявката не съществува!");

            var restaurant = await _context.Restaurants.FirstOrDefaultAsync(r => r.Id.ToString() == requestDto.RestaurantId);
            if (restaurant == null) return BadRequest("Ресторантът не съществува!");
            if (!restaurant.IsWorking) return BadRequest("Ресторантът не работи!");

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
                return Ok(new JsonResult(new { title = "Успешно потвърдена заявка!", message = $"Вече работите в ресторантът {request.Restaurant.Name}, собственост на {request.Sender.FirstName + " " + request.Sender.LastName}" }));
            }
            else
            {
                request.RejectedOn = DateTime.UtcNow;
                await _context.SaveChangesAsync();
                return Ok(new JsonResult(new { title = "Успешно отхвърлена заявка!", message = $"Заявката на {manager.Profile.FirstName} е отхвърлена!" }));
            }
        }

        private EmployeeMainViewDto GenerateNewEmployeeDto(string email)
        {
            var employee = _context.Employees.FirstOrDefault(e => e.Profile.Email == email);
            var profile = _context.Users.FirstOrDefault(u => u.Email == email);

            if (employee == null || profile == null) throw new ArgumentNullException("Потребителят не съществува!");

            ICollection<ManagerRestaurantDto> restaurants = new List<ManagerRestaurantDto>();

            foreach (var restaurant in employee.EmployeesRestaurants
                .Where(er => !er.EndedOn.HasValue)
                .Select(er => er.Restaurant))
            {
                restaurants.Add(new ManagerRestaurantDto
                {
                    Id = restaurant.Id.ToString(),
                    Name = restaurant.Name,
                    Address = restaurant.Address,
                    City = restaurant.City,
                    AtmosphereAverageRating = restaurant?.CuisineAverageRating ?? -1,
                    CuisineAverageRating = restaurant?.CuisineAverageRating ?? -1,
                    EmployeesAverageRating = restaurant?.EmployeesAverageRating ?? -1,
                    IconUrl = restaurant?.IconUrl
                });
            }

            var employeeMainViewDto = new EmployeeMainViewDto
            {
                Email = email,
                FirstName = employee.Profile.FirstName,
                LastName = employee.Profile.LastName,
                ProfilePictureUrl = employee.Profile.ProfilePictureUrl,
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
