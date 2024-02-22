using Microsoft.AspNetCore.Mvc;
using webapi.DTOs.Request;
using webapi.DTOs.Restaurant;
using webapi.Models;
using webapi.Services.ClassServices;
using webapi.Services.ModelServices;

namespace webapi.Controllers
{
    /// <summary>
    /// RestaurantController manages restaurants:
    /// Implements numerous queries for restaurants
    /// Searching restaurants and allows
    /// Logged users (as employees) to send working requests
    /// </summary>

    [ApiController]
    [Route("api/[controller]")]

    public class RestaurantsController : ControllerBase
    {
        private readonly RestaurantService _restaurantService;
        private readonly EmployeeService _employeeService;
        private readonly RequestService _requestService;
        public RestaurantsController(RestaurantService restaurantService,
            EmployeeService employeeService,
            RequestService requestService)
        {
            _restaurantService = restaurantService;
            _requestService = requestService;
            _employeeService = employeeService;
        }

        [HttpGet("get-all-restaurants-count")]
        public async Task<ActionResult<int>> GetAllRestaurantsCount()
        {
            return await _restaurantService.GetAllRestaurantsCount();
        }
        [HttpGet("get-all-restaurants/{lastRestaurantIndex}")]
        public async Task<ActionResult<List<BrowseRestaurantDto>>> GetAllRestaurants(int lastRestaurantIndex)
        {
            return await _restaurantService.GetAllRestaurants(lastRestaurantIndex);
        }

        [HttpGet("get-city-names")]
        public async Task<ActionResult<List<string>>> GetCityNames()
        {
            return await _restaurantService.GetCityRestaurantNames();
        }

        [HttpGet("get-city-count/{cityName}")]
        public async Task<ActionResult<int>> GetAllRestaurantsInACity(string cityName)
        {
            return await _restaurantService.GetCityRestaurantsCount(cityName);
        }

        [HttpGet("get-city-restaurants/{cityName}/{lastRestaurantIndex}")]
        public async Task<ActionResult<List<BrowseRestaurantDto>>> GetAllRestaurantsInACity(string cityName, int lastRestaurantIndex)
        {
            return await _restaurantService.GetCityRestaurants(lastRestaurantIndex, cityName);
        }

        [HttpGet("get-cuisine-restaurants/{lastRestaurantIndex}")]
        public async Task<ActionResult<List<BrowseRestaurantDto>>> GetBestCuisineRestaurants(int lastRestaurantIndex)
        {
            return await _restaurantService.GetRestaurantsByCertainRating(lastRestaurantIndex, "CuisineAverageRating");
        }

        [HttpGet("get-atmosphere-restaurants/{lastRestaurantIndex}")]
        public async Task<ActionResult<List<BrowseRestaurantDto>>> GetBestAtmosphereRestaurants(int lastRestaurantIndex)
        {
            return await _restaurantService.GetRestaurantsByCertainRating(lastRestaurantIndex, "AtmosphereAverageRating");
        }

        [HttpGet("get-employees-restaurants/{lastRestaurantIndex}")]
        public async Task<ActionResult<List<BrowseRestaurantDto>>> GetBestEmployeesRestaurants(int lastRestaurantIndex)
        {
            return await _restaurantService.GetRestaurantsByCertainRating(lastRestaurantIndex, "EmployeesAverageRating");
        }

        [HttpGet("get-best-restaurants/{lastRestaurantIndex}")]
        public async Task<ActionResult<List<BrowseRestaurantDto>>> GetBestRestaurants(int lastRestaurantIndex)
        {
            return await _restaurantService.GetRestaurantsByCertainRating(lastRestaurantIndex, "RestaurantAverageRating");
        }

        [HttpGet("restaurant-details/{restaurantId}")]
        public async Task<ActionResult<RestaurantDetailsDto>> GetRestaurantDetails(string restaurantId)
        {
            Restaurant restaurant;
            if (!await _restaurantService.CheckRestaurantExistById(restaurantId)) return BadRequest("Ресторантът не съществува!");
            else restaurant = await _restaurantService.GetRestaurantById(restaurantId);

            return _restaurantService.GetRestaurantDetails(restaurant);
        }

        [HttpGet("search/{input}")]
        public async Task<ActionResult<List<BrowseRestaurantDto>>> SearchRestaurant(string input)
        {
            List<Restaurant> foundRestaurants = await _restaurantService.GetRestaurantsWithMatchingNames(input);
            List<BrowseRestaurantDto> restaurantDtos = new List<BrowseRestaurantDto>();

            foreach (var res in foundRestaurants)
                restaurantDtos.Add(new BrowseRestaurantDto()
                {
                    Id = res.Id.ToString(),
                    Name = res.Name,
                    IconPath = res.IconPath,
                    Address1 = res.Address1,
                    Address2 = res.Address2,
                    Longitude = res.Longitude,
                    Latitude = res.Latitude,
                    IsWorking = res.IsWorking,
                    City = res.City,
                    TotalReviewsCount = res.TotalReviewsCount,
                    RestaurantAverageRating = res.RestaurantAverageRating ?? 0,
                });

            return restaurantDtos;
        }

        /// <summary>
        /// This method allows employees to send requests to work in a restaurant
        /// Authentication guard in angularapp only lets employees to use this method
        /// </summary>
        [HttpPost("send-working-request")]
        public async Task<IActionResult> SendWorkingRequest([FromBody] NewEmployeeRequestDto requestDto)
        {
            Employee employee;
            if (!await _employeeService.CheckEmployeeExistByEmail(requestDto.EmployeeEmail)) return BadRequest("Потребителят не съществува");
            else employee = await _employeeService.GetEmployeeByEmail(requestDto.EmployeeEmail);

            Restaurant restaurant;
            if (!await _restaurantService.CheckRestaurantExistById(requestDto.RestaurantId)) return BadRequest("Ресторантът не съществува!");
            else restaurant = await _restaurantService.GetRestaurantById(requestDto.RestaurantId);
            if (!restaurant.IsWorking) return BadRequest("Ресторантът не работи!");
            if (_restaurantService.IsRestaurantAtMaxCapacity(restaurant)) return BadRequest("Ресторантът не наема повече работници!");

            if (await _requestService.IsRequestAlreadySent(employee.Profile, restaurant)) return BadRequest("Вашата заявка не е отговорена или скоро е била отхвърлена! Моля опитайте пак по-късно!");
            if (_requestService.IsEmployeeAlreadyWorkingInRestaurant(employee, restaurant)) return BadRequest("Вие работите в този ресторант!");

            if (_restaurantService.HasRestaurantAManager(restaurant)) return BadRequest("Ресторантът няма мениджър!");

            await _requestService.AddRequest(employee, restaurant, true);
            await _restaurantService.SaveChangesAsync();

            return Ok(new JsonResult(new { title = "Успешно изпратена заявка!", message = $"Вашата заявка беше изпратена!" }));
        }
    }
}
