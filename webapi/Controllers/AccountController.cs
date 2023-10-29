using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Text;
using webapi.Data;
using webapi.DTOs.Account;
using webapi.Models;
using webapi.Services;

namespace webapi.Controllers
{
    /// <summary>
    /// Manages all tasks related to the accounts of a manager and an employee
    /// Edit account: picture, name, email, password
    /// </summary>
    
    public class AccountController : Controller
    {
        private readonly JWTService _jwtService;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly EmailService _emailService;
        private readonly IConfiguration _configuration;
        private readonly OptimaRestaurantContext _context;

        public AccountController(JWTService jwtService,
        SignInManager<ApplicationUser> signInManager,
        UserManager<ApplicationUser> userManager,
        EmailService emailService,
        IConfiguration configuration,
        OptimaRestaurantContext context)
        {
            _jwtService = jwtService;
            _signInManager = signInManager;
            _userManager = userManager;
            _emailService = emailService;
            _configuration = configuration;
            _context = context;
        }

        [HttpPost("api/account/login")]
        public async Task<ActionResult<ApplicationUserDto>> Login([FromBody] LoginDto model)
        {
            var user = await _userManager.FindByNameAsync(model.UserName);
            if (user == null) return Unauthorized("Грешен имейл или парола!");

            if (user.EmailConfirmed == false) return Unauthorized("Моля потвърдете имейл адреса си.");

            var result = await _signInManager.CheckPasswordSignInAsync(user, model.Password, false);
            if (!result.Succeeded) return Unauthorized("Грешен имейл или парола!");

            return CreateApplicationUserDto(user);
        }

        [Authorize]
        [HttpGet("/api/account/refresh-user-token")]
        public async Task<ActionResult<ApplicationUserDto>> RefreshUserToken()
        {
            var user = await _userManager.FindByNameAsync(User.FindFirst(ClaimTypes.Email)?.Value);
            return CreateApplicationUserDto(user);
        }

        [HttpPost("api/account/registerManager")]
        public async Task<ActionResult<ApplicationUserDto>> RegisterManager([FromBody] RegisterManagerDto model)
        {
            if (await CheckEmailExistAsync(model.Email))
            {
                return BadRequest($"Вече съществува акаунт с този имейл адрес!");
            }

            var userToAdd = new ApplicationUser
            {
                FirstName = model.FirstName.ToLower(),
                LastName = model.LastName.ToLower(),
                Email = model.Email.ToLower(),
                UserName = model.Email.ToLower(),
            };

            var result = await _userManager.CreateAsync(userToAdd, model.Password);
            if (!result.Succeeded) return BadRequest(result.Errors);

            try
            {
                if (await SendConfirmEmailAddress(userToAdd))
                {
                    return Ok(new JsonResult(new { title = "Успешно създаден акаунт!", message = "Вашият акаунт беше създаден. Моля, потвърдете имейл адреса си." }));
                }
            }
            catch (Exception)
            {
                return BadRequest("Неуспешно изпращане на имейл. Моля свържете се с администратор.");
            }

            await _userManager.AddToRoleAsync(userToAdd, Role.Manager.ToString());
            Manager manager = new Manager { Profile = userToAdd };
            await _context.Managers.AddAsync(manager);
            await _context.SaveChangesAsync();
            return Ok(new JsonResult(new { title = "Успешно създаден акаунт!", message = "Вашият акаунт беше създаден!" })); // login directly
        }
        [HttpPost("api/account/registerEmployee")]
        public async Task<ActionResult<ApplicationUserDto>> RegisterEmployee([FromBody] RegisterEmployeeDto model)
        {
            if (await CheckEmailExistAsync(model.Email))
            {
                return BadRequest($"Вече съществува акаунт с този имейл адрес");
            }

            var userToAdd = new ApplicationUser
            {
                FirstName = model.FirstName.ToLower(),
                LastName = model.LastName.ToLower(),
                Email = model.Email.ToLower(),
                UserName = model.Email.ToLower(),
            };

            var result = await _userManager.CreateAsync(userToAdd, model.Password);
            if (!result.Succeeded) return BadRequest(result.Errors);

            try
            {
                if (await SendConfirmEmailAddress(userToAdd))
                {
                    return Ok(new JsonResult(new { title = "Успешно създаден акаунт!", message = "Вашият акаунт беше създаден. Моля, потвърдете имейл адреса си." }));
                }
            }
            catch (Exception)
            {
                return BadRequest("Неуспешно изпращане на имейл. Моля свържете се с администратор.");
            }

            await _userManager.AddToRoleAsync(userToAdd, Role.Employee.ToString());
            Employee employee = new Employee
            {
                Profile = userToAdd,
                City = model.City.ToLower(),
                BirthDate = model.BirthDate
            };
            await _context.Employees.AddAsync(employee);
            await _context.SaveChangesAsync();
            return Ok(new JsonResult(new { title = "Успешно създаден акаунт!", message = "Вашият акаунт беше създаден!" })); // login directly
        }

