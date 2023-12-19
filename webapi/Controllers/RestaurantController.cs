using Microsoft.AspNetCore.Mvc;
using webapi.DTOs.Request;
using webapi.DTOs.Restaurant;
using webapi.Models;
using webapi.Services.ClassServices;
using webapi.Services.ModelServices;

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
        private readonly EmployeeService _employeeService;
        private readonly RequestService _requestService;
        public RestaurantController(RestaurantService restaurantService,
            EmployeeService employeeService,
            RequestService requestService)
        {
            _restaurantService = restaurantService;
            _requestService = requestService;
            _employeeService = employeeService;
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

        /* Auth guard should allow that only to logged users */
        [HttpPost("api/restaurants/send-working-request")]
        public async Task<IActionResult> SendWorkingRequest([FromBody] NewEmployeeRequestDto requestDto)
        {
            Employee employee;
            if (!await _employeeService.CheckEmployeeExistByEmail(requestDto.EmployeeEmail)) return BadRequest("Потребителят не съществува");
            else employee = await _employeeService.GetEmployeeByEmail(requestDto.EmployeeEmail);

            Restaurant restaurant;
            if (await _restaurantService.CheckRestaurantExistById(requestDto.RestaurantId)) return BadRequest("Ресторантът не съществува!");
            else restaurant = await _restaurantService.GetRestaurantById(requestDto.RestaurantId);
            if (!restaurant.IsWorking) return BadRequest("Ресторантът не работи!");
            if (_restaurantService.IsRestaurantAtMaxCapacity(restaurant)) return BadRequest("Ресторантът не наема повече работници!");

            if (await _requestService.IsRequestAlreadySent(employee.Profile, restaurant)) return BadRequest("Вие вече сте изпратили заявка към този ресторант!");
            if (_requestService.IsEmployeeAlreadyWorkingInRestaurant(employee, restaurant)) return BadRequest("Вие работите в този ресторант!");

            if (_restaurantService.HasRestaurantAManager(restaurant)) return BadRequest("Ресторантът няма мениджър!");

            await _requestService.AddRequest(employee, restaurant);
            await _restaurantService.SaveChangesAsync();

            return Ok(new JsonResult(new { title = "Успешно изпратена заявка!", message = $"Вашата заявка беше изпратена!" }));
        }
    }
}
