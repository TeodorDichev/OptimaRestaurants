using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using webapi.Data;
using webapi.Models;

namespace webapi.Services.ClassServices
{
    public class ManagerService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly OptimaRestaurantContext _context;

        public ManagerService(UserManager<ApplicationUser> userManager,
        OptimaRestaurantContext context)
        {
            _userManager = userManager;
            _context = context;
        }


        public async Task<bool> IsUserManager(ApplicationUser userProfile)
        {
            return await _context.Managers.AnyAsync(x => x.Profile.Email == userProfile.Email);
        }
        public async Task<Manager> AddManager(ApplicationUser userProfile)
        {
            await _userManager.AddToRoleAsync(userProfile, Role.Manager.ToString());
            Manager manager = new Manager { Profile = userProfile };
            await _context.Managers.AddAsync(manager);

            return manager;
        }

    }
}
