using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using webapi.Data;
using webapi.DTOs.Restaurant;
using webapi.DTOs.Review;
using webapi.Models;
using webapi.Services;

namespace webapi.Controllers
{
    public class ReviewController : Controller
    {
        private readonly JWTService _jwtService;
        private readonly OptimaRestaurantContext _context;

        public ReviewController(JWTService jwtService,
            OptimaRestaurantContext context)
        {
            _jwtService = jwtService;
            _context = context;
        }


        [HttpGet("api/review-employee/{email}/{token}")]
        public async Task<ActionResult<ReviewDto>> GetCustomerReviewForm(string email, string token)
        {
            var employee = await _context.Employees.FirstOrDefaultAsync(e => e.Profile.Email == email);
            if (employee == null) return Unauthorized("Този имейл не е регистриран в системата.");

            var isValidToken = _jwtService.ValidateQrToken(token, email);

            if (!isValidToken) return BadRequest("Невалиден токен. Моля, опитайте отново");

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
                    RestaurantAverageRating = restaurant.RestaurantAverageRating ?? 0,
                    IconPath = restaurant?.IconPath
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
        public async Task<IActionResult> SubmitCustomerReview([FromBody] CustomerReviewDto model)
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

            await _context.CustomerReviews.AddAsync(review);
            await _context.SaveChangesAsync();

            if (UpdateStatistics(employee, restaurant))
                return Ok(new JsonResult(new { title = "Успешно запаметено ревю!", message = "Благодарим Ви за отделеното време! Вашето ревю беше запаметено успешно!" }));
            else
                return BadRequest("Неуспешно обновени данни!");
        }

        private bool UpdateStatistics(Employee employee, Restaurant restaurant)
        {
            try
            {
                //updating employee statistics
                employee.AttitudeAverageRating = _context.CustomerReviews.Where(cr => cr.Employee == employee && cr.AttitudeRating != null)
                    .Select(cr => cr.AttitudeRating)
                    .Average();

                employee.SpeedAverageRating = _context.CustomerReviews.Where(cr => cr.Employee == employee && cr.SpeedRating != null)
                    .Select(cr => cr.SpeedRating)
                    .Average();

                employee.EmployeeAverageRating = (employee.PunctualityAverageRating
                    + employee.AttitudeAverageRating
                    + employee.CollegialityAverageRating
                    + employee.SpeedAverageRating) / 4;

                //updating restaurants statistics
                restaurant.AtmosphereAverageRating = _context.CustomerReviews.Where(cr => cr.Restaurant == restaurant && cr.AtmosphereRating != null)
                    .Select(cr => cr.AtmosphereRating)
                    .Average();

                restaurant.CuisineAverageRating = _context.CustomerReviews.Where(cr => cr.Restaurant == restaurant && cr.CuisineRating != null)
                    .Select(cr => cr.CuisineRating)
                    .Average();

                restaurant.EmployeesAverageRating = restaurant.EmployeesRestaurants
                    .Select(er => er.Employee)
                    .Select(e => e.EmployeeAverageRating)
                    .Average();

                restaurant.RestaurantAverageRating = (restaurant.CuisineAverageRating
                    + restaurant.AtmosphereAverageRating
                    + restaurant.EmployeesAverageRating) / 3;

                //saving the changes
                _context.Employees.Update(employee);
                _context.Restaurants.Update(restaurant);
                _context.SaveChanges();

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
