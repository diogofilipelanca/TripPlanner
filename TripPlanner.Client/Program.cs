using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Radzen;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.Services.AddHttpClient("api", client => {
    client.BaseAddress = new Uri("https://localhost:1000/");
});

builder.Services.AddRadzenComponents();

await builder.Build().RunAsync();