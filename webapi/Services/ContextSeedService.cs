using Microsoft.AspNetCore.Identity;
using webapi.Data;
using webapi.Models;
using webapi.Services.ClassServices;

namespace webapi.Services
{
    /// <summary>
    /// The service seeds the roles in the Database and two build-in accounts
    /// </summary>

    public class ContextSeedService
    {
        private readonly OptimaRestaurantContext _context;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly EmployeeService _employeeService;
        private readonly ManagerService _managerService;

        public ContextSeedService(
            OptimaRestaurantContext context,
            RoleManager<IdentityRole> roleManager,
            UserManager<ApplicationUser> userManager,
            EmployeeService employeeService,
            ManagerService managerService)
        {
            _context = context;
            _roleManager = roleManager;
            _userManager = userManager;
            _employeeService = employeeService;
            _managerService = managerService;
        }

        public async Task InitializeContextAsync()
        {
            if (!_roleManager.Roles.Any())
            {
                await _roleManager.CreateAsync(new IdentityRole { Name = Role.Manager.ToString() });
                await _roleManager.CreateAsync(new IdentityRole { Name = Role.Employee.ToString() });
            }

            if (!_context.Users.Any(u => u.Email == "manager@gmail.com"))
            {
                ApplicationUser profile = new ApplicationUser()
                {
                    FirstName = "Manager",
                    LastName = "Manager",
                    Email = "manager@gmail.com",
                    UserName = "manager@gmail.com"
                };

                var createUserResult = await _userManager.CreateAsync(profile, "Manager1P@ss");
                if (createUserResult.Succeeded)
                {
                    await _userManager.ConfirmEmailAsync(profile, await _userManager.GenerateEmailConfirmationTokenAsync(profile));
                    await _managerService.AddManager(profile);
                    await _context.SaveChangesAsync();
                }
            }

            if (!_context.Users.Any(u => u.Email == "employee@gmail.com"))
            {
                ApplicationUser profile = new ApplicationUser()
                {
                    FirstName = "Employee",
                    LastName = "Employee",
                    Email = "employee@gmail.com",
                    UserName = "employee@gmail.com"
                };

                var createUserResult = await _userManager.CreateAsync(profile, "Employee1P@ss");
                if (createUserResult.Succeeded)
                {
                    await _userManager.ConfirmEmailAsync(profile, await _userManager.GenerateEmailConfirmationTokenAsync(profile));
                    await _employeeService.AddEmployee(profile, "София", DateOnly.Parse("01/01/2000"));
                    await _context.SaveChangesAsync();
                }
            }
        }
    }
}
