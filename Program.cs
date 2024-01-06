using AccountingAssistant;
using AccountingAssistant.Clients;
using AccountingAssistant.Services;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddHttpClient<AssistantClient>();
builder.Services.AddScoped<IAssistantService, AssistantService>();


await builder.Build().RunAsync();
