using Microsoft.AspNetCore.Identity;
using webapi.Models;

namespace webapi.Data
{
    public static class DbSeeder
    {
        public static async Task SeedRoles(IServiceProvider service)
        {
            UserManager<ApplicationUser> userManager = service.GetService<UserManager<ApplicationUser>>();
            RoleManager<IdentityRole> roleManager = service.GetService<RoleManager<IdentityRole>>();
            await roleManager.CreateAsync(new IdentityRole(Role.Employer.ToString()));
            await roleManager.CreateAsync(new IdentityRole(Role.Employee.ToString()));
        }
        public static async Task Seed(IServiceProvider service)
        {
            var context = service.GetService<OptimaRestaurantContext>();
            if (!context.ShiftTypes.Any())
            {
                //can add more and edit the existing
                await context.ShiftTypes.AddRangeAsync(new List<ShiftType>()
                    {
                      new ShiftType()
                      {
                          Name = "Workday 9:00-17:00"
                      },

                      new ShiftType()
                      {
                          Name = "Workday 8:00-15:00"
                      },

                      new ShiftType()
                      {
                          Name = "Workday 15:00-22:00"
                      },

                      new ShiftType()
                      {
                          Name = "Holiday"
                      }
                    });
            }
            await context.SaveChangesAsync();
        }
    }
}
