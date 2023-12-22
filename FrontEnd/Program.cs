using Blazored.LocalStorage;
using Blazored.SessionStorage;
using FrontEnd;
using FrontEnd.Components;
using FrontEnd.Constants;
using FrontEnd.Handlers;
using FrontEnd.Services;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);


builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddBlazoredLocalStorage();
builder.Services.AddBlazoredSessionStorage();

builder.Services.AddTransient<TokenService>();
builder.Services.AddSingleton<Events>();
builder.Services.AddTransient<RefreshTokenHandler>();


builder.Services.AddHttpClient(ApiConfigs.AdvertisementClient, client =>
    {
        ///
        client.BaseAddress = new Uri(ApiConfigs.RootApiAddress);
    })
    .AddHttpMessageHandler<RefreshTokenHandler>();


try
{
    await builder.Build().RunAsync();
}
catch (Exception e)
{
    Console.Error.WriteLine(e);
}