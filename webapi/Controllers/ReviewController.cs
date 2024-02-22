using Microsoft.AspNetCore.Mvc;
using webapi.DTOs.Restaurant;
using webapi.DTOs.Review;
using webapi.Models;
using webapi.Services;
using webapi.Services.ClassServices;

namespace webapi.Controllers
{
    /// <summary>
    /// ReviewController manages reviews:
    /// Submitting customer and manager reviews,
    /// Retrieving employee review history
    /// </summary>

    [ApiController]
    [Route("api/[controller]")]

    public class ReviewsController : ControllerBase
    {
        private readonly JWTService _jwtService;
        private readonly ReviewService _reviewService;
        private readonly EmployeeService _employeeService;
        private readonly RestaurantService _restaurantService;

        public ReviewsController(JWTService jwtService,
            ReviewService reviewService,
            EmployeeService employeeService,
            RestaurantService restaurantService)
        {
            _jwtService = jwtService;
            _reviewService = reviewService;
            _employeeService = employeeService;
            _restaurantService = restaurantService;
        }


        [HttpGet("review-employee/{email}/{token}")]
        public async Task<ActionResult<ReviewDto>> GetCustomerReviewForm(string email, string token)
        {
            Employee employee;
            if (!await _employeeService.CheckEmployeeExistByEmail(email)) return BadRequest("Потребителят не съществува");
            else employee = await _employeeService.GetEmployeeByEmail(email);

            if (!_jwtService.ValidateQrToken(token, email)) return BadRequest("Невалиден токен. Моля, опитайте отново");

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
                    Address1 = restaurant.Address1,
                    Address2 = restaurant.Address2,
                    Longitude = restaurant.Longitude,
                    Latitude = restaurant.Latitude,
                    City = restaurant.City,
                    IsWorking = restaurant.IsWorking,
                    TotalReviewsCount = restaurant.TotalReviewsCount,
                    RestaurantAverageRating = restaurant.RestaurantAverageRating ?? 0,
                    IconPath = restaurant?.IconPath
                });
            }

            ReviewDto reviewDto = new ReviewDto
            {
                JwtToken = token,
                EmployeeEmail = email,
                EmployeeName = employee.Profile.FirstName + " " + employee.Profile.LastName,
                RestaurantDtos = restaurants.ToList()
            };
            return reviewDto;
        }
        [HttpPost("review-employee")]
        public async Task<IActionResult> SubmitCustomerReview([FromBody] CustomerReviewDto model)
        {
            Restaurant restaurant;
            if (!await _restaurantService.CheckRestaurantExistById(model.RestaurantId)) return BadRequest("Ресторантът не съществува!");
            else restaurant = await _restaurantService.GetRestaurantById(model.RestaurantId);

            Employee employee;
            if (!await _employeeService.CheckEmployeeExistByEmail(model.EmployeeEmail)) return BadRequest("Потребителят не съществува");
            else employee = await _employeeService.GetEmployeeByEmail(model.EmployeeEmail);

            await _reviewService.AddCustomerReview(restaurant, employee, model);
            await _reviewService.SaveChangesAsync();

            try
            {
                if (model.AttitudeRating.HasValue) _reviewService.UpdateAttitude(employee, model.AttitudeRating.Value);
                if (model.SpeedRating.HasValue) _reviewService.UpdateSpeed(employee, model.SpeedRating.Value);
                if (model.AtmosphereRating.HasValue) _reviewService.UpdateAtmosphere(restaurant, model.AtmosphereRating.Value);
                if (model.CuisineRating.HasValue) _reviewService.UpdateCuisine(restaurant, model.CuisineRating.Value);
                if (employee.EmployeeAverageRating.HasValue) _reviewService.UpdateRestaurantEmployeesAverage(restaurant, employee.EmployeeAverageRating.Value);
                return Ok(new JsonResult(new { title = "Успешно запаметено ревю!", message = "Благодарим Ви за отделеното време! Вашето ревю беше запаметено успешно!" }));
            }
            catch (Exception)
            {
                return BadRequest("Неуспешно обновени данни!");
            }
        }
        [HttpPost("manager/review-employee")]
        public async Task<IActionResult> SubmitManagerReview([FromBody] ManagerReviewDto model)
        {
            Restaurant restaurant;
            if (!await _restaurantService.CheckRestaurantExistById(model.RestaurantId)) return BadRequest("Ресторантът не съществува!");
            else restaurant = await _restaurantService.GetRestaurantById(model.RestaurantId);

            Employee employee;
            if (!await _employeeService.CheckEmployeeExistByEmail(model.EmployeeEmail)) return BadRequest("Потребителят не съществува");
            else employee = await _employeeService.GetEmployeeByEmail(model.EmployeeEmail);

            if (! _restaurantService.GetEmployeesOfRestaurant(restaurant).Any(e => e.Profile.Email == model.EmployeeEmail))
                return BadRequest("Потребителят не работи за вас!");

            await _reviewService.AddManagerReview(restaurant, employee, model);
            await _reviewService.SaveChangesAsync();

            try
            {
                if (model.CollegialityRating.HasValue) _reviewService.UpdateCollegiality(employee, model.CollegialityRating.Value);
                if (model.PunctualityRating.HasValue) _reviewService.UpdatePunctuality(employee, model.PunctualityRating.Value);
                if (employee.EmployeeAverageRating.HasValue) _reviewService.UpdateRestaurantEmployeesAverage(restaurant, employee.EmployeeAverageRating.Value);
                await _reviewService.SaveChangesAsync();
                return Ok(new JsonResult(new { title = "Успешно запаметено ревю!", message = "Благодарим Ви за отделеното време! Вашето ревю беше запаметено успешно!" }));
            }
            catch (Exception)
            {
                return BadRequest("Неуспешно обновени данни!");
            }
        }

        /// <summary>
        /// GetEmployeeReviewsHistory is here because the review 
        /// history of employee should be accessible from multiple 
        /// location including all types of users
        /// </summary>
        [HttpGet("get-reviews-history/{email}")]
        public async Task<ActionResult<List<OldReviewDto>>> GetEmployeeReviewsHistory(string email)
        {
            Employee employee;
            if (!await _employeeService.CheckEmployeeExistByEmail(email)) return BadRequest("Потребителят не съществува");
            else employee = await _employeeService.GetEmployeeByEmail(email);

            return _reviewService.GetEmployeeReviewsHistory(employee);
        }
    }
}
