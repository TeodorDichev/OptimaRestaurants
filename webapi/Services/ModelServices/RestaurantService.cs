using Microsoft.AspNetCore.Identity;
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
        public async Task<Restaurant> GetRestaurantById(string id)
        {
            return await _context.Restaurants.FirstOrDefaultAsync(r => r.Id.ToString() == id) ?? throw new ArgumentNullException("Ресторантът не съществува");
        }
        public async Task<bool> CheckRestaurantExistById(string id)
        {
            return await _context.Restaurants.AnyAsync(r => r.Id.ToString() == id);
        }
        public async Task<List<BrowseRestaurantDto>> GetAllRestaurants()
        {
            List<BrowseRestaurantDto> restaurantsDto = new List<BrowseRestaurantDto>();

            foreach (var restaurant in await _context.Restaurants.OrderByDescending(r => r.Name).ThenBy(r => r.IsWorking).ToListAsync())
            {
                var topEmployee = GetTopEmployeeOfRestaurant(restaurant);

                restaurantsDto.Add(new BrowseRestaurantDto
                {
                    Id = restaurant.Id.ToString(),
                    Name = restaurant.Name,
                    Address = restaurant.Address,
                    City = restaurant.City,
                    RestaurantAverageRating = restaurant?.RestaurantAverageRating ?? 0,
                    TotalReviewsCount = restaurant?.TotalReviewsCount ?? 0,
                    IsWorking = restaurant?.IsWorking ?? false,
                    IconPath = restaurant?.IconPath,
                    TopEmployeeFullName = topEmployee?.Profile.FirstName + " " + topEmployee?.Profile.LastName,
                    TopEmployeeRating = topEmployee?.EmployeeAverageRating ?? 0,
                    TopEmployeeEmail = topEmployee?.Profile.Email ?? string.Empty,
                    TopEmployeePicturePath = topEmployee?.Profile.ProfilePicturePath
                });
            }

            return restaurantsDto;
        }
        public async Task<List<BrowseRestaurantDto>> GetCityRestaurants(string cityName)
        {
            List<BrowseRestaurantDto> restaurantsDto = new List<BrowseRestaurantDto>();

            foreach (var restaurant in await _context.Restaurants.Where(r => r.City.ToLower() == cityName).OrderByDescending(r => r.Name).ThenBy(r => r.IsWorking).ToListAsync())
            {
                var topEmployee = GetTopEmployeeOfRestaurant(restaurant);

                restaurantsDto.Add(new BrowseRestaurantDto
                {
                    Id = restaurant.Id.ToString(),
                    Name = restaurant.Name,
                    Address = restaurant.Address,
                    City = restaurant.City,
                    RestaurantAverageRating = restaurant?.RestaurantAverageRating ?? 0,
                    TotalReviewsCount = restaurant?.TotalReviewsCount ?? 0,
                    IsWorking = restaurant?.IsWorking ?? false,
                    IconPath = restaurant?.IconPath,
                    TopEmployeeFullName = topEmployee?.Profile.FirstName + " " + topEmployee?.Profile.LastName,
                    TopEmployeeRating = topEmployee?.EmployeeAverageRating ?? 0,
                    TopEmployeeEmail = topEmployee?.Profile.Email ?? string.Empty,
                    TopEmployeePicturePath = topEmployee?.Profile.ProfilePicturePath
                });
            }

            return restaurantsDto;
        }
        public async Task<List<BrowseRestaurantDto>> GetRatingRestaurants(decimal rating)
        {
            List<BrowseRestaurantDto> restaurantsDto = new List<BrowseRestaurantDto>();

            foreach (var restaurant in await _context.Restaurants.Where(r => r.RestaurantAverageRating >= rating).OrderByDescending(r => r.Name).ThenBy(r => r.IsWorking).ToListAsync())
            {
                var topEmployee = GetTopEmployeeOfRestaurant(restaurant);

                restaurantsDto.Add(new BrowseRestaurantDto
                {
                    Id = restaurant.Id.ToString(),
                    Name = restaurant.Name,
                    Address = restaurant.Address,
                    City = restaurant.City,
                    RestaurantAverageRating = restaurant?.RestaurantAverageRating ?? 0,
                    TotalReviewsCount = restaurant?.TotalReviewsCount ?? 0,
                    IsWorking = restaurant?.IsWorking ?? false,
                    IconPath = restaurant?.IconPath,
                    TopEmployeeFullName = topEmployee?.Profile.FirstName + " " + topEmployee?.Profile.LastName,
                    TopEmployeeRating = topEmployee?.EmployeeAverageRating ?? 0,
                    TopEmployeeEmail = topEmployee?.Profile.Email ?? string.Empty,
                    TopEmployeePicturePath = topEmployee?.Profile.ProfilePicturePath
                });
            }

            return restaurantsDto;
        }
        public async Task<List<BrowseRestaurantDto>> GetRestaurantsByCertainRating(string ratingType)
        {
            List<BrowseRestaurantDto> restaurantsDto = new List<BrowseRestaurantDto>();

            switch (ratingType)
            {
                case "CuisineAverageRating":
                    foreach (var restaurant in await _context.Restaurants.OrderByDescending(r => r.CuisineAverageRating).ThenByDescending(r => r.Name).ThenBy(r => r.IsWorking).ToListAsync())
                    {
                        var topEmployee = GetTopEmployeeOfRestaurant(restaurant);

                        restaurantsDto.Add(new BrowseRestaurantDto
                        {
                            Id = restaurant.Id.ToString(),
                            Name = restaurant.Name,
                            Address = restaurant.Address,
                            City = restaurant.City,
                            RestaurantAverageRating = restaurant?.RestaurantAverageRating ?? 0,
                            TotalReviewsCount = restaurant?.TotalReviewsCount ?? 0,
                            IsWorking = restaurant?.IsWorking ?? false,
                            IconPath = restaurant?.IconPath,
                            TopEmployeeFullName = topEmployee?.Profile.FirstName + " " + topEmployee?.Profile.LastName,
                            TopEmployeeRating = topEmployee?.EmployeeAverageRating ?? 0,
                            TopEmployeeEmail = topEmployee?.Profile.Email ?? string.Empty,
                            TopEmployeePicturePath = topEmployee?.Profile.ProfilePicturePath
                        });
                    }
                    break;

                case "AtmosphereAverageRating":
                    foreach (var restaurant in await _context.Restaurants.OrderByDescending(r => r.AtmosphereAverageRating).ThenByDescending(r => r.Name).ThenBy(r => r.IsWorking).ToListAsync())
                    {
                        var topEmployee = GetTopEmployeeOfRestaurant(restaurant);

                        restaurantsDto.Add(new BrowseRestaurantDto
                        {
                            Id = restaurant.Id.ToString(),
                            Name = restaurant.Name,
                            Address = restaurant.Address,
                            City = restaurant.City,
                            RestaurantAverageRating = restaurant?.RestaurantAverageRating ?? 0,
                            TotalReviewsCount = restaurant?.TotalReviewsCount ?? 0,
                            IsWorking = restaurant?.IsWorking ?? false,
                            IconPath = restaurant?.IconPath,
                            TopEmployeeFullName = topEmployee?.Profile.FirstName + " " + topEmployee?.Profile.LastName,
                            TopEmployeeRating = topEmployee?.EmployeeAverageRating ?? 0,
                            TopEmployeeEmail = topEmployee?.Profile.Email ?? string.Empty,
                            TopEmployeePicturePath = topEmployee?.Profile.ProfilePicturePath
                        });
                    }
                    break;

                case "EmployeesAverageRating":
                    foreach (var restaurant in await _context.Restaurants.OrderByDescending(r => r.EmployeesAverageRating).ThenByDescending(r => r.Name).ThenBy(r => r.IsWorking).ToListAsync())
                    {
                        var topEmployee = GetTopEmployeeOfRestaurant(restaurant);

                        restaurantsDto.Add(new BrowseRestaurantDto
                        {
                            Id = restaurant.Id.ToString(),
                            Name = restaurant.Name,
                            Address = restaurant.Address,
                            City = restaurant.City,
                            RestaurantAverageRating = restaurant?.RestaurantAverageRating ?? 0,
                            TotalReviewsCount = restaurant?.TotalReviewsCount ?? 0,
                            IsWorking = restaurant?.IsWorking ?? false,
                            IconPath = restaurant?.IconPath,
                            TopEmployeeFullName = topEmployee?.Profile.FirstName + " " + topEmployee?.Profile.LastName,
                            TopEmployeeRating = topEmployee?.EmployeeAverageRating ?? 0,
                            TopEmployeeEmail = topEmployee?.Profile.Email ?? string.Empty,
                            TopEmployeePicturePath = topEmployee?.Profile.ProfilePicturePath
                        });
                    }
                    break;

                default:
                    break;
            }

            return restaurantsDto;
        }
        public RestaurantDetailsDto GetRestaurantDetails(Restaurant restaurant)
        {
            var manager = restaurant.Manager;

            var topEmployee = GetTopEmployeeOfRestaurant(restaurant);

            var restaurantDto = new RestaurantDetailsDto
            {
                Address = restaurant.Address,
                AtmosphereAverageRating = restaurant.AtmosphereAverageRating ?? 0,
                RestaurantAverageRating = restaurant.RestaurantAverageRating ?? 0,
                CuisineAverageRating = restaurant.CuisineAverageRating ?? 0,
                EmployeesAverageRating = restaurant.EmployeesAverageRating ?? 0,
                TotalReviewsCount = restaurant.TotalReviewsCount,
                City = restaurant.City,
                EmployeeCapacity = restaurant.EmployeeCapacity ?? 0,
                IconPath = restaurant.IconPath,
                IsWorking = restaurant.IsWorking,
                Name = restaurant.Name,
                Id = restaurant.Id.ToString(),
                ManagerFullName = manager?.Profile.FirstName + " " + manager?.Profile.LastName,
                ManagerPhoneNumber = manager?.Profile.Email ?? string.Empty,
                ManagerEmail = manager?.Profile.Email ?? string.Empty,
                TopEmployeeFullName = topEmployee?.Profile.FirstName + " " + topEmployee?.Profile.LastName,
                TopEmployeeRating = topEmployee?.EmployeeAverageRating ?? 0,
                TopEmployeeEmail = topEmployee?.Profile.Email ?? string.Empty,
                TopEmployeePicturePath = topEmployee?.Profile.ProfilePicturePath
            };

            return restaurantDto;

        }
        public async Task<List<Restaurant>> GetRestaurantsWithMatchingNames(string input)
        {
            return await _context.Restaurants.Where(r => EF.Functions.Like(r.Name, $"{input}%")).ToListAsync();
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
        public async Task<Restaurant> AddRestaurant(NewRestaurantDto model, Manager manager)
        {
            Restaurant restaurant = new Restaurant
            {
                Name = model.Name,
                Address = model.Address,
                City = model.City,
                IsWorking = true,
                EmployeeCapacity = model.EmployeeCapacity,
                TotalReviewsCount = 0,
                Manager = manager
            };

            if (model.IconFile != null) restaurant.IconPath = _pictureService.SaveImage(model.IconFile);
            await _context.Restaurants.AddAsync(restaurant);

            return restaurant;
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
        public List<AccountRestaurantDto> GetRestaurantsOfEmployee(Employee employee)
        {
            List<AccountRestaurantDto> restaurants = new List<AccountRestaurantDto>();

            foreach (var restaurant in employee.EmployeesRestaurants
                .Where(er => !er.EndedOn.HasValue)
                .Select(er => er.Restaurant))
            {
                restaurants.Add(new AccountRestaurantDto
                {
                    Id = restaurant.Id.ToString(),
                    Name = restaurant.Name,
                    Address = restaurant.Address,
                    City = restaurant.City,
                    TotalReviewsCount = restaurant.TotalReviewsCount,
                    IsWorking = restaurant.IsWorking,
                    EmployeeCapacity = restaurant.EmployeeCapacity ?? 0,
                    AtmosphereAverageRating = restaurant?.CuisineAverageRating ?? 0,
                    CuisineAverageRating = restaurant?.CuisineAverageRating ?? 0,
                    EmployeesAverageRating = restaurant?.EmployeesAverageRating ?? 0,
                    RestaurantAverageRating = restaurant?.RestaurantAverageRating ?? 0,
                    IconPath = restaurant?.IconPath,
                    ManagerEmail = restaurant?.Manager?.Profile?.Email ?? "Ресторантът няма мениджър",
                    ManagerPhone = restaurant?.Manager?.Profile?.PhoneNumber ?? "Ресторантът няма мениджър",
                    ManagerName = restaurant?.Manager?.Profile?.FirstName + " " + restaurant?.Manager?.Profile.LastName ?? "Ресторантът няма мениджър",
                });
            }

            return restaurants;
        }
        public List<AccountRestaurantDto> GetRestaurantsOfManager(Manager manager)
        {
            List<AccountRestaurantDto> restaurantsDto = new List<AccountRestaurantDto>();

            foreach (var restaurant in manager.Restaurants)
            {
                restaurantsDto.Add(new AccountRestaurantDto
                {
                    Id = restaurant.Id.ToString(),
                    Name = restaurant.Name,
                    Address = restaurant.Address,
                    City = restaurant.City,
                    TotalReviewsCount = restaurant.TotalReviewsCount,
                    IsWorking = restaurant.IsWorking,
                    EmployeeCapacity = restaurant.EmployeeCapacity ?? 0,
                    AtmosphereAverageRating = restaurant?.CuisineAverageRating ?? 0,
                    CuisineAverageRating = restaurant?.CuisineAverageRating ?? 0,
                    EmployeesAverageRating = restaurant?.EmployeesAverageRating ?? 0,
                    RestaurantAverageRating = restaurant?.RestaurantAverageRating ?? 0,
                    IconPath = restaurant?.IconPath,
                    ManagerEmail = restaurant?.Manager?.Profile?.Email ?? "Ресторантът няма мениджър",
                    ManagerPhone = restaurant?.Manager?.Profile?.PhoneNumber ?? "Ресторантът няма мениджър",
                    ManagerName = restaurant?.Manager?.Profile?.FirstName + " " + restaurant?.Manager?.Profile.LastName ?? "Ресторантът няма мениджър",
                });
            }

            return restaurantsDto;
        }
        public Employee? GetTopEmployeeOfRestaurant(Restaurant restaurant)
        {
            return restaurant.EmployeesRestaurants.Where(er => er.EndedOn == null).Select(e => e.Employee).OrderBy(e => e.EmployeeAverageRating).FirstOrDefault();
        }
        public List<Employee> GetEmployeesOfRestaurant(Restaurant restaurant)
        {
            return restaurant.EmployeesRestaurants.Where(er => er.EndedOn == null).Select(e => e.Employee).ToList();
        }
        public bool IsRestaurantAtMaxCapacity(Restaurant restaurant)
        {
            return restaurant.EmployeeCapacity <= _context.EmployeesRestaurants
                .Where(er => er.Restaurant == restaurant).Count();
        }
        public bool HasRestaurantAManager(Restaurant restaurant)
        {
            return restaurant.Manager == null;
        }
        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
