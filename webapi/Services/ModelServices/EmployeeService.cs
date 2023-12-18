using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using webapi.Data;
using webapi.DTOs.Employee;
using webapi.Models;
using webapi.Services.FileServices;

namespace webapi.Services.ClassServices
{
    public class EmployeeService
    {
        private readonly JWTService _jwtService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _configuration;
        private readonly OptimaRestaurantContext _context;
        private readonly QrCodesService _qrCodesService;
        private readonly PicturesAndIconsService _pictureService;

        public EmployeeService(JWTService jwtService,
        UserManager<ApplicationUser> userManager,
        IConfiguration configuration,
        OptimaRestaurantContext context,
        QrCodesService qrCodesService,
        PicturesAndIconsService pictureService)
        {
            _jwtService = jwtService;
            _userManager = userManager;
            _configuration = configuration;
            _context = context;
            _qrCodesService = qrCodesService;
            _pictureService = pictureService;
        }
        public async Task<Employee> AddEmployee(ApplicationUser userProfile, string city, DateTime birthDate)
        {
            await _userManager.AddToRoleAsync(userProfile, Role.Employee.ToString());
            Employee employee = new Employee
            {
                Profile = userProfile,
                City = city.ToUpper().First() + city.Substring(1).ToLower(),
                BirthDate = birthDate,
                QrCodePath = _qrCodesService.GenerateQrCode($"{_configuration["JWT:ClientUrl"]}review/{userProfile.Email}{_jwtService.GenerateQrToken(userProfile.Email)}")
            };
            await _context.Employees.AddAsync(employee);

            return employee;
        }
        public Employee UpdateEmployee(Employee employee, UpdateEmployeeDto updateDto)
        {
            if (updateDto.NewFirstName != null) employee.Profile.FirstName = updateDto.NewFirstName;
            if (updateDto.NewLastName != null) employee.Profile.LastName = updateDto.NewLastName;
            if (updateDto.NewPhoneNumber != null) employee.Profile.PhoneNumber = updateDto.NewPhoneNumber;
            if (updateDto.NewCity != null) employee.City = updateDto.NewCity;
            if (updateDto.NewBirthDate != null) employee.BirthDate = (DateTime)updateDto.NewBirthDate;
            if (updateDto.ProfilePictureFile != null)
            {
                if (employee.Profile.ProfilePicturePath == null) employee.Profile.ProfilePicturePath = _pictureService.SaveImage(updateDto.ProfilePictureFile);
                else
                {
                    _pictureService.DeleteImage(employee.Profile.ProfilePicturePath);
                    employee.Profile.ProfilePicturePath = _pictureService.SaveImage(updateDto.ProfilePictureFile);
                }
            }
            employee.IsLookingForJob = updateDto.IsLookingForJob;
            return employee;
        }
        public async Task<bool> DeleteEmployee(Employee employee)
        {
            try
            {
                var roles = await _userManager.GetRolesAsync(employee.Profile);

                foreach (var er in employee.EmployeesRestaurants) _context.EmployeesRestaurants.Remove(er);
                foreach (var r in _context.Requests.Where(r => r.Sender.Email == employee.Profile.Email || r.Receiver.Email == employee.Profile.Email)) _context.Requests.Remove(r);

                if (employee.Profile.ProfilePicturePath != null) _pictureService.DeleteImage(employee.Profile.ProfilePicturePath);
                if (employee.QrCodePath != null) _qrCodesService.DeleteQrCode(employee.QrCodePath);

                _context.Employees.Remove(employee);
                await _userManager.RemoveFromRolesAsync(employee.Profile, roles);
                await _userManager.DeleteAsync(employee.Profile);

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public async Task<Employee> GetEmployeeByEmail(string email)
        {
            return await _context.Employees.FirstOrDefaultAsync(e => e.Profile.Email == email) ?? throw new ArgumentNullException("Потребителят не съществува");
        }
        public async Task<bool> CheckEmployeeExistByEmail(string email)
        {
            return await _context.Employees.AnyAsync(x => x.Profile.Email == email);
        }
        public List<BrowseEmployeeDto> GetEmployeesLookingForJob()
        {
            List<BrowseEmployeeDto> employeesDto = new List<BrowseEmployeeDto>();

            foreach (var employee in _context.Employees.Where(e => e.IsLookingForJob).ToList())
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
        public string GetEmployeeQrCodePath(Employee employee)
        {
            string qrCodeName = employee.QrCodePath.Split("/").Last();
            return Path.Combine(Directory.GetCurrentDirectory(), _configuration["QrCodes:Path"] ?? string.Empty, qrCodeName);
        }
        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
