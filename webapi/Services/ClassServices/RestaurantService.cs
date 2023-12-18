using Microsoft.EntityFrameworkCore;
using webapi.Data;
using webapi.DTOs.Restaurant;
using webapi.Models;
using webapi.Services.FileServices;

namespace webapi.Services.ClassServices
{
    public class RestaurantService
    {
        private readonly OptimaRestaurantContext _context;
        private readonly PicturesAndIconsService _pictureService;

        public RestaurantService(OptimaRestaurantContext context,
        PicturesAndIconsService pictureService)
        {
            _context = context;
            _pictureService = pictureService;
        }
        public bool FireAnEmployee(Restaurant restaurant, Employee employee)
        {
            try
            {
                foreach (var er in restaurant.EmployeesRestaurants.Where(er => er.Employee.Profile.Email == employee.Profile.Email))
                {
                    er.EndedOn = DateTime.UtcNow;
                    _context.EmployeesRestaurants.Update(er);
                }

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public async Task<Restaurant> GetRestaurantById(string id)
        {
            return await _context.Restaurants.FirstOrDefaultAsync(r => r.Id.ToString() == id) ?? throw new ArgumentNullException("Ресторантът не съществува");
        }
        public async Task<bool> CheckRestaurantExistById(string id)
        {
            return await _context.Restaurants.AnyAsync(r => r.Id.ToString() == id);
        }
        public List<Employee> GetEmployeesOfRestaurant(Restaurant restaurant)
        {
            return restaurant.EmployeesRestaurants.Select(e => e.Employee).ToList();
        }
        public List<Restaurant> GetRestaurantsOfManager(Manager manager)
        {
            return _context.Restaurants.Where(r => r.Manager == manager).ToList();
        }
        public async Task<Restaurant> AddRestaurant(NewRestaurantDto model, Manager manager)
        {
            Restaurant restaurant = new Restaurant
            {
                Name = model.Name,
                Address = model.Address,
                City = model.City,
                IsWorking = true,
                EmployeeCapacity = model.EmployeeCapacity,
                Manager = manager
            };

            if (model.IconFile != null) restaurant.IconPath = _pictureService.SaveImage(model.IconFile);
            await _context.Restaurants.AddAsync(restaurant);

            return restaurant;
        }
        public bool DeleteRestaurant(Restaurant restaurant)
        {
            try
            {
                foreach (var er in restaurant.EmployeesRestaurants) er.EndedOn = DateTime.Now;
                restaurant.Manager = null;
                _context.Restaurants.Remove(restaurant);

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public Restaurant UpdateRestaurant(Restaurant restaurant, UpdateRestaurantDto updateDto)
        {
            if (updateDto.IsWorking.HasValue) restaurant.IsWorking = updateDto.IsWorking.Value;
            if (updateDto.Address != null) restaurant.Address = updateDto.Address;
            if (updateDto.City != null) restaurant.City = updateDto.City;
            if (updateDto.Name != null) restaurant.Name = updateDto.Name;
            if (updateDto.EmployeeCapacity.HasValue) restaurant.EmployeeCapacity = (int)updateDto.EmployeeCapacity;
            if (updateDto.IconFile != null)
            {
                if (restaurant.IconPath == null) restaurant.IconPath = _pictureService.SaveImage(updateDto.IconFile);
                else
                {
                    _pictureService.DeleteImage(restaurant.IconPath);
                    restaurant.IconPath = _pictureService.SaveImage(updateDto.IconFile);
                }
            }

            return restaurant;
        }
        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

    }
}
