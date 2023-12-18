using webapi.Data;
using webapi.DTOs.Review;
using webapi.Models;

namespace webapi.Services.ClassServices
{
    public class ReviewService
    {
        private readonly OptimaRestaurantContext _context;

        public ReviewService(OptimaRestaurantContext context)
        {
            _context = context;
        }

        public async Task<CustomerReview> AddCustomerReview(Restaurant restaurant, Employee employee, CustomerReviewDto model)
        {
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

            return review;
        }
        public async Task<ManagerReview> AddManagerReview(Restaurant restaurant, Employee employee, ManagerReviewDto model)
        {
            ManagerReview review = new ManagerReview()
            {
                Employee = employee,
                Restaurant = restaurant,
                DateTime = DateTime.UtcNow,
                Comment = model.Comment,
                CollegialityRating = model.CollegialityRating,
                PunctualityRating = model.PunctualityRating,
            };

            await _context.ManagerReviews.AddAsync(review);

            return review;
        }
        public bool UpdateStatistics(Employee employee, Restaurant restaurant)
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

                employee.PunctualityAverageRating = _context.ManagerReviews.Where(mr => mr.Employee == employee && mr.PunctualityRating != null)
                    .Select(mr => mr.PunctualityRating)
                    .Average();

                employee.CollegialityAverageRating = _context.ManagerReviews.Where(mr => mr.Employee == employee && mr.CollegialityRating != null)
                    .Select(mr => mr.CollegialityRating)
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

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
