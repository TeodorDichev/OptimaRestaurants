using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;
using webapi.Data;
using webapi.DTOs.Account;
using webapi.DTOs.Employee;
using webapi.DTOs.Manager;
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

        //[Authorize(Roles = "Employee,Manager")]
        [HttpGet("/api/account/refresh-user-token")]
        public async Task<ActionResult<ApplicationUserDto>> RefreshUserToken()
        {
            var user = await _userManager.FindByEmailAsync(User.FindFirst(ClaimTypes.Email)?.Value);
            return CreateApplicationUserDto(user);
        }

        [HttpPost("api/account/register-manager")]
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
                    await _userManager.AddToRoleAsync(userToAdd, Role.Manager.ToString());
                    Manager manager = new Manager { Profile = userToAdd };
                    await _context.Managers.AddAsync(manager);
                    await _context.SaveChangesAsync();
                    return Ok(new JsonResult(new { title = "Успешно създаден акаунт!", message = "Вашият акаунт беше създаден. Моля, потвърдете имейл адреса си." }));
                }
                else return BadRequest("Неуспешно изпращане на имейл. Моля свържете се с администратор.");
            }
            catch (Exception)
            {
                return BadRequest("Неуспешно изпращане на имейл. Моля свържете се с администратор.");
            }
        }

        [HttpPost("api/account/register-employee")]
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
                    await _userManager.AddToRoleAsync(userToAdd, Role.Employee.ToString());
                    Employee employee = new Employee
                    {
                        Profile = userToAdd,
                        City = model.City.ToLower(),
                        BirthDate = model.BirthDate
                    };
                    await _context.Employees.AddAsync(employee);
                    await _context.SaveChangesAsync();
                    return Ok(new JsonResult(new { title = "Успешно създаден акаунт!", message = "Вашият акаунт беше създаден. Моля, потвърдете имейл адреса си." }));
                }
                else return BadRequest("Неуспешно изпращане на имейл. Моля свържете се с администратор.");
            }
            catch (Exception)
            {
                return BadRequest("Неуспешно изпращане на имейл. Моля свържете се с администратор.");
            }

        }

        [HttpPut("api/account/confirm-email")]
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
        [HttpPut("api/account/reset-password")]
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

        [HttpPost("api/account/resend-email-confirmation-link/{email}")]
        public async Task<IActionResult> ResendEmailConfirmationLink(string email)
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

        [HttpPost("api/account/forgot-username-or-password/{email}")]
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

        [HttpPut("api/employee/update-employee")]
        public async Task<IActionResult> UpdateEmployeeAccount([FromBody] UpdateEmployeeDto employeeDto)
        {
            try
            {
                var existingEmployee = await _context.Employees.FirstOrDefaultAsync(m => m.Profile.Email == employeeDto.OldEmail);
                if (existingEmployee == null) return NotFound("User not found");


                // Update the user's properties
                if (!employeeDto.NewFirstName.IsNullOrEmpty()) existingEmployee.Profile.FirstName = employeeDto.NewFirstName;
                if (!employeeDto.NewLastName.IsNullOrEmpty()) existingEmployee.Profile.LastName = employeeDto.NewLastName;
                if (!employeeDto.NewPhoneNumber.IsNullOrEmpty()) existingEmployee.Profile.PhoneNumber = employeeDto.NewPhoneNumber;
                if (!employeeDto.NewPictureUrl.IsNullOrEmpty()) existingEmployee.Profile.ProfilePictureUrl = employeeDto.NewPictureUrl;
                if (employeeDto.NewBirthDate != null) existingEmployee.BirthDate = employeeDto.NewBirthDate.Value;
                if (!employeeDto.NewCity.IsNullOrEmpty()) existingEmployee.City = employeeDto.NewCity;

                // Reseting password
                if (!employeeDto.NewPassword.IsNullOrEmpty())
                {
                    ResetPasswordDto resetPasswordDto = new ResetPasswordDto
                    {
                        Token = await _userManager.GeneratePasswordResetTokenAsync(existingEmployee.Profile),
                        Email = existingEmployee.Profile.Email,
                        Password = employeeDto.NewPassword
                    };

                    await ResetPassword(resetPasswordDto);
                    await ForgotUsernameOrPassword(existingEmployee.Profile.Email);
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
                    await ConfirmEmail(confirmEmailDto);
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
                    await ResetPassword(resetPasswordDto);
                    await ForgotUsernameOrPassword(existingUser.Email);
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
                    await ConfirmEmail(confirmEmailDto);
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
            bool isManager = false;
            if (_context.Managers.FirstOrDefault(x => x.Profile.Email == user.Email) != null) isManager = true;

            return new ApplicationUserDto
            {
                Email = user.Email,
                JWT = _jwtService.CreateJWT(user),
                IsManager = isManager
            };
        }
        private async Task<bool> CheckEmailExistAsync(string email)
        {
            return await _userManager.Users.AnyAsync(x => x.Email == email.ToLower());
        }
    }
}
