using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using webapi.Data;
using webapi.Models;

namespace webapi.Services.ClassServices
{
    public class AccountService
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly OptimaRestaurantContext _context;

        public AccountService(SignInManager<ApplicationUser> signInManager,
        UserManager<ApplicationUser> userManager,
        OptimaRestaurantContext context)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _context = context;
        }

        public async Task<bool> CheckUserExistByEmail(string email)
        {
            return await _userManager.Users.AnyAsync(x => x.Email == email.ToLower());
        }

        public async Task<bool> CheckPasswordAsync(ApplicationUser user, string password)
        {
            var result = await _signInManager.CheckPasswordSignInAsync(user, password, false);
            if (result.Succeeded) return true;
            else return false;
        }

        public async Task<ApplicationUser> GetUserByEmailOrUserName(string email)
        {
            return await _userManager.FindByNameAsync(email) ?? throw new ArgumentNullException("Потребителят не съществува");
        }

        public async Task<ApplicationUser> AddApplicationUser(string firstName, string lastName, string email, string password)
        {
            var userToAdd = new ApplicationUser
            {
                FirstName = firstName.ToUpper().First() + firstName.Substring(1).ToLower(),
                LastName = lastName.ToUpper().First() + lastName.Substring(1).ToLower(),
                Email = email.ToLower(),
                UserName = email.ToLower(),
            };

            var result = await _userManager.CreateAsync(userToAdd, password);
            if (result.Succeeded) return userToAdd;
            else throw new ArgumentException(string.Join('\n', result.Errors));
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
