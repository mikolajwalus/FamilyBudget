using FamilyBudget.Server.Data;
using FamilyBudget.Server.Infractructure.Configuration;
using FamilyBudget.Server.Infractructure.Middleware;
using FamilyBudget.Server.Models;
using FamilyBudget.Shared.Users;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace FamilyBudget.Server.Infractructure
{
    public static class WebApplicationExtensions
    {
        public static void ConfigureRequestPipeline(this WebApplication app)
        {
            if (app.Environment.IsDevelopment())
            {
                app.UseMigrationsEndPoint();
                app.UseWebAssemblyDebugging();
            }
            else
            {
                app.UseExceptionHandler("/Error");
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
        }

        public static async Task PerformStartupDatabaseTasks(this WebApplication app)
        {
            var dataConfiguration = new DataConfiguration();
            app.Configuration.Bind(DataConfiguration.SectionName, dataConfiguration);

            using var scope = app.Services.CreateScope();
            var services = scope.ServiceProvider;
            try
            {
                var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
                var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
                var context = services.GetRequiredService<ApplicationDbContext>();

                await context.Database.EnsureCreatedAsync();

                var adminRoleExists = await context.Roles.AnyAsync(x => x.Name == Roles.Admin);

                if (!adminRoleExists)
                {
                    await roleManager.CreateAsync(new IdentityRole(Roles.Admin));
                }

                await Seed.SeedData(context, userManager, dataConfiguration);
            }
            catch (Exception ex)
            {
                var logger = services.GetRequiredService<ILogger<Program>>();
                logger.LogError(ex, "An error occurred during migration");
            }
        }
    }
}
