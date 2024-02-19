using Microsoft.EntityFrameworkCore;
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

        public async Task<Schedule> AddAssignmentToSchedule(ScheduleDto model)
        {
            return await AddAssignmentToScheduleInternal(model.Day, model.EmployeeEmail, model.RestaurantId, model.From, model.To, model.IsWorkDay);
        }
        public async Task<Schedule> AddAssignmentToSchedule(CreateScheduleDto model)
        {
            return await AddAssignmentToScheduleInternal(model.Day, model.EmployeeEmail, model.RestaurantId, model.From, model.To, model.IsWorkDay);
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
                    Day = assignment.Day.ToDateTime(TimeOnly.Parse("0:00:00")),
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
                    Day = assignment.Day.ToDateTime(TimeOnly.Parse("0:00:00")),
                    IsWorkDay = assignment.IsWorkDay
                });
            }

            return schedule;
        }
        public List<EmployeeDailyScheduleDto> GetEmployeeDailySchedule(Employee employee, DateTime day)
        {
            List<EmployeeDailyScheduleDto> schedule = new List<EmployeeDailyScheduleDto>();

            foreach (var assignment in _context.Schedules.Where(s => s.Employee == employee && s.Day == DateOnly.FromDateTime(day)).OrderBy(s => s.From))
            {
                schedule.Add(new EmployeeDailyScheduleDto
                {
                    ScheduleId = assignment.Id.ToString(),
                    IsWorkDay = assignment.IsWorkDay,
                    From = TimeOnlyToDateTime(day, assignment.From),
                    To = TimeOnlyToDateTime(day, assignment.To),
                    RestaurantName = assignment.Restaurant.Name,
                    RestaurantId = assignment.Restaurant.Id.ToString(),
                    IsFullDay = assignment.FullDay
                });
            }

            return schedule;
        }
        public async Task<List<FreeEmployeeDto>> GetFreeEmployees(Restaurant restaurant, DateTime day)
        {
            List<Employee> employees = restaurant.EmployeesRestaurants.Where(er => er.EndedOn == null).Select(er => er.Employee).ToList();
            List<FreeEmployeeDto> freeEmployees = new List<FreeEmployeeDto>();

            foreach (var employee in employees)
            {
                /* If this employee has a full day work/leisure assignment for this day in any restaurant then it cannot work on this day -> not adding it */
                if (await _context.Schedules.AnyAsync(a => a.Employee == employee && a.FullDay && a.Day == DateOnly.FromDateTime(day))) continue;

                /* for this day the employee has no assignments */
                else if (!await _context.Schedules.AnyAsync(a => a.Employee == employee && a.Day == DateOnly.FromDateTime(day)))
                {
                    freeEmployees.Add(new FreeEmployeeDto
                    {
                        EmployeeEmail = employee.Profile.Email ?? string.Empty,
                        EmployeeName = employee.Profile.FirstName + " " + employee.Profile.LastName,
                        RestaurantName = restaurant.Name,
                        /* From and To are null => no assignments */
                    });
                }
                else
                {
                    /* For this day the employee has some assignments */

                    freeEmployees.Add(new FreeEmployeeDto
                    {
                        EmployeeEmail = employee.Profile.Email ?? string.Empty,
                        EmployeeName = employee.Profile.FirstName + " " + employee.Profile.LastName,
                        RestaurantName = restaurant.Name,
                        From = _context.Schedules.Where(a => a.Employee == employee && a.Day == DateOnly.FromDateTime(day)).OrderBy(a => a.From).Select(a => a.From).First(),
                        To = _context.Schedules.Where(a => a.Employee == employee && a.Day == DateOnly.FromDateTime(day)).OrderByDescending(a => a.To).Select(a => a.To).First(),
                    });
                }
            };

            return freeEmployees;
        }
        public async Task<List<ManagerFullScheduleDto>> GetManagerFullSchedule(Restaurant restaurant, int month)
        {
            List<ManagerFullScheduleDto> fullSchedule = new List<ManagerFullScheduleDto>();
            foreach (var assignment in _context.Schedules.Where(a => a.Day.Month == month && a.Restaurant == restaurant && a.IsWorkDay))
            {
                fullSchedule.Add(new ManagerFullScheduleDto
                {
                    ScheduleId = assignment.Id.ToString(),
                    Day = assignment.Day.ToDateTime(TimeOnly.Parse("0:00:00")),
                    PeopleAssignedToWork = await _context.Schedules.Where(a => a.Day == assignment.Day && a.Restaurant == restaurant && a.IsWorkDay).CountAsync(),
                });
            }
            return fullSchedule;
        }
        public List<ManagerDailyScheduleDto> GetManagerDailySchedule(Restaurant restaurant, DateTime day)
        {
            List<ManagerDailyScheduleDto> dailySchedule = new List<ManagerDailyScheduleDto>();

            foreach (var assignment in _context.Schedules.Where(a => a.Day == DateOnly.FromDateTime(day) && a.Restaurant == restaurant))
            {
                dailySchedule.Add(new ManagerDailyScheduleDto
                {
                    ScheduleId = assignment.Id.ToString(),
                    EmployeeEmail = assignment.Employee.Profile.Email ?? string.Empty,
                    EmployeeName = assignment.Employee.Profile.FirstName + " " + assignment.Employee.Profile.LastName,
                    RestaurantName = restaurant.Name,
                    IsFullDay = assignment.FullDay,
                    IsWorkDay = assignment.IsWorkDay,
                    To = TimeOnlyToDateTime(day, assignment.To),
                    From = TimeOnlyToDateTime(day, assignment.From)
                });
            }
            return dailySchedule;
        }
        public async Task<bool> CanEmployeeTakeVacationOn(Employee employee, Restaurant restaurant, DateTime day, DateTime? from, DateTime? to)
        {
            /* Gets both worked and leave days of employee */
            List<Schedule> assignedDays = await GetEmployeeSchedule(employee);

            /* Gets all working assignments of the employee for the day */
            if (!IsEmployeeFree(assignedDays
                .Where(ad => ad.Day == DateOnly.FromDateTime(day) && ad.IsWorkDay)
                .OrderBy(ad => ad.From).
                ToList(), from, to))
                return false;

            /* Gets all non-working assignments of the employee for the day in the specified restaurant */
            return IsEmployeeFree(assignedDays
                .Where(ad => ad.Day == DateOnly.FromDateTime(day) && !ad.IsWorkDay && ad.Restaurant.Id == restaurant.Id)
                .OrderBy(ad => ad.From)
                .ToList(), from, to);
        }
        public async Task<bool> CanEmployeeWorkOn(Employee employee, DateTime day, DateTime? from, DateTime? to)
        {
            /* Gets both worked and leave days of employee */
            List<Schedule> assignedDays = await GetEmployeeSchedule(employee);

            /* Orders the scheduled assignments of the employee by their time of beginning */
            return IsEmployeeFree(assignedDays
                .Where(ad => ad.Day == DateOnly.FromDateTime(day))
                .OrderBy(ad => ad.From)
                .ToList(), from, to);
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
        public async Task<CreateScheduleDto> CreateScheduleDto(string scheduleId)
        {
            Schedule schedule = await GetEmployeeAssignment(scheduleId);
            CreateScheduleDto oldSchedule = new CreateScheduleDto()
            {
                Day = schedule.Day.ToDateTime(TimeOnly.Parse("0:00:00")),
                From = TimeOnlyToDateTime(schedule.Day.ToDateTime(TimeOnly.Parse("0:00:00")), schedule.From),
                To = TimeOnlyToDateTime(schedule.Day.ToDateTime(TimeOnly.Parse("0:00:00")), schedule.To),
                RestaurantId = schedule.Restaurant.Id.ToString(),
                EmployeeEmail = schedule.Employee.Profile.Email,
                FullDay = schedule.FullDay,
                IsWorkDay = schedule.IsWorkDay,
            };

            return oldSchedule;
        }
        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
        private bool IsEmployeeFree(List<Schedule> orderedSchedule, DateTime? from, DateTime? to)
        {
            /* The employee has nothing assigned for the day */
            if (orderedSchedule.Count <= 0) return true;

            /* The employee has a single assignment for the day */
            if (orderedSchedule.Count == 1)
            {
                /* The new assignment is for a full day but the employee already has another one */
                if (from == null || to == null) return false;

                /* The existing assignment of the employee is for a full day */
                if (orderedSchedule.First().FullDay) return false;

                /* The new assignment is for later compared to the existing assignment (8-14 + 16-20) */
                if (TimeOnly.FromDateTime(from.Value) >= orderedSchedule.First().To) return true;

                /* The new assignment is for earlier compared to the existing assignment (16-20 + 8-14) */
                if (orderedSchedule.First().From >= TimeOnly.FromDateTime(from.Value)) return true;
            }

            /* The employee has a multiple assignments for the day */
            if (orderedSchedule.Count > 1)
            {
                /* The new assignment is for a full day but the employee already has another few */
                if (from == null || to == null) return false;

                /* The new assignment is for earlier compared to the existing assignments (16-20, 20-22 + 8-14) */
                if (orderedSchedule.First().From > TimeOnly.FromDateTime(from.Value)) return true;

                /* The new assignment is between existing assignments (8-14 + 16-20 + 20-22) */
                for (int i = 0; i < orderedSchedule.Count - 1; i++)
                    if (TimeOnly.FromDateTime(from.Value) >= orderedSchedule[i].To && TimeOnly.FromDateTime(to.Value) <= orderedSchedule[i + 1].From)
                        return true;

                /* The new assignment is for later compared to the existing assignments (8-12, 12-14 + 16-20) */
                if (TimeOnly.FromDateTime(from.Value) >= orderedSchedule.Last().To) return true;
            }

            return false;
        }
        private async Task<List<Schedule>> GetEmployeeSchedule(Employee employee)
        {
            return await _context.Schedules.Where(s => s.Employee == employee).ToListAsync();
        }
        private async Task<Schedule> GetEmployeeAssignment(string scheduleId)
        {
            return await _context.Schedules.FirstAsync(s => s.Id.ToString() == scheduleId);
        }
        private async Task<Schedule> AddAssignmentToScheduleInternal(DateTime day, string employeeEmail, string restaurantId, DateTime? from, DateTime? to, bool isWorkDay)
        {
            Schedule schedule = new Schedule
            {
                Day = DateOnly.FromDateTime(day),
                Employee = await _employeeService.GetEmployeeByEmail(employeeEmail),
                Restaurant = await _restaurantService.GetRestaurantById(restaurantId),
                AssignedOn = DateTime.Now,
                FullDay = (to == null || from == null),
                IsWorkDay = isWorkDay
            };

            if (from != null) schedule.From = TimeOnly.FromDateTime((DateTime)from);
            else schedule.From = null;
            if (to != null) schedule.To = TimeOnly.FromDateTime((DateTime)to);
            else schedule.To = null;

            await _context.Schedules.AddAsync(schedule);

            return schedule;
        }
        private DateTime? TimeOnlyToDateTime(DateTime day, TimeOnly? time)
        {
            if (time == null) return null;
            return day.Date + time.Value.ToTimeSpan();
        }
    }
}
