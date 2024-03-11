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

        public ContextSeedService(
            OptimaRestaurantContext context,
            RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _roleManager = roleManager;
        }

        public async Task InitializeContextAsync()
        {
            if (!_roleManager.Roles.Any())
            {
                await _roleManager.CreateAsync(new IdentityRole { Name = Role.Manager.ToString() });
                await _roleManager.CreateAsync(new IdentityRole { Name = Role.Employee.ToString() });
            }

            await _context.SaveChangesAsync();
        }
    }
}
