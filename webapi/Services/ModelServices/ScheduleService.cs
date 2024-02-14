using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using webapi.Data;
using webapi.DTOs.Employee;
using webapi.DTOs.Schedule;
using webapi.Models;
using webapi.Services.ClassServices;

namespace webapi.Services.ModelServices
{
    public class ScheduleService
    {
        private readonly OptimaRestaurantContext _context;
        private readonly EmployeeService _employeeService;
        private readonly RestaurantService _restaurantService;

        public ScheduleService(OptimaRestaurantContext context,
            EmployeeService employeeService,
            RestaurantService restaurantService)
        {
            _context = context;
            _employeeService = employeeService;
            _restaurantService = restaurantService;
        }

        /* Possible issue: not adding to the specific employee */

        public async Task<Schedule> AddAssignmentToSchedule(ScheduleDto model)
        {
            return await AddAssignmentToScheduleInternal(model.Day, model.EmployeeEmail, model.RestaurantId, model.From, model.To, model.IsWorkDay);
        }

        public async Task<Schedule> AddAssignmentToSchedule(CreateScheduleDto model)
        {
            return await AddAssignmentToScheduleInternal(model.Day, model.EmployeeEmail, model.RestaurantId, model.From, model.To, model.IsWorkDay);
        }

        public async Task<Schedule> EditScheduleAssignment(ScheduleDto model)
        {
            Schedule schedule = await GetEmployeeAssignment(model.ScheduleId);
            
            schedule.Day = model.Day;
            schedule.Employee = await _employeeService.GetEmployeeByEmail(model.EmployeeEmail);
            schedule.Restaurant = await _restaurantService.GetRestaurantById(model.RestaurantId);
            schedule.From = model.From;
            schedule.To = model.To;
            schedule.FullDay = (model.To == null || model.From == null);
            schedule.IsWorkDay = model.IsWorkDay;

            _context.Schedules.Update(schedule);

            return schedule;
        }
        public async Task<bool> DoesScheduleExistsById(string scheduleId)
        {
            return await _context.Schedules.AnyAsync(a => a.Id.ToString() == scheduleId);
        }
        public async Task<List<EmployeeFullScheduleDto>> GetAssignedDaysOfEmployee(Employee employee, int month)
        {
            List<EmployeeFullScheduleDto> schedule = new List<EmployeeFullScheduleDto>();

            foreach (var assignment in await _context.Schedules.Where(s => s.Employee == employee && s.Day.Month == month).ToListAsync())
            {
                schedule.Add(new EmployeeFullScheduleDto
                {
                    ScheduleId = assignment.Id.ToString(),
                    Day = assignment.Day,
                    IsWorkDay = assignment.IsWorkDay
                });
            }

            return schedule;
        }
        public async Task<List<EmployeeFullScheduleDto>> GetAssignedDaysOfEmployeeInRestaurant(Employee employee, Restaurant restaurant, int month)
        {
            List<EmployeeFullScheduleDto> schedule = new List<EmployeeFullScheduleDto>();

            foreach (var assignment in await _context.Schedules.Where(s => s.Employee == employee && s.Restaurant == restaurant && s.Day.Month == month).ToListAsync())
            {
                schedule.Add(new EmployeeFullScheduleDto
                {
                    ScheduleId = assignment.Id.ToString(),
                    Day = assignment.Day,
                    IsWorkDay = assignment.IsWorkDay
                });
            }

            return schedule;
        }
        public List<EmployeeDailyScheduleDto> GetEmployeeDailySchedule(Employee employee, DateOnly day)
        {
            List<EmployeeDailyScheduleDto> schedule = new List<EmployeeDailyScheduleDto>();

            foreach (var assignment in _context.Schedules.Where(s => s.Employee == employee && s.Day == day))
            {
                schedule.Add(new EmployeeDailyScheduleDto
                {
                    ScheduleId = assignment.Id.ToString(),
                    IsWorkDay = assignment.IsWorkDay,
                    From = assignment.From,
                    To = assignment.To,
                    RestaurantName = assignment.Restaurant.Name,
                    IsFullDay = assignment.FullDay
                });
            }

            return schedule;
        }
        public async Task<List<FreeEmployeeDto>> GetFreeEmployees(Restaurant restaurant, DateOnly day)
        {
            List<Employee> employees = restaurant.EmployeesRestaurants.Where(er => er.EndedOn == null).Select(er => er.Employee).ToList();
            List<FreeEmployeeDto> freeEmployees = new List<FreeEmployeeDto>();

            foreach (var employee in employees)
            {
                /* If this employee has a full day work/leisure assignment for this day in any restaurant then it cannot work on this day */
                if (await _context.Schedules.AnyAsync(a => a.Employee == employee && a.FullDay && a.Day == day)) employees.Remove(employee);

                /* for this day the employee has no assignments */
                if (!await _context.Schedules.AnyAsync(a => a.Employee == employee && a.Day == day))
                {
                    freeEmployees.Add(new FreeEmployeeDto
                    {
                        EmployeeEmail = employee.Profile.Email ?? string.Empty,
                        EmployeeName = employee.Profile.FirstName + " " + employee.Profile.LastName,
                        RestaurantName = restaurant.Name,
                        /* From and To are null => no assignments */
                    });
                }

                /* For this day the employee has some assignments */
                freeEmployees.Add(new FreeEmployeeDto
                {
                    EmployeeEmail = employee.Profile.Email ?? string.Empty,
                    EmployeeName = employee.Profile.FirstName + " " + employee.Profile.LastName,
                    RestaurantName = restaurant.Name,
                    From = _context.Schedules.Where(a => a.Employee == employee && a.Day == day).OrderBy(a => a.From).Select(a => a.From).First(),
                    To = _context.Schedules.Where(a => a.Employee == employee && a.Day == day).OrderByDescending(a => a.To).Select(a => a.To).First(),
                });
            };

            return freeEmployees;
        }
        public async Task<List<ManagerFullScheduleDto>> GetManagerFullSchedule(Restaurant restaurant, int month)
        {
            List <ManagerFullScheduleDto> fullSchedule = new List<ManagerFullScheduleDto>();
            foreach (var assignment in _context.Schedules.Where(a => a.Day.Month == month && a.Restaurant == restaurant && a.IsWorkDay))
            {
                fullSchedule.Add(new ManagerFullScheduleDto
                {
                    ScheduleId = assignment.Id.ToString(),
                    Day = assignment.Day,
                    PeopleAssignedToWork = await _context.Schedules.Where(a => a.Day == assignment.Day && a.Restaurant == restaurant && a.IsWorkDay).CountAsync(),
                });
            }
            return fullSchedule;
        }
        public List<ManagerDailyScheduleDto> GetManagerDailySchedule(Restaurant restaurant, DateOnly day)
        {
            List <ManagerDailyScheduleDto> dailySchedule = new List<ManagerDailyScheduleDto>();

            foreach (var assignment in _context.Schedules.Where(a => a.Day == day && a.Restaurant == restaurant))
            {
                dailySchedule.Add(new ManagerDailyScheduleDto
                {
                    ScheduleId = assignment.Id.ToString(),
                    EmployeeEmail = assignment.Employee.Profile.Email ?? string.Empty,
                    EmployeeName = assignment.Employee.Profile.FirstName + " " + assignment.Employee.Profile.LastName,
                    RestaurantName = restaurant.Name,
                    IsFullDay = assignment.FullDay,
                    IsWorkDay = assignment.IsWorkDay,
                    To = assignment.To,
                    From = assignment.From,
                });
            }
            return dailySchedule;
        }

