using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using webapi.Data;
using webapi.Models;

namespace webapi.Services
{
    public class PictureAndIconService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly OptimaRestaurantContext _context;

        public PictureAndIconService(UserManager<ApplicationUser> userManager,
            OptimaRestaurantContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        public async Task<string> UploadProfilePictureAsync(IFormFile file, string email)
        {
            if (!IsValidPictureFormat(file) || !IsWithinSizeLimit(file, 5 * 1024 * 1024)) // 5MB limit
            {
                throw new InvalidOperationException("Invalid picture format or size exceeds the limit.");
            }
            using (var memoryStream = new MemoryStream())
            {
                await file.CopyToAsync(memoryStream);
                var user = await _userManager.FindByEmailAsync(email);

                if (user != null)
                {
                    user.ProfilePictureData = memoryStream.ToArray();
                    user.ProfilePictureMimeType = file.ContentType;
                    await _userManager.UpdateAsync(user);
                }
            }

            return email;
        }

        public async Task<byte[]> GetProfilePictureDataAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);

            return user?.ProfilePictureData;
        }

        public async Task<string> UploadRestaurantIconAsync(IFormFile file, string restaurantId)
        {
            if (!IsValidPictureFormat(file) || !IsWithinSizeLimit(file, 5 * 1024 * 1024)) // 5MB limit
            {
                throw new InvalidOperationException("Invalid picture format or size exceeds the limit.");
            }   
            using (var memoryStream = new MemoryStream())
            {
                await file.CopyToAsync(memoryStream);
                var restaurant = await _context.Restaurants.FirstOrDefaultAsync(r => r.Id.ToString() == restaurantId);

                if (restaurant != null)
                {
                    restaurant.IconData = memoryStream.ToArray();
                    restaurant.IconMimeType = file.ContentType;
                    _context.Update(restaurant);
                }
            }

            return restaurantId;
        }

        public async Task<byte[]> GetRestaurantIconDataAsync(string restaurantId)
        {
            var restaurant = await _context.Restaurants.FirstOrDefaultAsync(r => r.Id.ToString() == restaurantId);

            return restaurant?.IconData;
        }

        private bool IsValidPictureFormat(IFormFile file)
        {
            string[] validFormats = { "image/png", "image/jpeg", "image/jpg" };
            return validFormats.Contains(file.ContentType);
        }

        private bool IsWithinSizeLimit(IFormFile file, int sizeLimit)
        {
            return file.Length <= sizeLimit;
        }
    }
}
