using Microsoft.AspNetCore.Mvc;
using webapi.DTOs.Restaurant;
using webapi.DTOs.Review;
using webapi.Models;
using webapi.Services;
using webapi.Services.ClassServices;

namespace webapi.Controllers
{
    public class ReviewController : Controller
    {
        private readonly JWTService _jwtService;
        private readonly ReviewService _reviewService;
        private readonly EmployeeService _employeeService;
        private readonly RestaurantService _restaurantService;

        public ReviewController(JWTService jwtService,
            ReviewService reviewService,
            EmployeeService employeeService,
            RestaurantService restaurantService)
        {
            _jwtService = jwtService;
            _reviewService = reviewService;
            _employeeService = employeeService;
            _restaurantService = restaurantService;
        }


        [HttpGet("api/review-employee/{email}/{token}")]
        public async Task<ActionResult<ReviewDto>> GetCustomerReviewForm(string email, string token)
        {
            Employee employee;
            if (!await _employeeService.CheckEmployeeExistByEmail(email)) return BadRequest("Потребителят не съществува");
            else employee = await _employeeService.GetEmployeeByEmail(email);

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
                EmployeeName = employee.Profile.FirstName + " " + employee.Profile.LastName,
                RestaurantDtos = restaurants.ToList()
            };
            return reviewDto;
        }

        [HttpPost("api/review-employee")]
        public async Task<IActionResult> SubmitCustomerReview([FromBody] CustomerReviewDto model)
        {
            Restaurant restaurant;
            if (await _restaurantService.CheckRestaurantExistById(model.RestaurantId)) return BadRequest("Ресторантът не съществува!");
            else restaurant = await _restaurantService.GetRestaurantById(model.RestaurantId);

            Employee employee;
            if (!await _employeeService.CheckEmployeeExistByEmail(model.EmployeeEmail)) return BadRequest("Потребителят не съществува");
            else employee = await _employeeService.GetEmployeeByEmail(model.EmployeeEmail);

            await _reviewService.AddCustomerReview(restaurant, employee, model);
            await _reviewService.SaveChangesAsync();

            if (_reviewService.UpdateStatistics(employee, restaurant))
                return Ok(new JsonResult(new { title = "Успешно запаметено ревю!", message = "Благодарим Ви за отделеното време! Вашето ревю беше запаметено успешно!" }));
            else
                return BadRequest("Неуспешно обновени данни!");
        }
        [HttpPost("api/manager/review-employee")]
        public async Task<IActionResult> SubmitManagerReview([FromBody] ManagerReviewDto model)
        {
            Restaurant restaurant;
            if (await _restaurantService.CheckRestaurantExistById(model.RestaurantId)) return BadRequest("Ресторантът не съществува!");
            else restaurant = await _restaurantService.GetRestaurantById(model.RestaurantId);

            Employee employee;
            if (!await _employeeService.CheckEmployeeExistByEmail(model.EmployeeEmail)) return BadRequest("Потребителят не съществува");
            else employee = await _employeeService.GetEmployeeByEmail(model.EmployeeEmail);


            if (employee.EmployeesRestaurants.Select(er => er.EndedOn != null && er.Restaurant == restaurant) != null) return BadRequest("Потребителят не работи за вас!");

            await _reviewService.AddManagerReview(restaurant, employee, model);
            await _reviewService.SaveChangesAsync();

            if (_reviewService.UpdateStatistics(employee, restaurant))
                return Ok(new JsonResult(new { title = "Успешно запаметено ревю!", message = "Благодарим Ви за отделеното време! Вашето ревю беше запаметено успешно!" }));
            else
                return BadRequest("Неуспешно обновени данни!");
        }
    }
}
