using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using AgroSolutions.Dashboard;
using AgroSolutions.Dashboard.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// Registrar AuthStateService como Singleton
builder.Services.AddSingleton<AuthStateService>();

// Configurar HttpClient para Property API
builder.Services.AddHttpClient<PropertyService>(client =>
{
    client.BaseAddress = new Uri("http://localhost:5002/");
});

// Configurar HttpClient para Field Service (usa Property API)
builder.Services.AddHttpClient<FieldService>(client =>
{
    client.BaseAddress = new Uri("http://localhost:5002/");
});

// Configurar HttpClient para Identity API
builder.Services.AddHttpClient<AuthService>(client =>
{
    client.BaseAddress = new Uri("http://localhost:5001/");
});

// Configurar HttpClient para Alerts API
builder.Services.AddHttpClient<AlertService>(client =>
{
    client.BaseAddress = new Uri("http://localhost:5004/");
});

await builder.Build().RunAsync();