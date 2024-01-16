using Microsoft.EntityFrameworkCore;
using webapi.Data;
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
        public async Task<Schedule> AddAssignmentToSchedule(ScheduleDetailsDto model)
        {
            Schedule schedule = new Schedule
            {
                Day = model.Day,
                Employee = await _employeeService.GetEmployeeByEmail(model.EmployeeEmail),
                Restaurant = await _restaurantService.GetRestaurantById(model.RestaurantId),
                AssignedOn = DateTime.Now,
                From = model.From,
                To = model.To,
                Reason = model.Reason,
                FullDay = (model.To == null || model.From == null),
                IsWorkDay = model.IsWorkDay
            };

            await _context.Schedules.AddAsync(schedule);

            return schedule;
        }
        public async Task<Schedule> EditScheduleAssignment(ScheduleDetailsDto model)
        {
            Schedule schedule = await GetEmployeeAssignment(model.ScheduleId);
            
            schedule.Day = model.Day;
            schedule.Employee = await _employeeService.GetEmployeeByEmail(model.EmployeeEmail);
            schedule.Restaurant = await _restaurantService.GetRestaurantById(model.RestaurantId);
            schedule.From = model.From;
            schedule.To = model.To;
            schedule.Reason = model.Reason;
            schedule.FullDay = (model.To == null || model.From == null);
            schedule.IsWorkDay = model.IsWorkDay;

            _context.Schedules.Update(schedule);

            return schedule;
        }
        public async Task<bool> DoesScheduleExistsById(string scheduleId)
        {
            return await _context.Schedules.AnyAsync(a => a.Id.ToString() == scheduleId);
        }
        public async Task<ScheduleDetailsDto> GetAssignmentDetails(string scheduleId)
        {
            Schedule schedule = await GetEmployeeAssignment(scheduleId);

            ScheduleDetailsDto assignment = new ScheduleDetailsDto
            {
                ScheduleId = schedule.Id.ToString(),
                Day = schedule.Day,
                EmployeeEmail = schedule.Employee.Profile.Email ?? string.Empty,
                RestaurantId = schedule.Restaurant.Id.ToString(),
                Reason = schedule.Reason,
                From = schedule.From,
                To = schedule.To,
                FullDay = schedule.FullDay,
                IsWorkDay = schedule.IsWorkDay
            };

            return assignment;
        }
        public async Task<List<ScheduleBrowseDto>> GetAssignedDaysOfEmployee(Employee employee, int month)
        {
            List<ScheduleBrowseDto> schedule = new List<ScheduleBrowseDto>();

            foreach (var assignment in await _context.Schedules.Where(s => s.Employee == employee && s.Day.Month == month).ToListAsync())
            {
                schedule.Add(new ScheduleBrowseDto
                {
                    ScheduleId = assignment.Id.ToString(),
                    Day = assignment.Day,
                    EmployeeEmail = assignment.Employee.Profile.Email ?? string.Empty,
                    RestaurantId = assignment.Restaurant.Id.ToString(),
                    IsWorkDay = assignment.IsWorkDay
                });
            }

            return schedule;
        }
        public async Task<List<ScheduleBrowseDto>> GetAssignedDaysOfEmployeeInRestaurant(Employee employee, Restaurant restaurant, int month)
        {
            List<ScheduleBrowseDto> schedule = new List<ScheduleBrowseDto>();

            foreach (var assignment in await _context.Schedules.Where(s => s.Employee == employee && s.Restaurant == restaurant && s.Day.Month == month).ToListAsync())
            {
                schedule.Add(new ScheduleBrowseDto
                {
                    ScheduleId = assignment.Id.ToString(),
                    Day = assignment.Day,
                    EmployeeEmail = assignment.Employee.Profile.Email ?? string.Empty,
                    RestaurantId = assignment.Restaurant.Id.ToString(),
                    IsWorkDay = assignment.IsWorkDay
                });
            }

            return schedule;
        }
        public async Task<List<ScheduleBrowseDto>> GetDailyEmployeeSchedule(Employee employee, DateOnly day)
        {
            List<ScheduleBrowseDto> schedule = new List<ScheduleBrowseDto>();

            foreach (var assignment in await _context.Schedules.Where(s => s.Employee == employee && s.Day == day).ToListAsync())
            {
                schedule.Add(new ScheduleBrowseDto
                {
                    ScheduleId = assignment.Id.ToString(),
                    Day = assignment.Day,
                    EmployeeEmail = assignment.Employee.Profile.Email ?? string.Empty,
                    RestaurantId = assignment.Restaurant.Id.ToString(),
                    IsWorkDay = assignment.IsWorkDay
                });
            }

            return schedule;
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
    }
}
