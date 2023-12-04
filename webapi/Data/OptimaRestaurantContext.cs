using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using webapi.Models;

namespace webapi.Data;

public class OptimaRestaurantContext : IdentityDbContext<ApplicationUser>
{
    public OptimaRestaurantContext(DbContextOptions<OptimaRestaurantContext> options)
        : base(options)
    { }
    public DbSet<Manager> Managers { get; set; }
    public DbSet<Restaurant> Restaurants { get; set; }
    public DbSet<Employee> Employees { get; set; }
    public DbSet<Request> Requests { get; set; }
    public DbSet<Transfer> Transfers { get; set; }
    public DbSet<EmployeeRestaurant> EmployeesRestaurants { get; set; }
    public DbSet<CustomerReview> CustomerReviews { get; set; }
    public DbSet<ManagerReview> ManagerReviews { get; set; }
    public DbSet<ShiftType> ShiftTypes { get; set; }
    public DbSet<Shift> Shifts { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
    }
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseLazyLoadingProxies();
    }
}