        [HttpPut("api/account/confirmEmail")]
        public async Task<IActionResult> ConfirmEmail([FromBody] ConfirmEmailDto model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null) return Unauthorized("This email has not been registered yet");
            if (user.EmailConfirmed == true) return BadRequest("This email has already been confirmed");

            try
            {
                var decodedTokenBytes = WebEncoders.Base64UrlDecode(model.Token);
                var decodedToken = Encoding.UTF8.GetString(decodedTokenBytes);

                var result = await _userManager.ConfirmEmailAsync(user, decodedToken);
                if (result.Succeeded)
                {
                    return Ok(new JsonResult(new { title = "Email confirmed", message = "Вашият акаунт беше създаден!" })); // change message and login directly
                }

                return BadRequest("Invalid token. Please try again");
            }
            catch (Exception)
            {
                return BadRequest("Invalid token. Please try again");
            }
        }
        [HttpPut("api/account/resetPassword")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null) return Unauthorized("This email has not been registered yet");
            if (user.EmailConfirmed == false) return BadRequest("This email has already been confirmed");

            try
            {
                var decodedTokenBytes = WebEncoders.Base64UrlDecode(model.Token);
                var decodedToken = Encoding.UTF8.GetString(decodedTokenBytes);

                var result = await _userManager.ResetPasswordAsync(user, decodedToken, model.Password);
                if (result.Succeeded)
                {
                    return Ok(new JsonResult(new { title = "Password reset success", message = "Your password has been reseted" }));
                }

                return BadRequest("Invalid token. Please try again");
            }
            catch (Exception)
            {
                return BadRequest("Invalid token. Please try again");
            }
        }

        [HttpPost("api/account/resendEmailConfirmationLink/{email}")]
        public async Task<IActionResult> ResendEmailConfimationLink(string email)
        {
            if (string.IsNullOrEmpty(email)) return BadRequest("Invalid email");
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null) return Unauthorized("This email has not been registered yet");
            if (user.EmailConfirmed == true) return BadRequest("This email has already been confirmed");

            try
            {
                if (await SendConfirmEmailAddress(user))
                {
                    return Ok(new JsonResult(new { title = "Confirmation link send", message = "Please confirm your email address" }));
                }
                return BadRequest("Failed to send email. Please contact admin");
            }
            catch (Exception)
            {
                return BadRequest("Failed to send email. Please contact admin");
            }
        }

        [HttpPost("api/account/forgotUsernameOrPassword/{email}")]
        public async Task<IActionResult> ForgotUsernameOrPassword(string email)
        {
            if (string.IsNullOrEmpty(email)) return BadRequest("Invalid email");
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null) return Unauthorized("This email has not been registered yet");
            if (user.EmailConfirmed == false) return BadRequest("Please confirm your email address first.");

            try
            {
                if (await SendForgotUsernameOrPassword(user))
                {
                    return Ok(new JsonResult(new { title = "Forgot username or password email sent", message = "Please check your email" }));
                }

                return BadRequest("Failed to send email. Please contact admin");
            }
            catch (Exception)
            {
                return BadRequest("Failed to send email. Please contact admin");
            }
        }

        [HttpPut("api/account/updateEmployee")]
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

        [HttpPut("api/account/updateManager")]
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


        private async Task<bool> SendForgotUsernameOrPassword(ApplicationUser user)
        {
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            token = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));
            var url = $"{_configuration["JWT:ClientUrl"]}{_configuration["Email:ResetPasswordPath"]}?token={token}&email={user.Email}";

            var body = $"<p>Hello: {user.FirstName} {user.LastName}</p>" +
                $"<p>Username: {user.UserName}.</p>" +
                "<p>You can reset your password by clicking on the following link.</p>" +
                $"<p><a href=\"{url}\">Click here</a></p>" +
                "<p>Thank you,</p>" +
                $"<br>{_configuration["Email:ApplicationName"]}";

            var emailSend = new EmailSendDto(user.Email, body, "Forgot username or password");

            return await _emailService.SendEmailAsync(emailSend);
        }

        private async Task<bool> SendConfirmEmailAddress(ApplicationUser user)
        {
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            token = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));
            var url = $"{_configuration["JWT:ClientUrl"]}{_configuration["Email:ConfirmEmailPath"]}?token={token}&email={user.Email}";

            var body = $"<p>Hello: {user.FirstName} {user.LastName}</p>" +
                "<p>Please confirm your email address here:</p>" +
                $"<p><a href=\"{url}\">Click here</a></p>" +
                "<p>Thank you,</p>" +
                $"<br>{_configuration["Email:ApplicationName"]}";

            var emailSend = new EmailSendDto(user.Email, body, "Confirm your email");

            return await _emailService.SendEmailAsync(emailSend);
        }

        private ApplicationUserDto CreateApplicationUserDto(ApplicationUser user)
        {
            return new ApplicationUserDto
            {
                Email = user.Email,
                JWT = _jwtService.CreateJWT(user)
            };
        }
        private async Task<bool> CheckEmailExistAsync(string email)
        {
            return await _userManager.Users.AnyAsync(x => x.Email == email.ToLower());
        }
    }
}
