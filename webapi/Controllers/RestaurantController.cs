using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using webapi.Data;
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
        [Authorize(Roles = "Admin")]
        [Authorize(Roles = "Employee")]
        public async Task<IEnumerable<Restaurant>> GetAllRestaurantsByStandardMonthlyPayment(decimal minimum = 0)
        {
            return await _context.Restaurants.OrderBy(r => r.StandardMonthlyPayment > minimum).ThenBy(r => r.Name).ToListAsync();
        }
        [Authorize(Roles = "Admin")]
        [Authorize(Roles = "Employee")]
        public async Task<IEnumerable<Restaurant>> GetAllRestaurantsWithOpenJobs()
        {
            return await _context.Restaurants
                .Where(r => r.IsWorking && (r.EmployeeCapacity - r.EmployeesRestaurants.Where(x => x.Restaurant.Id == r.Id).Count() > 0))
                .OrderBy(r => r.Name)
                .ToListAsync();
        }
        [Authorize(Roles = "Admin")]
        [Authorize(Roles = "Employee")]
        public async Task<IEnumerable<Restaurant>> GetAllRestaurantsOfAManager(string managerEmail)
        {
            return await _context.Restaurants
                .Where(r => r.Manager.Profile.Email.ToString() == managerEmail)
                .OrderBy(r => r.Name)
                .ToListAsync();
        }
        [Authorize(Roles = "Admin")]
        [Authorize(Roles = "Employee")]
        public async Task<IEnumerable<Restaurant>> GeAllRestaurantsWhereEmployeeWorks(string employeeEmail)
        {
            return await _context.EmployeesRestaurants
                .Where(x => x.Employee.Profile.Email == employeeEmail && !x.EndedOn.HasValue && x.ConfirmedOn.HasValue)
                .Select(x => x.Restaurant)
                .ToListAsync();
        }
        public async Task<IEnumerable<Employee>> GeAllEmployeesOfARestaurant(string restaurantId)
        {
            return (IEnumerable<Employee>) await _context.Restaurants
                .Where(r => r.Id.ToString() == restaurantId)
                .Select(x => x.EmployeesRestaurants.Select(y => y.Employee))
                .ToListAsync();
        }

    }
}