        /* Possible issue: OrderBy => OrderByAscending */
        public async Task<bool> IsEmployeeFreeToWork(Employee employee, DateOnly day, TimeOnly? from, TimeOnly? to)
        {
            /* Gets both worked and leave days of employee */
            List<Schedule> assignedDays = await GetEmployeeSchedule(employee);

            /* Gets the schedule of the employee on this day */
            /* The employee has multiple assignments on this day (8-11 + 12-15 + 20-23) */
            if (assignedDays.Where(ad => ad.Day == day).ToList().Count > 1)
            {
                /* Orders the scheduled assignments of the employee by their time of beginning */
                List<Schedule> orderedSchedule = assignedDays.Where(ad => ad.Day == day).OrderBy(ad => ad.From).ToList();

                /* Loops through them and checks if there is a suitable gap */
                for (int i = 0; i < orderedSchedule.Count - 1; i++)
                    if (from > orderedSchedule[i].To && to < orderedSchedule[i + 1].From)
                        return true;

                /* After the loop there was not a suitable gap */
                return false;
            }

            /* The employee has one or none assignments for the day */
            Schedule? schedule = assignedDays.FirstOrDefault(wd => wd.Day == day);

            /* The employee is free on this day */
            if (schedule == null) return true;

            /* The employee is not free on this day */
            /* It has to work full day */
            if (from == null || to == null) return false;

            /* It has to work certain hours */
            /* The schedule of the employee on this day is already full */
            if (schedule.FullDay) return false;

            /* The schedule of the employee on this day is not full */
            /* The new assignment is for later (8-14 + 16-20) */
            if (from > schedule.To) return true;

            /* The new assignment is for earlier (16-20 + 8-14) */
            if (schedule.From > from) return true;

            return false;
        }
        public async Task<bool> IsAssignmentForWork(string scheduleId)
        {
            return await _context.Schedules.AnyAsync(a => a.IsWorkDay && a.Id.ToString() == scheduleId);
        }
        public async Task<bool> DeleteAssignment(string scheduleId)
        {
            try
            {
                _context.Schedules.Remove(await GetEmployeeAssignment(scheduleId));
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
        private async Task<List<Schedule>> GetEmployeeSchedule(Employee employee)
        {
            return await _context.Schedules.Where(s => s.Employee == employee).ToListAsync();
        }
        private async Task<Schedule> GetEmployeeAssignment(string scheduleId)
        {
            return await _context.Schedules.FirstAsync(s => s.Id.ToString() == scheduleId);
        }
        private async Task<Schedule> AddAssignmentToScheduleInternal(DateOnly day, string employeeEmail, string restaurantId, TimeOnly? from, TimeOnly? to, bool isWorkDay)
        {
            Schedule schedule = new Schedule
            {
                Day = day,
                Employee = await _employeeService.GetEmployeeByEmail(employeeEmail),
                Restaurant = await _restaurantService.GetRestaurantById(restaurantId),
                AssignedOn = DateTime.Now,
                From = from,
                To = to,
                FullDay = (to == null || from == null),
                IsWorkDay = isWorkDay
            };

            await _context.Schedules.AddAsync(schedule);

            return schedule;
        }

    }
}
