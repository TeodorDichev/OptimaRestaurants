using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using webapi.Data;
using webapi.DTOs.Employee;
using webapi.DTOs.Restaurant;
using webapi.Models;

namespace webapi.Controllers
{
    /// <summary>
    /// Adds/Deletes/Edit a restaurant - pass a managers id
    /// assigns a employee to a restaurant - pass a managers id and an employee id
    /// Get restaurants by different elements
    /// </summary>
    public class RestaurantController : Controller
    {
        private readonly OptimaRestaurantContext _context;
        public RestaurantController(OptimaRestaurantContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Restaurant>> GetAllRestaurantsByNameAsync(string name)
        {
            return await _context.Restaurants.Where(r => r.Name == name).OrderBy(r => r.Name).ToListAsync();
        }
        public async Task<IEnumerable<Restaurant>> GetAllRestaurantsByCityAsync(string city)
        {
            return await _context.Restaurants.Where(r => r.City == city).OrderBy(r => r.Name).ToListAsync();
        }
        public async Task<IEnumerable<Restaurant>> GetAllRestaurantsByCuisineAverageRating()
        {
            return await _context.Restaurants.OrderBy(r => r.CuisineAverageRating).ThenBy(r => r.Name).ToListAsync();
        }
        public async Task<IEnumerable<Restaurant>> GetAllRestaurantsByAtmosphereAverageRating()
        {
            return await _context.Restaurants.OrderBy(r => r.AtmosphereAverageRating).ThenBy(r => r.Name).ToListAsync();
        }
        public async Task<IEnumerable<Restaurant>> GetAllRestaurantsByEmployeesAverageRating()
        {
            return await _context.Restaurants.OrderBy(r => r.EmployeesAverageRating).ThenBy(r => r.Name).ToListAsync();
        }
        public async Task<IEnumerable<Restaurant>> GetAllRestaurantsByRestaurantAverageRating()
        {
            return await _context.Restaurants.OrderBy(r => r.RestaurantAverageRating).ThenBy(r => r.Name).ToListAsync();
        }
        [Authorize(Roles = "Manager")]
        [Authorize(Roles = "Employee")]
        public async Task<IEnumerable<Restaurant>> GetAllRestaurantsByStandardMonthlyPayment(decimal minimum = 0)
        {
            return await _context.Restaurants.OrderBy(r => r.StandardMonthlyPayment > minimum).ThenBy(r => r.Name).ToListAsync();
        }
        [Authorize(Roles = "Manager")]
        [Authorize(Roles = "Employee")]
        public async Task<IEnumerable<Restaurant>> GetAllRestaurantsWithOpenJobs()
        {
            return await _context.Restaurants
                .Where(r => r.IsWorking && (r.EmployeeCapacity - r.EmployeesRestaurants.Where(x => x.Restaurant.Id == r.Id).Count() > 0))
                .OrderBy(r => r.Name)
                .ToListAsync();
        }

        [Authorize(Roles = "Manager")]
        public ICollection<RestaurantDto> GetAllRestaurantsOfAManager(string email)
        {
            ICollection<RestaurantDto> restaurants = new List<RestaurantDto>();

            foreach (var restaurant in _context.Restaurants
                .Where(r => r.Manager.Profile.Email.ToString() == email)
                .OrderBy(r => r.Name)
                .ToList())
            {
                restaurants.Add(new RestaurantDto
                {
                    Id = restaurant.Id.ToString(),
                    Name = restaurant.Name,
                    Address = restaurant.Address,
                    City = restaurant.City,
                    AtmosphereAverageRating = restaurant?.CuisineAverageRating ?? -1, // in front-end if -1 "no reviews yet"
                    CuisineAverageRating = restaurant?.CuisineAverageRating ?? -1,
                    EmployeesAverageRating = restaurant?.EmployeesAverageRating ?? -1,
                    IconData = restaurant.IconData,
                });
            }

            return restaurants;

        }

        [Authorize(Roles = "Employee")]
        public ICollection<RestaurantDto> GetAllRestaurantsWhereEmployeeWorks(string email)
        {
            ICollection<RestaurantDto> restaurants = new List<RestaurantDto>();

            foreach(var restaurant in _context.EmployeesRestaurants
                .Where(x => x.Employee.Profile.Email == email && !x.EndedOn.HasValue)
                .Select(x => x.Restaurant)
                .ToList())
            {
                restaurants.Add(new RestaurantDto
                {
                    Id = restaurant.Id.ToString(),
                    Name = restaurant.Name,
                    Address = restaurant.Address,
                    City = restaurant.City,
                    AtmosphereAverageRating = restaurant?.CuisineAverageRating ?? -1, // in front-end if -1 "no reviews yet"
                    CuisineAverageRating = restaurant?.CuisineAverageRating ?? -1,
                    EmployeesAverageRating = restaurant?.EmployeesAverageRating ?? -1,
                    IconData = restaurant.IconData,
                });
            }

            return restaurants;
        }

        [Authorize(Roles = "Manager")]
        public ICollection<EmployeeDto> GetAllEmployeesOfARestaurant(string restaurantId)
        {
            ICollection<EmployeeDto> employees = new List<EmployeeDto>();

            foreach (var emp in _context.EmployeesRestaurants
                .Where(er => er.Restaurant.Id.ToString() == restaurantId)
                .Select(e => e.Employee))
            {
                employees.Add(new EmployeeDto
                {
                    Email = emp.Profile.Email,
                    FirstName = emp.Profile.FirstName,
                    LastName = emp.Profile.LastName,
                    ProfilePictureData = emp.Profile.ProfilePictureData,
                    EmployeeAverageRating = emp?.EmployeeAverageRating ?? -1 // in front-end if -1 "no reviews yet"
                });
            }

            return employees;
        }
    }
}
