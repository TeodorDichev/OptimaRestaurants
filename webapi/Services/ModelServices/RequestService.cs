using Microsoft.EntityFrameworkCore;
using webapi.Data;
using webapi.DTOs.Request;
using webapi.Models;

namespace webapi.Services.ModelServices
{
    public class RequestService
    {
        private readonly OptimaRestaurantContext _context;

        public RequestService(OptimaRestaurantContext context)
        {
            _context = context;
        }
        public async Task<Request> AddRequest(Employee employee, Restaurant restaurant, bool isFromEmployee)
        {
            Request request;

            if (isFromEmployee)
            {
                request = new Request
                {
                    Sender = employee.Profile,
                    Receiver = restaurant.Manager.Profile,
                    Restaurant = restaurant,
                    SentOn = DateTime.Now,
                };
            }
            else
            {
                request = new Request
                {
                    Sender = restaurant.Manager.Profile,
                    Receiver = employee.Profile,
                    Restaurant = restaurant,
                    SentOn = DateTime.Now,
                };

            }
            await _context.Requests.AddAsync(request);
            return request;
        }
        public async Task<Request> GetRequestById(string id)
        {
            return await _context.Requests.FirstOrDefaultAsync(e => e.Id.ToString() == id) ?? throw new ArgumentNullException("Заявката не съществува");
        }
        public async Task<bool> CheckRequestExistById(string id)
        {
            return await _context.Requests.AnyAsync(e => e.Id.ToString() == id);
        }
        public List<RequestDto> GetManagerRequests(string email)
        {
            List<RequestDto> requests = new List<RequestDto>();

            /* adding all unanswered requests */
            foreach (var r in _context.Requests.Where(r => r.Receiver.Email == email && (r.ConfirmedOn == null && r.RejectedOn == null)).OrderByDescending(r => r.SentOn))
            {
                var request = new RequestDto
                {
                    Id = r.Id.ToString(),
                    RestaurantId = r.Restaurant.Id.ToString(),
                    SenderEmail = r.Sender.Email ?? string.Empty,
                    SentOn = r.SentOn.ToString(),
                    Confirmed = null,
                    Text = $"Работи ли {r.Sender.FirstName + " " + r.Sender.LastName} в {r.Restaurant.Name}?"
                };

                requests.Add(request);
            }

            /* adding the most recent 10 answered requests (limited due to scaling issues) */
            foreach (var r in _context.Requests.Where(r => r.Receiver.Email == email && (r.ConfirmedOn != null || r.RejectedOn != null)).OrderByDescending(r => r.SentOn))
            {
                bool confirmed = false;
                if (r.ConfirmedOn != null) confirmed = true;

                var request = new RequestDto
                {
                    Id = r.Id.ToString(),
                    RestaurantId = r.Restaurant.Id.ToString(),
                    SenderEmail = r.Sender.Email ?? string.Empty,
                    SentOn = r.SentOn.ToString(),
                    Confirmed = confirmed,
                    Text = $"Работи ли {r.Sender.FirstName + " " + r.Sender.LastName} в {r.Restaurant.Name}?"
                };

                requests.Add(request);
            }

            return requests;
        }
        public List<RequestDto> GetEmployeeRequests(string email)
        {
            List<RequestDto> requests = new List<RequestDto>();

            /* adding all unanswered requests */
            foreach (var r in _context.Requests.Where(r => r.Receiver.Email == email && (r.ConfirmedOn == null && r.RejectedOn == null)).OrderByDescending(r => r.SentOn))
            {
                var request = new RequestDto
                {
                    Id = r.Id.ToString(),
                    RestaurantId = r.Restaurant.Id.ToString(),
                    SenderEmail = r.Sender.Email ?? string.Empty,
                    SentOn = r.SentOn.ToString(),
                    Confirmed = null,
                    Text = $"Работите ли в ресторантът {r.Restaurant.Name}, собственост на {r.Sender.FirstName + " " + r.Sender.LastName}?"
                };

                requests.Add(request);
            }

            /* adding the most recent 10 answered requests (limited due to scaling issues) */
            foreach (var r in _context.Requests.Where(r => r.Receiver.Email == email && (r.ConfirmedOn != null || r.RejectedOn != null)).OrderByDescending(r => r.SentOn))
            {
                bool confirmed = false;
                if (r.ConfirmedOn != null) confirmed = true;

                var request = new RequestDto
                {
                    Id = r.Id.ToString(),
                    RestaurantId = r.Restaurant.Id.ToString(),
                    SenderEmail = r.Sender.Email ?? string.Empty,
                    SentOn = r.SentOn.ToString(),
                    Confirmed = confirmed,
                    Text = $"Работите ли в ресторантът {r.Restaurant.Name}, собственост на {r.Sender.FirstName + " " + r.Sender.LastName}?"
                };

                requests.Add(request);
            }


            return requests;
        }
        public async Task<bool> RespondToRequest(Employee employee, Restaurant restaurant, Request request, bool confirmed)
        {
            if (confirmed)
            {
                request.ConfirmedOn = DateTime.Now;
                EmployeeRestaurant er = new EmployeeRestaurant
                {
                    Employee = employee,
                    Restaurant = restaurant,
                    StartedOn = DateTime.Now,
                };
                employee.EmployeesRestaurants.Add(er);
                restaurant.EmployeesRestaurants.Add(er);
                await _context.EmployeesRestaurants.AddAsync(er);
                return true;
            }
            else
            {
                request.RejectedOn = DateTime.Now;
                return false;
            }

        }
        public async Task<bool> IsRequestAlreadySent(ApplicationUser profile, Restaurant restaurant)
        {
            return await _context.Requests.FirstOrDefaultAsync(r => r.Sender == profile && r.Restaurant == restaurant && r.SentOn.AddDays(7) > DateTime.Now) != null;
        }
        public async Task<bool> IsRequestReceived(ApplicationUser profile, Restaurant restaurant)
        {
            return await _context.Requests.FirstOrDefaultAsync(r => r.Receiver == profile && r.Restaurant == restaurant && r.SentOn.AddDays(7) > DateTime.Now) != null;
        }
        public bool IsEmployeeAlreadyWorkingInRestaurant(Employee employee, Restaurant restaurant)
        {
            return restaurant.EmployeesRestaurants.FirstOrDefault(er => er.Employee == employee && er.EndedOn == null) != null;
        }
        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

    }
}
