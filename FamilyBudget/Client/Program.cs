using FamilyBudget.Client;
using FamilyBudget.Client.Services;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Radzen;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddHttpClient("FamilyBudget.ServerAPI", client => client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress))
    .AddHttpMessageHandler<BaseAddressAuthorizationMessageHandler>();

// Supply HttpClient instances that include access tokens when making requests to the server project
builder.Services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient("FamilyBudget.ServerAPI"));

builder.Services.AddApiAuthorization();

//Add radzen
builder.Services.AddScoped<NotificationService>();

//Add services
builder.Services.AddScoped<IBudgetService, BudgetService>();
builder.Services.AddScoped<IUserProvider, UserProvider>();

await builder.Build().RunAsync();