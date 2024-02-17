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
                    AtmosphereRating = mr.CollegialityRating,
                    AttitudeRating = mr.PunctualityRating,
                });
            }

            return reviews.OrderBy(r => Convert.ToDateTime(r.ReviewDate)).ToList();
        }
        public bool UpdateStatistics(Employee employee, Restaurant restaurant)
        {
            try
            {
                /* updating employee statistics */
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

                _context.SaveChanges();

                int dellEmp = 0;
                if (employee.PunctualityAverageRating == null) ++dellEmp;
                if (employee.AttitudeAverageRating == null) ++dellEmp;
                if (employee.CollegialityAverageRating == null) ++dellEmp;
                if (employee.SpeedAverageRating == null) ++dellEmp;
                if (dellEmp != 0)
                {
                    employee.EmployeeAverageRating = (employee.PunctualityAverageRating ?? 0
                        + employee.AttitudeAverageRating ?? 0
                        + employee.CollegialityAverageRating ?? 0
                        + employee.SpeedAverageRating ?? 0) / dellEmp;
                }

                _context.SaveChanges();

                /* updating restaurants statistics */
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

                _context.SaveChanges();

                int dellRes = 0;
                if (restaurant.RestaurantAverageRating == null) ++dellRes;
                if (restaurant.AtmosphereAverageRating == null) ++dellRes;
                if (restaurant.EmployeesAverageRating == null) ++dellRes;

                restaurant.RestaurantAverageRating = (restaurant.CuisineAverageRating ?? 0
                    + restaurant.AtmosphereAverageRating ?? 0
                    + restaurant.EmployeesAverageRating ?? 0) / dellRes;

                _context.SaveChanges();

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
