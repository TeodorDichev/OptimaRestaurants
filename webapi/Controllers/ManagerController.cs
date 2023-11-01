using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using webapi.Data;
using webapi.DTOs.Employee;
using webapi.DTOs.Manager;

namespace webapi.Controllers
{
    public class ManagerController : Controller
    {
        private readonly OptimaRestaurantContext _context;
        private readonly RestaurantController _restaurantController;

        public ManagerController(OptimaRestaurantContext context
            , RestaurantController restaurantController)
        {
            _context = context;
            _restaurantController = restaurantController;
        }

        [HttpGet("{restaurantId}")] // pass the restaurant id to show the employees there
        public async Task<IActionResult> GetRestaurantEmployees(string restaurantId)
        {
            return Ok(_restaurantController.GetAllEmployeesOfARestaurant(restaurantId));
        }

        [HttpGet("{email}")] // pass either email from register or username from login
        public async Task<IActionResult> GetManager(string email)
        {
            var manager = await _context.Managers
            .FirstOrDefaultAsync(e => e.Profile.Email == email);

            if (manager == null) return NotFound();

            var managerMainViewDto = new ManagerMainViewDto
            {
                FirstName = manager.Profile.FirstName,
                LastName = manager.Profile.LastName,
                ProfilePictureUrl = manager.Profile?.ProfilePictureUrl ?? string.Empty,
                Restaurants = _restaurantController.GetAllRestaurantsOfAManager(email)
            };

            return Ok(managerMainViewDto);
        }

        public async Task<bool> SendRequestToEmployee(string managerId, string employeeId)
        {
            //employeesrestaurants through employee id -> add a record which is not confirmed
            //send an email to the employee
            //add a message to the employee inbox "do you work there"
            return true;
        }

        [HttpPut("api/manager/update-manager")]
        public async Task<IActionResult> UpdateManagerAccount(UpdateManagerDto managerDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Id == managerDto.CurrentManager.Profile.Id);

                if (existingUser == null) return NotFound("User not found");

                // Update the user's properties
                existingUser.FirstName = managerDto.NewFirstName; // by default they are filled with the old data
                existingUser.LastName = managerDto.NewLastName;
                existingUser.Email = managerDto.NewEmail;
                // if changed resend email -> ask the user to confirm their email
                existingUser.PhoneNumber = managerDto.NewPhoneNumber;
                existingUser.ProfilePictureUrl = managerDto.NewPictureUrl;
                // invoke reset password again

                _context.Entry(existingUser).State = EntityState.Modified;
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
