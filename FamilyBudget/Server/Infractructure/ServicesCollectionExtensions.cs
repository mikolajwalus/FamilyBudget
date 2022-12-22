using FamilyBudget.Client.Services;
using FamilyBudget.Client.Services.Identity;
using FamilyBudget.Server.Data;
using FamilyBudget.Server.Infractructure.Configuration;
using FamilyBudget.Server.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace FamilyBudget.Server.Infractructure
{
    public static class ServicesCollectionExtensions
    {
        public const string RoleClaimName = "role";

        public static void AddServices(this IServiceCollection services)
        {
            services.AddScoped<IUserProvider, UserProvider>();
            services.AddTransient<IUserService, UserService>();
            services.AddScoped<IBudgetService, BudgetService>();
            services.AddScoped<IBudgetEntriesService, BudgetEntriesService>();
            services.AddScoped<IBudgetEntryCategoriesService, BudgetEntryCategoriesService>();
        }

        public static void AddData(this IServiceCollection services, ConfigurationManager configuration)
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(connectionString));
            services.AddDatabaseDeveloperPageExceptionFilter();
        }

        public static void AddIdentity(this IServiceCollection services, ConfigurationManager configuration)
        {
            services.AddDefaultIdentity<ApplicationUser>()
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>();

            services.AddIdentityServer()
                .AddApiAuthorization<ApplicationUser, ApplicationDbContext>(options =>
                {
                    options.IdentityResources["openid"].UserClaims.Add(RoleClaimName);
                    options.ApiResources.Single().UserClaims.Add(RoleClaimName);
                });

            System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler
                            .DefaultInboundClaimTypeMap.Remove(RoleClaimName);

            var dataConfiguration = new DataConfiguration();
            configuration.Bind(DataConfiguration.SectionName, dataConfiguration);

            if (dataConfiguration.DisablePasswordRequirements)
            {
                services.Configure<IdentityOptions>(options =>
                {
                    options.Password.RequireDigit = false;
                    options.Password.RequireLowercase = false;
                    options.Password.RequireNonAlphanumeric = false;
                    options.Password.RequireUppercase = false;
                    options.Password.RequiredLength = 1;
                    options.Password.RequiredUniqueChars = 0;
                });
            }

            services.AddAuthentication()
                .AddIdentityServerJwt();
        }
    }
}
