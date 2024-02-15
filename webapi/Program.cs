using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using webapi.Data;
using webapi.Models;
using webapi.Services;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using webapi.Services.FileServices;
using webapi.Services.ClassServices;
using webapi.Services.ModelServices;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<OptimaRestaurantContext>(options => 
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("OptimaRestaurantContextConnection"));
});

builder.Services.AddScoped<JWTService>();
builder.Services.AddScoped<EmailService>();
builder.Services.AddScoped<QrCodesService>();
builder.Services.AddScoped<PdfFilesService>();
builder.Services.AddScoped<ContextSeedService>();
builder.Services.AddScoped<PicturesAndIconsService>();

builder.Services.AddScoped<ReviewService>();
builder.Services.AddScoped<RequestService>();
builder.Services.AddScoped<AccountService>();
builder.Services.AddScoped<ManagerService>();
builder.Services.AddScoped<EmployeeService>();
builder.Services.AddScoped<ScheduleService>();
builder.Services.AddScoped<RestaurantService>();

/* Defining identity core services */
builder.Services.AddIdentityCore<ApplicationUser>(options =>
{
    /* password configuration */
    options.Password.RequiredLength = 6;
    options.Password.RequireDigit = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireNonAlphanumeric = false;

    /* email configuration */
    options.SignIn.RequireConfirmedEmail = false;
    options.User.RequireUniqueEmail = true;
})
    .AddRoles<IdentityRole>()
    .AddRoleManager<RoleManager<IdentityRole>>()
    .AddEntityFrameworkStores<OptimaRestaurantContext>()
    .AddSignInManager<SignInManager<ApplicationUser>>()
    .AddDefaultTokenProviders();

/* JWT authentication settings */
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:Key"])),
            ValidIssuer = builder.Configuration["JWT:Issuer"],
            ValidateIssuer = true,
            ValidateAudience = true
        };
    });

builder.Services.AddCors();

builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.InvalidModelStateResponseFactory = actionContext =>
    {
        var errors = actionContext.ModelState
        .Where(x => x.Value.Errors.Count > 0)
        .SelectMany(x => x.Value.Errors)
        .Select(x => x.ErrorMessage).ToArray();

        var toReturn = new
        {
            Errors = errors
        };

        return new BadRequestObjectResult(toReturn);
    };
});

var app = builder.Build();

/* HTTP request pipeline configuration and CORS policies */
app.UseCors(opt =>
{
    opt.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin();
});

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//remove when remote
//app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

/* Seeding roles to the database */
using (var scope = app.Services.CreateScope())
{
    try
    {
        var contextSeedServices = scope.ServiceProvider.GetService<ContextSeedService>();
        await contextSeedServices.InitializeContextAsync();
    }
    catch (Exception ex)
    {
        var logger = scope.ServiceProvider.GetService<ILogger<Program>>();
        logger.LogError(ex.Message, "Failed to initialize and seed database ");    
    }
}

app.Run();