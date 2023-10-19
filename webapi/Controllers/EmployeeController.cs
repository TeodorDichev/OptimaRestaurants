using Microsoft.AspNetCore.Mvc;
using webapi.Data;

namespace webapi.Controllers
{
    /// <summary>
    /// This class manages all employee related functions
    /// Edit - city, birthdate
    /// Add/Delete a request to a manager's restaurant
    /// </summary>
    public class EmployeeController : Controller
    {
        private readonly OptimaRestaurantContext _context;
        public EmployeeController(OptimaRestaurantContext context)
        {
            _context = context;
        }

        public async Task<bool> SendRequestToManager(string managerId, string employeeId)
        {
            //employeesrestaurants through employee id -> add a record which is not confirmed
            //send an email to the manager
            //add a message to the manager inbox "do this gay work there"
            return true;
        }

    }
}
