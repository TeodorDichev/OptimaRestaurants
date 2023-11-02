using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using webapi.Data;
using webapi.DTOs.Account;
using webapi.DTOs.Employee;
using webapi.DTOs.Manager;
using webapi.Models;

namespace webapi.Controllers
{
    /// <summary>
    /// This class manages all employee related functions
    /// Edit - city, birthdate
    /// Add/Delete a request to a manager's restaurant
    /// </summary>
    public class EmployeeController : Controller
    {
        private readonly OptimaRestaurantContext _context;
        private readonly RestaurantController _restaurantController;
        private readonly AccountController _accountController;
        private readonly UserManager<ApplicationUser> _userManager;

        public EmployeeController(OptimaRestaurantContext context,
            RestaurantController restaurantController,
            AccountController accountController,
            UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _restaurantController = restaurantController;
            _accountController = accountController;
            _userManager = userManager;
        }

        [HttpDelete("api/employee/{email}")]
        public async Task<IActionResult> DeleteEmployeeAccount(string email)
        {
            var employee = await _context.Employees.FirstOrDefaultAsync(e => e.Profile.Email == email);

            foreach (var er in employee.EmployeesRestaurants)
            {
                er.Restaurant.EmployeesRestaurants.Remove(er);
                _context.Remove(er);
            }

            var user = employee.Profile;

            _context.Remove(user);
            _context.Remove(employee);
            await _context.SaveChangesAsync();

            return Ok("You have successfully deleted your account");
        }


        [HttpGet("api/employee{email}")] // pass either email from register or username from login
        public async Task<IActionResult> GetEmployee(string email)
        {
            var employee = await _context.Employees
            .FirstOrDefaultAsync(e => e.Profile.Email == email);

            if (employee == null) return NotFound();

            var employeeMainViewDto = new EmployeeMainViewDto
            {
                FirstName = employee.Profile.FirstName,
                LastName = employee.Profile.LastName,
                ProfilePictureUrl = employee?.Profile.ProfilePictureUrl ?? string.Empty,
                AttitudeAverageRating = employee?.AttitudeAverageRating ?? -1, // in front-end if -1 then "no reviews yet"
                CollegialityAverageRating = employee?.CollegialityAverageRating ?? -1,
                SpeedAverageRating = employee?.SpeedAverageRating ?? -1,
                PunctualityAverageRating = employee?.PunctualityAverageRating ?? -1,
                EmployeeAverageRating = employee?.EmployeeAverageRating ?? -1,
                Restaurants = _restaurantController.GetAllRestaurantsWhereEmployeeWorks(email)
            };

            return Ok(employeeMainViewDto);
        }

        [HttpPut("api/employee/{email}")]
        public async Task<IActionResult> UpdateEmployeeAccount(string email)
        {
            try
            {
                var existingEmployee = await _context.Employees.FirstOrDefaultAsync(m => m.Profile.Email == email);
                if (existingEmployee == null) return NotFound("User not found");

                UpdateEmployeeDto employeeDto = new UpdateEmployeeDto
                {
                    OldEmail = existingEmployee.Profile?.Email ?? string.Empty,
                    OldPhoneNumber = existingEmployee.Profile?.PhoneNumber ?? string.Empty,
                    OldFirstName = existingEmployee.Profile?.FirstName ?? string.Empty,
                    OldLastName = existingEmployee.Profile?.LastName ?? string.Empty,
                    OldPictureUrl = existingEmployee.Profile?.ProfilePictureUrl ?? string.Empty,
                    OldBirthDate = existingEmployee?.BirthDate ?? DateTime.Now,
                    OldCity = existingEmployee?.City ?? string.Empty,
                };

                // Update the user's properties
                if (!employeeDto.NewFirstName.IsNullOrEmpty()) existingEmployee.Profile.FirstName = employeeDto.NewFirstName;
                if (!employeeDto.NewLastName.IsNullOrEmpty()) existingEmployee.Profile.LastName = employeeDto.NewLastName;
                if (!employeeDto.NewPhoneNumber.IsNullOrEmpty()) existingEmployee.Profile.PhoneNumber = employeeDto.NewPhoneNumber;
                if (!employeeDto.NewPictureUrl.IsNullOrEmpty()) existingEmployee.Profile.ProfilePictureUrl = employeeDto.NewPictureUrl;
                if (employeeDto.NewBirthDate != null) existingEmployee.BirthDate = employeeDto.NewBirthDate.Value;
                if (!employeeDto.OldCity.IsNullOrEmpty()) existingEmployee.City = employeeDto.NewCity;

                // Reseting password
                if (!employeeDto.NewPassword.IsNullOrEmpty())
                {
                    ResetPasswordDto resetPasswordDto = new ResetPasswordDto
                    {
                        Token = await _userManager.GeneratePasswordResetTokenAsync(existingEmployee.Profile),
                        Email = existingEmployee.Profile.Email,
                        Password = employeeDto.NewPassword
                    };
                    await _accountController.ResetPassword(resetPasswordDto);
                    await _accountController.ForgotUsernameOrPassword(existingEmployee.Profile.Email);
                }

                // Reseting email
                if (!employeeDto.NewEmail.IsNullOrEmpty())
                {
                    ConfirmEmailDto confirmEmailDto = new ConfirmEmailDto
                    {
                        Token = await _userManager.GeneratePasswordResetTokenAsync(existingEmployee.Profile),
                        Email = employeeDto.NewEmail,
                    };

                    existingEmployee.Profile.Email = employeeDto.NewEmail;
                    await _accountController.ConfirmEmail(confirmEmailDto);
                }

                _context.Update(existingEmployee);
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
