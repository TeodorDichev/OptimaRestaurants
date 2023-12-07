using iText.IO.Image;
using Mailjet.Client.Resources;
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
        private readonly QrCodesService _qrCodesService;
        private readonly IConfiguration _configuration;

        public EmployeeController(OptimaRestaurantContext context,
            UserManager<ApplicationUser> userManager,
            PicturesAndIconsService picturesAndIconsService,
            QrCodesService qrCodesService,
            IConfiguration configuration)
        {
            _context = context;
            _userManager = userManager;
            _picturesAndIconsService = picturesAndIconsService;
            _qrCodesService = qrCodesService;
            _configuration = configuration;
        }

        [HttpGet("api/employee/get-employee/{email}")]
        public async Task<ActionResult<EmployeeMainViewDto>> GetEmployee(string email)
        {
            if (await _context.Employees.FirstOrDefaultAsync(e => e.Profile.Email == email) == null
                || await _userManager.FindByEmailAsync(email) == null) return BadRequest("Потребителят не съществува!");

            return GenerateNewEmployeeDto(email);
        }

        [HttpPut("api/employee/update-employee/{email}")]
        public async Task<ActionResult<EmployeeMainViewDto>> UpdateEmployeeAccount([FromForm] UpdateEmployeeDto employeeDto, string email)
        {
            var profile = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
            if (profile == null) return BadRequest("Потребителят не съществува");

            var employee = await _context.Employees.FirstOrDefaultAsync(m => m.Profile.Id == profile.Id);
            if (employee == null) return BadRequest("Потребителят не съществува");

            if (employee == null || profile == null) return BadRequest("Потребителят не е намерен!");

            if (employeeDto.NewFirstName != null) profile.FirstName = employeeDto.NewFirstName;
            if (employeeDto.NewLastName != null) profile.LastName = employeeDto.NewLastName;
            if (employeeDto.NewPhoneNumber != null) profile.PhoneNumber = employeeDto.NewPhoneNumber;
            if (employeeDto.ProfilePictureFile != null)
            {
                if (profile.ProfilePicturePath == null) profile.ProfilePicturePath = _picturesAndIconsService.SaveImage(employeeDto.ProfilePictureFile);
                else
                {
                    _picturesAndIconsService.DeleteImage(profile.ProfilePicturePath);
                    profile.ProfilePicturePath = _picturesAndIconsService.SaveImage(employeeDto.ProfilePictureFile);
                }
            }
            employee.IsLookingForJob = employeeDto.IsLookingForJob;

            _context.Update(employee);
            await _context.SaveChangesAsync();

            return GenerateNewEmployeeDto(profile.Email ?? string.Empty);
        }

        [HttpDelete("api/employee/delete-employee/{email}")]
        public async Task<IActionResult> DeleteEmployeeAccount(string email)
        {
            var profile = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
            var employee = await _context.Employees.FirstOrDefaultAsync(e => e.Profile.Email == email);

            if (employee == null || profile == null) return BadRequest("Потребителят не съществува!");
            var roles = await _userManager.GetRolesAsync(profile);

            foreach (var er in employee.EmployeesRestaurants) _context.EmployeesRestaurants.Remove(er);
            foreach (var r in _context.Requests.Where(r => r.Sender.Email == email || r.Receiver.Email == email)) _context.Requests.Remove(r);

            if (profile.ProfilePicturePath != null) _picturesAndIconsService.DeleteImage(profile.ProfilePicturePath);
            if (employee.QrCodePath != null) _qrCodesService.DeleteQrCode(employee.QrCodePath);

            _context.Employees.Remove(employee);
            await _userManager.RemoveFromRolesAsync(profile, roles);
            await _userManager.DeleteAsync(profile);
            await _context.SaveChangesAsync();

            return Ok(new JsonResult(new { title = "Успешно изтриване!", message = "Успешно изтрихте своя акаунт!" }));
        }

        [HttpGet("api/manager/browse-employees/details/{email}")]
        public async Task<ActionResult<EmployeeMainViewDto>> GetEmployeeDetails(string email)
        {
            if (await _context.Employees.FirstOrDefaultAsync(e => e.Profile.Email == email) == null
                || await _userManager.FindByEmailAsync(email) == null) return BadRequest("Потребителят не съществува!");

            return GenerateNewEmployeeDto(email);
        }

        [HttpGet("api/employee/get-all-requests/{email}")]  
        public async Task<ActionResult<List<RequestDto>>> GetRequests(string email)
        {
            if (await _userManager.FindByEmailAsync(email) == null) { return BadRequest("Потребителят не съществува!"); }

            List<RequestDto> requests = new List<RequestDto>();
            foreach (var r in _context.Requests.Where(r => r.Receiver.Email == email).OrderBy(x => x.SentOn))
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
                    Text = $"Работите ли в ресторантът {r.Restaurant.Name}, собственост на {r.Sender.FirstName + " " + r.Sender.LastName}?"
                };

                requests.Add(request);
            }

            return Ok(requests);
        }

        [HttpPost("api/employee/respond-to-request")]
        public async Task<IActionResult> RespondToRequest([FromBody] ResponceToRequestDto requestDto)
        {
            var request = _context.Requests.FirstOrDefault(r => r.Id.ToString() == requestDto.RequestId);
            if (request == null) return BadRequest("Заявката не съществува!");
            if (request.ConfirmedOn != null || request.RejectedOn != null) return BadRequest("Заявката вече е отговорена!");


            var employeeProfile = request.Receiver;
            var employee = await _context.Employees.FirstOrDefaultAsync(m => m.Profile.Email == employeeProfile.Email);
            if (employeeProfile == null || employee == null) return BadRequest("Потребителят не съществува!");

            var managerProfile = request.Sender;
            var manager = await _context.Managers.FirstOrDefaultAsync(e => e.Profile.Email == managerProfile.Email);
            if (managerProfile == null || manager == null) return BadRequest("Потребителят изпратил заявката не съществува!");

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

        [HttpGet("api/employee/download-qrcode/{email}")]
        public async Task<IActionResult> DownloadQrCode(string email)
        {
            var employee = await _context.Employees.FirstOrDefaultAsync(e => e.Profile.Email == email);
            if (employee == null) return BadRequest("Потребителят не съществува!");

            string imageName = employee.QrCodePath.Split("/").Last();
            string imagePath = Path.Combine(Directory.GetCurrentDirectory(), _configuration["QrCodes:Path"] ?? string.Empty, imageName);
            
            if (System.IO.File.Exists(imagePath))
            {
                return PhysicalFile(imagePath, "image/png", "qrcode.png");
            }
            else
            {
                return BadRequest("Вашият QrCode не беше намерен!");
            }
        }

        private EmployeeMainViewDto GenerateNewEmployeeDto(string email)
        {
            var employee = _context.Employees.FirstOrDefault(e => e.Profile.Email == email);
            var profile = _context.Users.FirstOrDefault(u => u.Email == email);

            if (employee == null || profile == null) throw new ArgumentNullException("Потребителят не съществува!");

            ICollection<AccountRestaurantDto> restaurants = new List<AccountRestaurantDto>();

            foreach (var restaurant in employee.EmployeesRestaurants
                .Where(er => !er.EndedOn.HasValue)
                .Select(er => er.Restaurant))
            {
                restaurants.Add(new AccountRestaurantDto
                {
                    Id = restaurant.Id.ToString(),
                    Name = restaurant.Name,
                    Address = restaurant.Address,
                    City = restaurant.City,
                    AtmosphereAverageRating = restaurant?.CuisineAverageRating ?? 0,
                    CuisineAverageRating = restaurant?.CuisineAverageRating ?? 0,
                    EmployeesAverageRating = restaurant?.EmployeesAverageRating ?? 0,
                    RestaurantAverageRating = restaurant?.RestaurantAverageRating ?? 0,
                    IconUrl = restaurant?.IconPath
                });
            }

            var employeeMainViewDto = new EmployeeMainViewDto
            {
                Email = email,
                FirstName = profile.FirstName,
                LastName = profile.LastName,
                ProfilePictureUrl = profile.ProfilePicturePath,
                QrCodeUrl = employee.QrCodePath,
                PhoneNumber = profile.PhoneNumber ?? string.Empty,
                City = employee.City,
                BirthDate = employee.BirthDate,
                AttitudeAverageRating = employee.AttitudeAverageRating ?? -1,
                CollegialityAverageRating = employee.CollegialityAverageRating ?? -1,
                SpeedAverageRating = employee.SpeedAverageRating ?? -1,
                PunctualityAverageRating = employee.PunctualityAverageRating ?? -1,
                EmployeeAverageRating = employee.EmployeeAverageRating ?? -1,
                Restaurants = restaurants,
                IsLookingForJob = employee.IsLookingForJob,
            };

            return employeeMainViewDto;
        }
    }
}
