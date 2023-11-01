using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using webapi.Data;
using webapi.DTOs.Employee;

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
        public EmployeeController(OptimaRestaurantContext context,
            RestaurantController restaurantController)
        {
            _context = context;
            _restaurantController = restaurantController;
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

        [HttpPut("api/employee/update-employee")]
        public async Task<IActionResult> UpdateEmployeeAccount(UpdateEmployeeDto employeeDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Id == employeeDto.CurrentEmployee.Profile.Id);
                var existingEmp = await _context.Employees.FirstOrDefaultAsync(u => u.Id == employeeDto.CurrentEmployee.Id);

                if (existingUser == null || existingEmp == null) return NotFound("User not found");

                // Update the user's properties
                existingUser.FirstName = employeeDto.NewFirstName; // by default they are filled with the old data
                existingUser.LastName = employeeDto.NewLastName;
                existingUser.Email = employeeDto.NewEmail;
                // if changed resend email -> ask the user to confirm their email
                existingUser.PhoneNumber = employeeDto.NewPhoneNumber;
                existingUser.ProfilePictureUrl = employeeDto.NewPictureUrl;
                existingEmp.BirthDate = employeeDto.NewBirthDate;
                existingEmp.City = employeeDto.NewCity;
                // invoke reset password again

                _context.Entry(existingUser).State = EntityState.Modified;
                _context.Entry(existingEmp).State = EntityState.Modified;
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
