using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using System.Text;
using webapi.Data;
using webapi.DTOs.Account;
using webapi.DTOs.Restaurant;
using webapi.DTOs.Review;
using webapi.Models;
using webapi.Services;

namespace webapi.Controllers
{
    public class ReviewController : Controller
    {
        private readonly JWTService _jwtService;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly EmailService _emailService;
        private readonly IConfiguration _configuration;
        private readonly OptimaRestaurantContext _context;
        private readonly QrCodesService _qrCodesService;

        public ReviewController(JWTService jwtService,
            SignInManager<ApplicationUser> signInManager,
            UserManager<ApplicationUser> userManager,
            EmailService emailService,
            IConfiguration configuration,
            OptimaRestaurantContext context,
            QrCodesService qrCodesService)
        {
            _jwtService = jwtService;
            _signInManager = signInManager;
            _userManager = userManager;
            _emailService = emailService;
            _configuration = configuration;
            _context = context;
            _qrCodesService = qrCodesService;
        }


        [HttpGet("api/review-employee/{email}/{token}")]
        public async Task<ActionResult<ReviewDto>> GetReviewForm(string email, string token)
        {
            var employee = await _context.Employees.FirstOrDefaultAsync(e => e.Profile.Email == email);
            if (employee == null) return Unauthorized("Този имейл не е регистриран в системата.");

            var isValidToken = _jwtService.ValidateQrToken(token, email);

            if (!isValidToken)
            {
                return BadRequest("Невалиден токен. Моля, опитайте отново");
            }

            ICollection<BrowseRestaurantDto> restaurants = new List<BrowseRestaurantDto>();

            foreach (var restaurant in employee.EmployeesRestaurants
                .Where(er => !er.EndedOn.HasValue)
                .Select(er => er.Restaurant)
                .Where(r => r.IsWorking))
            {
                restaurants.Add(new BrowseRestaurantDto
                {
                    Id = restaurant.Id.ToString(),
                    Name = restaurant.Name,
                    Address = restaurant.Address,
                    City = restaurant.City,
                    IsWorking = restaurant.IsWorking,
                    RestaurantAverageRating = restaurant.RestaurantAverageRating,
                    IconUrl = restaurant?.IconPath
                });
            }

            ReviewDto reviewDto = new ReviewDto 
            { 
                JwtToken = token, 
                EmployeeEmail = email,
                RestaurantDtos = restaurants.ToList()
            };
            return reviewDto;
        }

        [HttpPost("api/review-employee")]
        public async Task<IActionResult> SubmitReview([FromBody] CustomerReviewDto model)
        {
            var employee = await _context.Employees.FirstOrDefaultAsync(e => e.Profile.Email == model.EmployeeEmail);
            if (employee == null) return BadRequest("Потребителят не съществува!");

            var restaurant = await _context.Restaurants.FirstOrDefaultAsync(e => e.Id.ToString() == model.RestaurantId);
            if (restaurant == null) return BadRequest("Ресторантът не съществува!");

            CustomerReview review = new CustomerReview()
            {
                Employee = employee,
                Restaurant = restaurant,
                DateTime = DateTime.UtcNow,
                Comment = model.Comment,
                SpeedRating = model.SpeedRating,
                AttitudeRating = model.AttitudeRating,
                AtmosphereRating = model.AtmosphereRating,
                CuisineRating = model.CuisineRating,
            };

            // implement calculations for the models

            return Ok(new JsonResult(new { title = "Успешно запаметено ревю!", message = "Благодарим Ви за отделеното време! Вашето ревю беше запаметено успешно!" }));
        }
    }
}
