using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using webapi.Data;
using webapi.Models;

namespace webapi.Services
{
    /// <summary>
    /// The service seeds the roles in the Database
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
