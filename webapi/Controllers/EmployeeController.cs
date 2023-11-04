using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using webapi.Data;
using webapi.DTOs.Account;
using webapi.DTOs.Employee;
using webapi.DTOs.Restaurant;
using webapi.Models;

namespace webapi.Controllers
{
    /// <summary>
    /// This class manages all employee related functions
    /// Edit - city, birthdate
    /// Add/Delete a request to a manager's restaurant
    /// </summary>
    [Authorize(Roles = "Employee")]
    public class EmployeeController : Controller
    {
        private readonly OptimaRestaurantContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public EmployeeController(OptimaRestaurantContext context,
            RestaurantController restaurantController,
            AccountController accountController,
            UserManager<ApplicationUser> userManager)
        {
            _context = context;
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

            ICollection<RestaurantDto> restaurants = new List<RestaurantDto>();

            foreach (var restaurant in _context.EmployeesRestaurants
                .Where(x => x.Employee.Profile.Email == email && !x.EndedOn.HasValue && x.ConfirmedOn.HasValue)
                .Select(x => x.Restaurant)
                .ToList())
            {
                restaurants.Add(new RestaurantDto
                {
                    Id = restaurant.Id.ToString(),
                    Name = restaurant.Name,
                    Address = restaurant.Address,
                    City = restaurant.City,
                    AtmosphereAverageRating = restaurant?.CuisineAverageRating ?? -1, // in front-end if -1 "no reviews yet"
                    CuisineAverageRating = restaurant?.CuisineAverageRating ?? -1,
                    EmployeesAverageRating = restaurant?.EmployeesAverageRating ?? -1,
                    IconUrl = restaurant?.IconUrl ?? string.Empty,
                });
            }

            if (employee == null) return NotFound();

            var employeeMainViewDto = new EmployeeMainViewDto
            {
                Email = email,
                FirstName = employee.Profile.FirstName,
                LastName = employee.Profile.LastName,
                ProfilePictureUrl = employee?.Profile.ProfilePictureUrl ?? string.Empty,
                PhoneNumber = employee.Profile?.PhoneNumber ?? string.Empty,
                City = employee?.City ?? string.Empty,
                BirthDate = employee?.BirthDate ?? DateTime.Now,
                AttitudeAverageRating = employee?.AttitudeAverageRating ?? -1, // in front-end if -1 then "no reviews yet"
                CollegialityAverageRating = employee?.CollegialityAverageRating ?? -1,
                SpeedAverageRating = employee?.SpeedAverageRating ?? -1,
                PunctualityAverageRating = employee?.PunctualityAverageRating ?? -1,
                EmployeeAverageRating = employee?.EmployeeAverageRating ?? -1,
                Restaurants = restaurants
            };

            return Ok(employeeMainViewDto);
        }

    }
}
