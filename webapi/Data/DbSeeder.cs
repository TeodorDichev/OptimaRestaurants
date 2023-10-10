using Microsoft.AspNetCore.Identity;
using System.Data;
using webapi.Models;

namespace webapi.Data
{
    public static class DbSeeder
    {
        public static async Task SeedRolesAndAdminAsync(IServiceProvider service)
        {
            UserManager<ApplicationUser> userManager = service.GetService<UserManager<ApplicationUser>>();
            RoleManager<IdentityRole> roleManager = service.GetService<RoleManager<IdentityRole>>();
            await roleManager.CreateAsync(new IdentityRole(Roles.Employer.ToString()));
            await roleManager.CreateAsync(new IdentityRole(Roles.Employee.ToString()));

            var user = new ApplicationUser
            {
                Email = "teodichev@gmail.com",
                FirstName = "Teodor",
                LastName = "Dichev",
                PhoneNumber = "0896112324",
                EmailConfirmed = true,
                PhoneNumberConfirmed = true
            };

            var userInDb = await userManager.FindByEmailAsync(user.Email);
            if (userInDb == null)
            {
                await userManager.CreateAsync(user, "teodichev@gmail.com");
                await userManager.AddToRoleAsync(user, Roles.Employer.ToString());
            }
        }
    }
}
