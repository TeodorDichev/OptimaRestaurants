using Microsoft.AspNetCore.Mvc;
using webapi.Data;

namespace webapi.Controllers
{
    public class ManagerController : Controller
    {
        private readonly OptimaRestaurantContext _context;
        public ManagerController(OptimaRestaurantContext context)
        {
            _context = context;
        }

        public async Task<bool> SendRequestToEmployee(string managerId, string employeeId)
        {
            //employeesrestaurants through employee id -> add a record which is not confirmed
            //send an email to the employee
            //add a message to the employee inbox "do you work there"
            return true;
        }
    }
}
