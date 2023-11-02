using Mailjet.Client.Resources;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using webapi.Data;
using webapi.DTOs.Account;
using webapi.DTOs.Employee;
using webapi.DTOs.Manager;
using webapi.DTOs.Restaurant;
using webapi.Models;

namespace webapi.Controllers
{
    public class ManagerController : Controller
    {
        private readonly OptimaRestaurantContext _context;
        private readonly RestaurantController _restaurantController;
        private readonly AccountController _accountController;
        private readonly UserManager<ApplicationUser> _userManager;

        public ManagerController(OptimaRestaurantContext context,
                RestaurantController restaurantController,
                AccountController accountController,
                UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _restaurantController = restaurantController;
            _accountController = accountController;
            _userManager = userManager;
        }

        [HttpGet("{restaurantId}")] // pass the restaurant id to show the employees there
        public async Task<IActionResult> GetRestaurantEmployees(string restaurantId)
        {
            return Ok(_restaurantController.GetAllEmployeesOfARestaurant(restaurantId));
        }

        [HttpGet("api/manager/{email}")] // pass either email from register or username from login
        public async Task<IActionResult> GetManager(string email)
        {
            var manager = await _context.Managers
            .FirstOrDefaultAsync(e => e.Profile.Email == email);

            if (manager == null) return NotFound();

            var managerMainViewDto = new ManagerMainViewDto
            {
                Email = email,
                FirstName = manager.Profile.FirstName,
                LastName = manager.Profile.LastName,
                ProfilePictureUrl = manager.Profile?.ProfilePictureUrl ?? string.Empty,
                Restaurants = _restaurantController.GetAllRestaurantsOfAManager(email)
            };

            return Ok(managerMainViewDto);
        }

        [HttpPost("api/manager/{email}")]
        public async Task<IActionResult> AddNewRestaurant([FromBody] NewRestaurantDto newRestaurant, string managerEmail)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var manager = await _context.Managers
                .FirstOrDefaultAsync(e => e.Profile.Email == managerEmail);

            if (manager == null) return NotFound();

            Restaurant restaurant = new Restaurant
            {
                Name = newRestaurant.Name,
                Address = newRestaurant.Address,
                City = newRestaurant.City,
                IsWorking = true,
                EmployeeCapacity = newRestaurant.EmployeeCapacity,
                Manager = manager
            };
            await _context.Restaurants.AddAsync(restaurant);

            return Ok("You have successfully added a new restaurant");
        }

        [HttpPut("api/manager/{email}")]
        public async Task<IActionResult> UpdateManagerAccount(string email)
        {
            try
            {
                var existingUser = await _context.Users.FirstOrDefaultAsync(m => m.Email == email);
                if (existingUser == null) return NotFound("User not found");

                UpdateManagerDto managerDto = new UpdateManagerDto
                {
                    OldEmail = existingUser?.Email ?? string.Empty,
                    OldPhoneNumber = existingUser?.PhoneNumber ?? string.Empty,
                    OldFirstName = existingUser?.FirstName ?? string.Empty,
                    OldLastName = existingUser?.LastName ?? string.Empty,
                    OldPictureUrl = existingUser?.ProfilePictureUrl ?? string.Empty,
                };

                // Update the user's properties
                if (!managerDto.NewFirstName.IsNullOrEmpty()) existingUser.FirstName = managerDto.NewFirstName;
                if (!managerDto.NewLastName.IsNullOrEmpty()) existingUser.LastName = managerDto.NewLastName;
                if (!managerDto.NewPhoneNumber.IsNullOrEmpty()) existingUser.PhoneNumber = managerDto.NewPhoneNumber;
                if (!managerDto.NewPictureUrl.IsNullOrEmpty()) existingUser.ProfilePictureUrl = managerDto.NewPictureUrl;

                // Reseting password
                if (!managerDto.NewPassword.IsNullOrEmpty())
                {
                    ResetPasswordDto resetPasswordDto = new ResetPasswordDto
                    {
                        Token = await _userManager.GeneratePasswordResetTokenAsync(existingUser),
                        Email = existingUser.Email,
                        Password = managerDto.NewPassword
                    };
                    await _accountController.ResetPassword(resetPasswordDto);
                    await _accountController.ForgotUsernameOrPassword(existingUser.Email);
                }

                // Reseting email
                if (!managerDto.NewEmail.IsNullOrEmpty())
                {
                    ConfirmEmailDto confirmEmailDto = new ConfirmEmailDto
                    {
                        Token = await _userManager.GeneratePasswordResetTokenAsync(existingUser),
                        Email = managerDto.NewEmail,
                    };

                    existingUser.Email = managerDto.NewEmail;
                    await _accountController.ConfirmEmail(confirmEmailDto);
                }

                _context.Update(existingUser);
                await _context.SaveChangesAsync();

                return Ok("Account updated successfully");
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
