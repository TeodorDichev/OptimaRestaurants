using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using System.Text;
using webapi.DTOs.Account;
using webapi.Models;
using webapi.Services;
using webapi.Services.ClassServices;

namespace webapi.Controllers
{
    /// <summary>
    /// AccountController manages all accounts:
    /// Registration, accounts' password and email, searching accounts
    /// </summary>

    public class AccountController : Controller
    {
        private readonly JWTService _jwtService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly EmailService _emailService;
        private readonly IConfiguration _configuration;
        private readonly AccountService _accountService;
        private readonly ManagerService _managerService;
        private readonly EmployeeService _employeeService;

        public AccountController(JWTService jwtService,
        UserManager<ApplicationUser> userManager,
        EmailService emailService,
        IConfiguration configuration,
        AccountService accountService,
        ManagerService managerService,
        EmployeeService employeeService)
        {
            _jwtService = jwtService;
            _userManager = userManager;
            _emailService = emailService;
            _configuration = configuration;
            _accountService = accountService;
            _managerService = managerService;
            _employeeService = employeeService;
        }

        [HttpPost("api/account/register-manager")]
        public async Task<ActionResult<ApplicationUserDto>> RegisterManager([FromBody] RegisterManagerDto model)
        {
            ApplicationUser userToAdd;
            if (await _accountService.CheckUserExistByEmail(model.Email)) return BadRequest($"Вече съществува акаунт с този имейл адрес!");
            else userToAdd = await _accountService.AddApplicationUser(model.FirstName, model.LastName, model.Email, model.Password);

            try
            {
                if (await SendConfirmEmailAddress(userToAdd))
                {
                    await _managerService.AddManager(userToAdd);
                    await _accountService.SaveChangesAsync();
                    return await CreateApplicationUserDto(userToAdd);
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
            ApplicationUser userToAdd;
            if (await _accountService.CheckUserExistByEmail(model.Email)) return BadRequest($"Вече съществува акаунт с този имейл адрес!");
            else userToAdd = await _accountService.AddApplicationUser(model.FirstName, model.LastName, model.Email, model.Password);

            try
            {
                if (await SendConfirmEmailAddress(userToAdd))
                {
                    await _employeeService.AddEmployee(userToAdd, model.City, model.BirthDate);
                    await _accountService.SaveChangesAsync();
                    return await CreateApplicationUserDto(userToAdd);
                }
                else return BadRequest("Неуспешно изпращане на имейл. Моля свържете се с администратор.");
            }
            catch (Exception)
            {
                return BadRequest("Неуспешно изпращане на имейл. Моля свържете се с администратор.");
            }
        }

        [HttpPost("api/account/login")]
        public async Task<ActionResult<ApplicationUserDto>> Login([FromBody] LoginDto model)
        {
            ApplicationUser user;
            if (! await _accountService.CheckUserExistByEmail(model.UserName)) return Unauthorized("Грешен имейл или парола!");
            else user = await _accountService.GetUserByEmailOrUserName(model.UserName);

            if (user.EmailConfirmed == false) return Unauthorized("Моля потвърдете имейл адреса си.");

            if (!await _accountService.CheckPasswordAsync(user, model.Password)) return Unauthorized("Грешен имейл или парола!");

            return await CreateApplicationUserDto(user);
        }

        [HttpGet("/api/account/refresh-user-token/{email}")]
        public async Task<ActionResult<ApplicationUserDto>> RefreshUserToken(string email)
        {
            ApplicationUser user;
            if (!await _accountService.CheckUserExistByEmail(email)) return Unauthorized("Потребителят не съществува!");
            else user = await _accountService.GetUserByEmailOrUserName(email);

            return await CreateApplicationUserDto(user);
        }

        [HttpPut("api/account/confirm-email")]
        public async Task<IActionResult> ConfirmEmail([FromBody] ConfirmEmailDto model)
        {
            ApplicationUser user;
            if (! await _accountService.CheckUserExistByEmail(model.Email)) return Unauthorized("Този имейл не е регистриран в системата.");
            else user = await _accountService.GetUserByEmailOrUserName(model.Email);

            if (user.EmailConfirmed == true) return BadRequest("Този имейл вече е потвърден!");

            try
            {
                var decodedTokenBytes = WebEncoders.Base64UrlDecode(model.Token);
                var decodedToken = Encoding.UTF8.GetString(decodedTokenBytes);

                var result = await _userManager.ConfirmEmailAsync(user, decodedToken);
                if (result.Succeeded)
                {
                    return Ok(new JsonResult(new { title = "Имейлът успешно потвърден!", message = "Вашият акаунт беше създаден!" }));
                }

                return BadRequest("Невалиден токен. Моля, опитайте отново");
            }
            catch (Exception)
            {
                return BadRequest("Невалиден токен. Моля, опитайте отново");
            }
        }

        [HttpPut("api/account/reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto model)
        {
            ApplicationUser user;
            if (!await _accountService.CheckUserExistByEmail(model.Email)) return Unauthorized("Този имейл не е регистриран в системата.");
            else user = await _accountService.GetUserByEmailOrUserName(model.Email);

            if (user.EmailConfirmed == false) return BadRequest("Имейлът ви не е потвърден, моля потвърдете го!");

            try
            {
                var decodedTokenBytes = WebEncoders.Base64UrlDecode(model.Token);
                var decodedToken = Encoding.UTF8.GetString(decodedTokenBytes);

                var result = await _userManager.ResetPasswordAsync(user, decodedToken, model.Password);
                if (result.Succeeded)
                {
                    return Ok(new JsonResult(new { title = "Нулирането на паролата е успешно!", message = "Вашата парола е сменена успешно!" }));
                }

                return BadRequest("Невалиден токен. Моля, опитайте отново");
            }
            catch (Exception)
            {
                return BadRequest("Невалиден токен. Моля, опитайте отново");
            }
        }

        [HttpPost("api/account/resend-email-confirmation-link/{email}")]
        public async Task<IActionResult> ResendEmailConfirmationLink(string email)
        {
            if (string.IsNullOrEmpty(email)) return BadRequest("Невалиден имейл адрес!");
            var user = await _accountService.GetUserByEmailOrUserName(email);
            if (user == null) return Unauthorized("Този имейл не е регистриран в системата.");
            if (user.EmailConfirmed == true) return BadRequest("Този имейл вече е потвърден!");

            try
            {
                if (await SendConfirmEmailAddress(user))
                {
                    return Ok(new JsonResult(new { title = "Линкът за потвърждаване е изпратен!", message = "Моля, потвърдете имейл адреса си." }));
                }
                return BadRequest("Неуспешно изпращане на имейл. Моля свържете се с администратор.");
            }
            catch (Exception)
            {
                return BadRequest("Неуспешно изпращане на имейл. Моля свържете се с администратор.");
            }
        }

        [HttpPost("api/account/forgot-username-or-password/{email}")]
        public async Task<IActionResult> ForgotUsernameOrPassword(string email)
        {
            if (string.IsNullOrEmpty(email)) return BadRequest("Невалиден имейл адрес!");
            var user = await _accountService.GetUserByEmailOrUserName(email);
            if (user == null) return Unauthorized("Този имейл не е регистриран в системата.");
            if (user.EmailConfirmed == false) return BadRequest("Моля потвърдете имейл адреса си.");

            try
            {
                if (await SendForgotUsernameOrPassword(user))
                {
                    return Ok(new JsonResult(new { title = "Имейлът за подновяване на паролата е изпратен!", message = "Моля, проверете имейл адреса си." }));
                }

                return BadRequest("Неуспешно изпращане на имейл. Моля свържете се с администратор.");
            }
            catch (Exception)
            {
                return BadRequest("Неуспешно изпращане на имейл. Моля свържете се с администратор.");
            }
        }

        [HttpGet("api/account/search/{input}")]
        public async Task<ActionResult<List<SearchedAccountDto>>> SearchAccount(string input)
        {
            List<ApplicationUser> foundUsers = await _accountService.GetUsersWithMatchingData(input);
            List<SearchedAccountDto> accounts = new List<SearchedAccountDto>();

            foreach (var user in foundUsers)
                accounts.Add(new SearchedAccountDto()
                {
                    Fullname = user.FirstName + " " + user.LastName,
                    Email = user.Email,
                    Role = string.Join(" ", await _userManager.GetRolesAsync(user)),
                    PicturePath = user.ProfilePicturePath
                });

            return accounts;
        }

        private async Task<bool> SendForgotUsernameOrPassword(ApplicationUser user)
        {
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            token = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));
            var url = $"{_configuration["JWT:ClientUrl"]}{_configuration["Email:ResetPasswordPath"]}?token={token}&email={user.Email}";

            var body = $"<p>Здравейте: {user.FirstName} {user.LastName}</p>" +
                $"<p>Имейл: {user.UserName}.</p>" +
                "<p>Може да подновите паролата си тук.</p>" +
                $"<p><a href=\"{url}\">Подновяване</a></p>" +
                "<p>Благодарим ви,</p>" +
                $"<br>{_configuration["Email:ApplicationName"]}";

            var emailSend = new EmailSendDto(user.Email ?? string.Empty, body, "Forgot username or password");

            return await _emailService.SendEmailAsync(emailSend);
        }

        private async Task<bool> SendConfirmEmailAddress(ApplicationUser user)
        {
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            token = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));
            var url = $"{_configuration["JWT:ClientUrl"]}{_configuration["Email:ConfirmEmailPath"]}?token={token}&email={user.Email}";

            var body = $"<p>Здравейте: {user.FirstName} {user.LastName}</p>" +
                "<p>Моля, потвърдетe имейл адреса си тук:</p>" +
                $"<p><a href=\"{url}\">Потвърждаване</a></p>" +
                "<p>Благодарим ви,</p>" +
                $"<br>{_configuration["Email:ApplicationName"]}";

            var emailSend = new EmailSendDto(user.Email ?? string.Empty, body, "Confirm your email");

            return await _emailService.SendEmailAsync(emailSend);
        }

        private async Task<ApplicationUserDto> CreateApplicationUserDto(ApplicationUser user)
        {
            bool isManager = false;
            if (await _managerService.IsUserManager(user)) isManager = true;

            return new ApplicationUserDto
            {
                Email = user.Email ?? string.Empty,
                JWT = _jwtService.CreateJWT(user),
                IsManager = isManager
            };
        }
    }
}
