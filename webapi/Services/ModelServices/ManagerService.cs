using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using webapi.Data;
using webapi.DTOs.Manager;
using webapi.Models;
using webapi.Services.FileServices;

namespace webapi.Services.ClassServices
{
    public class ManagerService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly OptimaRestaurantContext _context;
        private readonly PicturesAndIconsService _pictureService;

        public ManagerService(UserManager<ApplicationUser> userManager,
        OptimaRestaurantContext context,
        PicturesAndIconsService pictureService)
        {
            _userManager = userManager;
            _context = context;
            _pictureService = pictureService;
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
        public async Task<bool> DeleteManager(Manager manager)
        {
            try
            {
                var roles = await _userManager.GetRolesAsync(manager.Profile);

                foreach (var restaurant in manager.Restaurants) restaurant.Manager = null;

                if (manager.Profile.ProfilePicturePath != null) _pictureService.DeleteImage(manager.Profile.ProfilePicturePath);
                foreach (var r in _context.Requests.Where(r => r.Sender.Email == manager.Profile.Email || r.Receiver.Email == manager.Profile.Email)) _context.Requests.Remove(r);

                _context.Managers.Remove(manager);
                await _userManager.RemoveFromRolesAsync(manager.Profile, roles);
                await _userManager.DeleteAsync(manager.Profile);

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public Manager UpdateManager(Manager manager, UpdateManagerDto updateDto)
        {
            if (updateDto.NewFirstName != null) manager.Profile.FirstName = updateDto.NewFirstName;
            if (updateDto.NewLastName != null) manager.Profile.LastName = updateDto.NewLastName;
            if (updateDto.NewPhoneNumber != null) manager.Profile.PhoneNumber = updateDto.NewPhoneNumber;
            if (updateDto.ProfilePictureFile != null)
            {
                if (manager.Profile.ProfilePicturePath == null) manager.Profile.ProfilePicturePath = _pictureService.SaveImage(updateDto.ProfilePictureFile);
                else
                {
                    _pictureService.DeleteImage(manager.Profile.ProfilePicturePath);
                    manager.Profile.ProfilePicturePath = _pictureService.SaveImage(updateDto.ProfilePictureFile);
                }
            }

            return manager;
        }
        public async Task<Manager> GetManagerByEmail(string email)
        {
            return await _context.Managers.FirstOrDefaultAsync(e => e.Profile.Email == email) ?? throw new ArgumentNullException("Потребителят не съществува");
        }
        public async Task<bool> CheckManagerExistByEmail(string email)
        {
            return await _context.Managers.AnyAsync(x => x.Profile.Email == email);
        }
        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
