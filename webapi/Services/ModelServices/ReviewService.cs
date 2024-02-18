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
            employee.TotalReviewsCount++;
            restaurant.TotalReviewsCount++;

            CustomerReview review = new CustomerReview()
            {
                Employee = employee,
                Restaurant = restaurant,
                DateTime = DateTime.Now,
                Comment = model.Comment,
                SpeedRating = model.SpeedRating == 0 ? null : model.SpeedRating,
                AttitudeRating = model.AttitudeRating == 0 ? null : model.AttitudeRating,
                AtmosphereRating = model.AtmosphereRating == 0 ? null : model.AtmosphereRating,
                CuisineRating = model.CuisineRating == 0 ? null : model.CuisineRating,
            };

            await _context.CustomerReviews.AddAsync(review);

            return review;
        }
        public async Task<ManagerReview> AddManagerReview(Restaurant restaurant, Employee employee, ManagerReviewDto model)
        {
            employee.TotalReviewsCount++;

            ManagerReview review = new ManagerReview()
            {
                Employee = employee,
                Restaurant = restaurant,
                DateTime = DateTime.Now,
                Comment = model.Comment,
                CollegialityRating = model.CollegialityRating == 0 ? null : model.CollegialityRating,
                PunctualityRating = model.PunctualityRating == 0 ? null : model.PunctualityRating,
            };

            await _context.ManagerReviews.AddAsync(review);

            return review;
        }
        public List<OldReviewDto> GetEmployeeReviewsHistory(Employee employee)
        {
            List<OldReviewDto> reviews = new List<OldReviewDto>();

            foreach (var cr in _context.CustomerReviews.Take(10).Where(cr => cr.Employee == employee).ToList())
            {
                reviews.Add(new OldReviewDto
                {
                    RestaurantName = cr.Restaurant.Name,
                    RestaurantCity = cr.Restaurant.City,
                    ReviewType = "CustomerReview",
                    ReviewDate = cr.DateTime.ToShortDateString(),
                    Comment = cr.Comment,
                    AtmosphereRating = cr.AtmosphereRating,
                    AttitudeRating = cr.AttitudeRating,
                    SpeedRating = cr.SpeedRating,
                    CuisineRating = cr.CuisineRating,
                });
            }

            foreach (var mr in _context.ManagerReviews.Take(10).Where(mr => mr.Employee == employee).ToList())
            {
                reviews.Add(new OldReviewDto
                {
                    RestaurantName = mr.Restaurant.Name,
                    RestaurantCity = mr.Restaurant.City,
                    ReviewType = "ManagerReview",
                    ReviewDate = mr.DateTime.ToShortDateString(),
                    Comment = mr.Comment,
                    CollegialityRating = mr.CollegialityRating,
                    PunctualityRating = mr.PunctualityRating,
                });
            }

            return reviews.OrderBy(r => Convert.ToDateTime(r.ReviewDate)).ToList();
        }
        public bool UpdateAttitude(Employee employee, decimal AttitudeRating)
        {
            try
            {
                if (employee.AttitudeAverageRating != null)
                {
                    employee.AttitudeAverageRating = (employee.AttitudeAverageRating + AttitudeRating) / 2;
                    return true;
                }
                employee.AttitudeAverageRating = AttitudeRating;
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public bool UpdateSpeed(Employee employee, decimal speedRating)
        {
            try
            {
                if (employee.SpeedAverageRating != null) employee.SpeedAverageRating = (employee.SpeedAverageRating + speedRating) / 2;
                else employee.SpeedAverageRating = speedRating;
                
                return UpdateEmployeeAverage(employee, speedRating);
            }
            catch (Exception)
            {
                return false;
            }
        }
        public bool UpdatePunctuality(Employee employee, decimal punctualityRating)
        {
            try
            {
                if (employee.PunctualityAverageRating != null) employee.PunctualityAverageRating = (employee.PunctualityAverageRating + punctualityRating) / 2;
                else employee.PunctualityAverageRating = punctualityRating;

                return UpdateEmployeeAverage(employee, punctualityRating);
            }
            catch (Exception)
            {
                return false;
            }
        }
        public bool UpdateCollegiality(Employee employee, decimal collegialityRating)
        {
            try
            {
                if (employee.CollegialityAverageRating != null) employee.CollegialityAverageRating = (employee.CollegialityAverageRating + collegialityRating) / 2;
                else employee.CollegialityAverageRating = collegialityRating;

                return UpdateEmployeeAverage(employee, collegialityRating);
            }
            catch (Exception)
            {
                return false;
            }
        }
        public bool UpdateAtmosphere(Restaurant restaurant, decimal atmosphereRating)
        {
            try
            {
                if (restaurant.AtmosphereAverageRating != null) restaurant.AtmosphereAverageRating = (restaurant.AtmosphereAverageRating + atmosphereRating) / 2;
                else restaurant.AtmosphereAverageRating = atmosphereRating;

                return UpdateRestaurantAverage(restaurant, atmosphereRating);
            }
            catch (Exception)
            {
                return false;
            }
        }
        public bool UpdateCuisine(Restaurant restaurant, decimal cuisineRating)
        {
            try
            {
                if (restaurant.CuisineAverageRating != null) restaurant.CuisineAverageRating = (restaurant.CuisineAverageRating + cuisineRating) / 2;
                else restaurant.CuisineAverageRating = cuisineRating;

                return UpdateRestaurantAverage(restaurant, cuisineRating);
            }
            catch (Exception)
            {
                return false;
            }
        }
        public bool UpdateRestaurantEmployeesAverage(Restaurant restaurant, decimal rating)
        {
            try
            {
                if (restaurant.EmployeesAverageRating != null) restaurant.EmployeesAverageRating = (restaurant.RestaurantAverageRating + rating) / 2;
                else restaurant.EmployeesAverageRating = rating;

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
        private bool UpdateEmployeeAverage (Employee employee, decimal rating)
        {
            try
            {
                if (employee.EmployeeAverageRating != null) employee.EmployeeAverageRating = (employee.EmployeeAverageRating + rating) / 2;
                else employee.EmployeeAverageRating = rating;

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        private bool UpdateRestaurantAverage (Restaurant restaurant, decimal rating)
        {
            try
            {
                if (restaurant.RestaurantAverageRating != null) restaurant.RestaurantAverageRating = (restaurant.RestaurantAverageRating + rating) / 2;
                else restaurant.RestaurantAverageRating = rating;

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
