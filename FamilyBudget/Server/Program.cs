using FamilyBudget.Server.Infractructure;
using FamilyBudget.Server.Infractructure.Configuration;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<DbContextConfiguration>(builder.Configuration.GetSection(DbContextConfiguration.SectionName));

builder.Services.AddData(builder.Configuration);

builder.Services.AddIdentity(builder.Configuration);

builder.Services.AddControllersWithViews();

builder.Services.AddRazorPages();

builder.Services.AddServices();

var app = builder.Build();

app.ConfigureRequestPipeline();

await app.PerformStartupDatabaseTasks();

app.Run();
