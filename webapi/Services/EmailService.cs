using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using SendGrid;
using SendGrid.Helpers.Mail;
using System.Text;
using webapi.Models;

namespace webapi.Services
{
    /// <summary>
    /// The service enables email sending by using the SendGrid API
    /// </summary>

    public class EmailService
    {
        private readonly IConfiguration _configuration;
        private readonly UserManager<ApplicationUser> _userManager;
        public EmailService(IConfiguration configuration,
            UserManager<ApplicationUser> userManager)
        {
            _configuration = configuration;
            _userManager = userManager;
        }
        public async Task<bool> SendConfirmEmailAsync(ApplicationUser user)
        {
            try
            {
                var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                token = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));
                var url = $"{_configuration["JWT:ClientUrl"]}{_configuration["Email:ConfirmEmailPath"]}?token={token}&email={user.Email}";

                var apiKey = _configuration["Email:Key"];
                var client = new SendGridClient(apiKey);
                var from = new EmailAddress(_configuration["Email:From"]);
                var subject = "Потвърждаване на имейл адрес";
                var to = new EmailAddress(user.Email);
                var plainTextContent = "Потвърждаване на имейл адрес";
                var body = $"<p>Здравейте: {user.FirstName} {user.LastName}</p>" +
                    "<p>Моля, потвърдетe имейл адреса си тук:</p>" +
                    $"<p><a href=\"{url}\">Потвърждаване</a></p>" +
                    "<p>Благодарим ви,,</p>" +
                    $"<br>{_configuration["Email:ApplicationName"]}";

                var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, body);
                var response = await client.SendEmailAsync(msg);

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public async Task<bool> SendResetPasswordAsync(ApplicationUser user, string tempPassword)
        {
            try
            {
                var apiKey = _configuration["Email:Key"];
                var client = new SendGridClient(apiKey);
                var from = new EmailAddress(_configuration["Email:From"]);
                var subject = "Смяна на забравена парола";
                var to = new EmailAddress(user.Email);
                var plainTextContent = "Смяна на забравена парола";
                var body = $"<p>Здравейте: {user.FirstName} {user.LastName}</p>" +
                    $"<p>Имейл: {user.UserName}.</p>" +
                    "<p>Това е новата парола, с която може да влезете в профила си.</p>" +
                    $"<p>{tempPassword}</p>" +
                    "<p>Благодарим ви,</p>" +
                    $"<br>{_configuration["Email:ApplicationName"]}";

                var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, body);
                var response = await client.SendEmailAsync(msg);

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
