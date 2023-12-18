using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using webapi.Data;
using webapi.DTOs.Request;
using webapi.DTOs.Restaurant;
using webapi.Models;
using webapi.Services.ClassServices;

namespace webapi.Controllers
{
    /// <summary>
    /// Adds/Deletes/Edit a restaurant - pass a managers id
    /// assigns a employee to a restaurant - pass a managers id and an employee id
    /// Get restaurants by different elements
    /// </summary>
    public class RestaurantController : Controller
    {
        private readonly RestaurantService _restaurantService;
        private readonly OptimaRestaurantContext _context;
        public RestaurantController(RestaurantService restaurantService, 
            OptimaRestaurantContext context)
        {
            _restaurantService = restaurantService;
            _context = context;
        }

        [HttpGet("api/restaurants/get-all-restaurants")]
        public async Task<ActionResult<List<BrowseRestaurantDto>>> GetAllRestaurants()
        {
            List<BrowseRestaurantDto> restaurantsDto = await _restaurantService.GetAllRestaurants();
            if (restaurantsDto.Count == 0) return BadRequest("Няма ресторанти!");
            else return restaurantsDto;
        }

        [HttpGet("api/restaurants/get-local-restaurants/{cityName}")]
        public async Task<ActionResult<List<BrowseRestaurantDto>>> GetAllRestaurantsInACity(string cityName)
        {
            List<BrowseRestaurantDto> restaurantsDto = await _restaurantService.GetCityRestaurants(cityName);
            if (restaurantsDto.Count == 0) return BadRequest("Няма ресторанти!");
            else return restaurantsDto;
        }

        [HttpGet("api/restaurants/get-rating-restaurants/{rating}")]
        public async Task<ActionResult<List<BrowseRestaurantDto>>> GetAllRestaurantsAboveRating(decimal rating)
        {
            List<BrowseRestaurantDto> restaurantsDto = await _restaurantService.GetRatingRestaurants(rating);
            if (restaurantsDto.Count == 0) return BadRequest("Няма ресторанти!");
            else return restaurantsDto;
        }

        [HttpGet("api/restaurants/get-cuisine-restaurants")]
        public async Task<ActionResult<List<BrowseRestaurantDto>>> GetBestCuisineRestaurants()
        {
            List<BrowseRestaurantDto> restaurantsDto = await _restaurantService.GetRestaurantsByCertainRating("CuisineAverageRating");
            if (restaurantsDto.Count == 0) return BadRequest("Няма ресторанти!");
            else return restaurantsDto;
        }

        [HttpGet("api/restaurants/get-atmosphere-restaurants")]
        public async Task<ActionResult<List<BrowseRestaurantDto>>> GetBestAtmosphereRestaurants()
        {
            List<BrowseRestaurantDto> restaurantsDto = await _restaurantService.GetRestaurantsByCertainRating("AtmosphereAverageRating");
            if (restaurantsDto.Count == 0) return BadRequest("Няма ресторанти!");
            else return restaurantsDto;
        }

        [HttpGet("api/restaurants/get-employees-restaurants")]
        public async Task<ActionResult<List<BrowseRestaurantDto>>> GetBestEmployeesRestaurants()
        {
            List<BrowseRestaurantDto> restaurantsDto = await _restaurantService.GetRestaurantsByCertainRating("EmployeesAverageRating");
            if (restaurantsDto.Count == 0) return BadRequest("Няма ресторанти!");
            else return restaurantsDto;
        }

        [HttpGet("api/restaurants/restaurant-details/{restaurantId}")]
        public async Task<ActionResult<RestaurantDetailsDto>> GetRestaurantDetails(string restaurantId)
        {
            Restaurant restaurant;
            if (await _restaurantService.CheckRestaurantExistById(restaurantId)) return BadRequest("Ресторантът не съществува!");
            else restaurant = await _restaurantService.GetRestaurantById(restaurantId);

            return _restaurantService.GetRestaurantDetails(restaurant);
        }

        [HttpPost("api/restaurants/send-working-request")]
        public async Task<IActionResult> SendWorkingRequest([FromBody] NewEmployeeRequestDto requestDto)
        {
            var restaurant = await _context.Restaurants.FirstOrDefaultAsync(r => r.Id.ToString() == requestDto.RestaurantId);
            if (restaurant == null) return BadRequest("Ресторантът не съществува!");
            if (!restaurant.IsWorking) return BadRequest("Ресторантът не работи!");
            if (restaurant.EmployeeCapacity <= _context.EmployeesRestaurants
                .Where(er => er.Restaurant.Id.ToString() == requestDto.RestaurantId).Count())
                return BadRequest("Ресторантът не наема повече работници!");


            var employee = await _context.Employees.FirstOrDefaultAsync(e => e.Profile.Email == requestDto.EmployeeEmail);
            var employeeProfile = await _context.Users.FirstOrDefaultAsync(p => p.Email == requestDto.EmployeeEmail);
            if (employee == null || employeeProfile == null) return BadRequest("Потребителят не съществува!");

            if (_context.Requests.FirstOrDefault(r => r.Sender == employeeProfile && r.Restaurant == restaurant && r.SentOn.AddDays(7) > DateTime.UtcNow) != null) return BadRequest("Вие вече сте изпратили заявка към този ресторант!");
            if (restaurant.EmployeesRestaurants.FirstOrDefault(er => er.Employee == employee && er.EndedOn == null) != null) return BadRequest("Вие работите в този ресторант!");
            
            var manager = restaurant.Manager;
            if (manager == null) return BadRequest("Ресторантът няма мениджър!");
            var managerProfile = manager.Profile;

            Request request = new Request
            {
                Sender = employeeProfile,
                Receiver = managerProfile,
                Restaurant = restaurant,
                SentOn = DateTime.UtcNow,
            };

            await _context.Requests.AddAsync(request);
            await _context.SaveChangesAsync();

            return Ok(new JsonResult(new { title = "Успешно изпратена заявка!", message = $"Вашата заявка беше изпратена!" }));
        }
    }
}
