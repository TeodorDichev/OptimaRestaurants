using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using System.Text;
using webapi.DTOs.Account;
using webapi.Models;
using webapi.Services;
using webapi.Services.ClassServices;
using webapi.Services.FileServices;

namespace webapi.Controllers
{
    /// <summary>
    /// AccountController manages all accounts:
    /// Registration, accounts' password and email, searching accounts
    /// </summary>

    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : ControllerBase
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

        [HttpPost("register-manager")]
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
                else return BadRequest("Акаунтът Ви е създаден, но не успяхме да изпратим имейл. Ако не можете да си влезете в профила, моля свържете се с администратор.");
            }
            catch (Exception)
            {
                return BadRequest("Неуспешно изпращане на имейл. Моля свържете се с администратор.");
            }
        }

        [HttpPost("register-employee")]
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
                else return BadRequest("Акаунтът Ви е създаден, но не успяхме да изпратим имейл. Ако не можете да си влезете в профила, моля свържете се с администратор.");
            }
            catch (Exception)
            {
                return BadRequest("Неуспешно изпращане на имейл. Моля свържете се с администратор.");
            }
        }

        [HttpPost("login")]
        public async Task<ActionResult<ApplicationUserDto>> Login([FromBody] LoginDto model)
        {
            ApplicationUser user;
            if (!await _accountService.CheckUserExistByEmail(model.UserName)) return Unauthorized("Грешен имейл или парола!");
            else user = await _accountService.GetUserByEmailOrUserName(model.UserName);

            if (user.EmailConfirmed == false) return Unauthorized("Моля потвърдете имейл адреса си.");

            if (!await _accountService.CheckPasswordAsync(user, model.Password)) return Unauthorized("Грешен имейл или парола!");

            return await CreateApplicationUserDto(user);
        }

        [HttpGet("refresh-user-token/{email}")]
        public async Task<ActionResult<ApplicationUserDto>> RefreshUserToken(string email)
        {
            ApplicationUser user;
            if (!await _accountService.CheckUserExistByEmail(email)) return Unauthorized("Потребителят не съществува!");
            else user = await _accountService.GetUserByEmailOrUserName(email);

            return await CreateApplicationUserDto(user);
        }

        [HttpPut("confirm-email")]
        public async Task<IActionResult> ConfirmEmail([FromBody] ConfirmEmailDto model)
        {
            ApplicationUser user;
            if (!await _accountService.CheckUserExistByEmail(model.Email)) return Unauthorized("Този имейл не е регистриран в системата.");
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

        [HttpPut("reset-password/{email}")]
        public async Task<IActionResult> ResetPassword(string? email)
        {
            if (string.IsNullOrEmpty(email)) return BadRequest("Моля, първо, въведете вашия имейл адрес.");

            ApplicationUser user;
            if (!await _accountService.CheckUserExistByEmail(email)) return Unauthorized("Този имейл не е регистриран в системата.");
            else user = await _accountService.GetUserByEmailOrUserName(email);

            if (user.EmailConfirmed == false) return BadRequest("Имейлът ви не е потвърден, моля потвърдете го!");

            try
            {
                string tempPassword = GeneratePassword();

                await _userManager.RemovePasswordAsync(user);
                await _userManager.AddPasswordAsync(user, tempPassword);
                await SendResetPassword(user, tempPassword);

                return Ok(new JsonResult(new { title = "Нулирането на паролата е успешно!", message = "Изпратихме ви имейл с нова парола, с която да влезете във вашия профил." }));
            }
            catch (Exception)
            {
                return BadRequest("Неуспешно сменена парола. Моля, опитайте отново!");
            }
        }

        [HttpGet("search/{input}")]
        public async Task<ActionResult<List<SearchedAccountDto>>> SearchAccount(string input)
        {
            List<ApplicationUser> foundUsers = await _accountService.GetUsersWithMatchingData(input);
            List<SearchedAccountDto> accounts = new List<SearchedAccountDto>();

            foreach (var user in foundUsers)
                accounts.Add(new SearchedAccountDto()
                {
                    FullName = user.FirstName + " " + user.LastName,
                    Email = user.Email,
                    Role = string.Join(" ", await _userManager.GetRolesAsync(user)),
                    PicturePath = user.ProfilePicturePath
                });

            return accounts;
        }

        private async Task<bool> SendResetPassword(ApplicationUser user, string tempPassword)
        {
            var body = $"<p>Здравейте: {user.FirstName} {user.LastName}</p>" +
                $"<p>Имейл: {user.UserName}.</p>" +
                "<p>Това е новата парола, с която може да влезете в профила си.</p>" +
                $"<p>{tempPassword}</p>" +
                "<p>Благодарим ви,</p>" +
                $"<br>{_configuration["Email:ApplicationName"]}";

            var emailSend = new EmailSendDto(user.Email ?? string.Empty, body, "Reset password");

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

        private const string SpecialChars = "!@#$%^&*()-_=+";
        private const string Numbers = "0123456789";
        private const string CapitalLetters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        private const string LowercaseLetters = "abcdefghijklmnopqrstuvwxyz";

        private readonly Random _random = new Random();

        private string GeneratePassword()
        {
            string password = "";
            password += SpecialChars[_random.Next(SpecialChars.Length)];
            password += Numbers[_random.Next(Numbers.Length)];
            password += CapitalLetters[_random.Next(CapitalLetters.Length)];
            for (int i = 0; i < 5; i++)
            {
                password += LowercaseLetters[_random.Next(LowercaseLetters.Length)];
            }
            password = ShuffleString(password);
            return password;
        }

        private string ShuffleString(string str)
        {
            char[] array = str.ToCharArray();
            int n = array.Length;
            while (n > 1)
            {
                n--;
                int k = _random.Next(n + 1);
                char value = array[k];
                array[k] = array[n];
                array[n] = value;
            }
            return new string(array);
        }
    }
}
