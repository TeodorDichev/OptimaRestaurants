using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using webapi.Data;
using webapi.Models;

namespace webapi.Services
{
    public class ContextSeedService
    {
        private readonly OptimaRestaurantContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public ContextSeedService(
            OptimaRestaurantContext context,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task InitializeContextAsync()
        {
            if (_context.Database.GetPendingMigrationsAsync().GetAwaiter().GetResult().Count() > 0)
            {
                await _context.Database.MigrateAsync();
            }

            if(! _roleManager.Roles.Any())
            {
                await _roleManager.CreateAsync(new IdentityRole { Name = Role.Manager.ToString() });
                await _roleManager.CreateAsync(new IdentityRole { Name = Role.Employee.ToString() });
            }
        }
    }
}
