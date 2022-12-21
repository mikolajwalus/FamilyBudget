using FamilyBudget.Client;
using FamilyBudget.Client.Services;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Radzen;
using Toolbelt.Blazor.Extensions.DependencyInjection;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddHttpClient("FamilyBudget.ServerAPI", (sp, client) =>
    {
        client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress);
        client.EnableIntercept(sp);
    })
    .AddHttpMessageHandler<BaseAddressAuthorizationMessageHandler>();

builder.Services.AddHttpClientInterceptor();
builder.Services.AddScoped<HttpInterceptorService>();

// Supply HttpClient instances that include access tokens when making requests to the server project
builder.Services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient("FamilyBudget.ServerAPI"));

builder.Services.AddApiAuthorization();

//Add radzen
builder.Services.AddScoped<NotificationService>();

//Add services
builder.Services.AddTransient<IBudgetService, BudgetService>();
builder.Services.AddTransient<IBudgetEntriesService, BudgetEntriesService>();
builder.Services.AddTransient<IBudgetEntryCategoriesService, BudgetEntryCategoriesService>();
builder.Services.AddTransient<IUserProvider, UserProvider>();

await builder.Build().RunAsync();