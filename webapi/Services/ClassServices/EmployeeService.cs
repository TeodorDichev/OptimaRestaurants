using Microsoft.AspNetCore.Identity;
using webapi.Data;
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

        public EmployeeService(JWTService jwtService,
        UserManager<ApplicationUser> userManager,
        IConfiguration configuration,
        OptimaRestaurantContext context,
        QrCodesService qrCodesService)
        {
            _jwtService = jwtService;
            _userManager = userManager;
            _configuration = configuration;
            _context = context;
            _qrCodesService = qrCodesService;
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
    }
}
