using FamilyBudget.Server.Data;
using FamilyBudget.Server.Infractructure.Configuration;
using FamilyBudget.Server.Infractructure.Middleware;
using FamilyBudget.Server.Models;
using FamilyBudget.Server.Services.Budgets;
using FamilyBudget.Server.Services.Identity;
using FamilyBudget.Shared.Users;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

//Add configuration
builder.Services.Configure<DbContextConfiguration>(builder.Configuration.GetSection(DbContextConfiguration.SectionName));

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<ApplicationUser>()
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddIdentityServer()
    .AddApiAuthorization<ApplicationUser, ApplicationDbContext>();


var dataConfiguration = new DataConfiguration();
builder.Configuration.Bind(DataConfiguration.SectionName, dataConfiguration);

if (dataConfiguration.DisablePasswordRequirements)
{
    builder.Services.Configure<IdentityOptions>(options =>
    {
        options.Password.RequireDigit = false;
        options.Password.RequireLowercase = false;
        options.Password.RequireNonAlphanumeric = false;
        options.Password.RequireUppercase = false;
        options.Password.RequiredLength = 1;
        options.Password.RequiredUniqueChars = 0;
    });
}

builder.Services.AddAuthentication()
    .AddIdentityServerJwt();

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

//services
builder.Services.AddScoped<IUserProvider, UserProvider>();
builder.Services.AddTransient<IUserService, UserService>();
builder.Services.AddScoped<IBudgetService, BudgetService>();
builder.Services.AddScoped<IBudgetEntriesService, BudgetEntriesService>();
builder.Services.AddScoped<IBudgetEntryCategoriesService, BudgetEntryCategoriesService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseHttpsRedirection();

app.UseBlazorFrameworkFiles();
app.UseStaticFiles();

app.UseRouting();

app.UseIdentityServer();
app.UseAuthorization();

app.MapRazorPages();
app.MapControllers();
app.MapFallbackToFile("index.html");


using var scope = app.Services.CreateScope();
var services = scope.ServiceProvider;
try
{
    var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
    var context = services.GetRequiredService<ApplicationDbContext>();


    var adminRoleExists = await context.Roles.AnyAsync(x => x.Name == Roles.Admin);

    if (!adminRoleExists)
    {
        await roleManager.CreateAsync(new IdentityRole(Roles.Admin));
    }

    await context.Database.MigrateAsync();
    await Seed.SeedData(context, userManager, dataConfiguration);
}
catch (Exception ex)
{
    var logger = services.GetRequiredService<ILogger<Program>>();
    logger.LogError(ex, "An error occurred during migration");
}

app.Run();
