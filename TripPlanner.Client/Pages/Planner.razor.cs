using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Net.Http.Json;
using TripPlanner.Client.Models;

namespace TripPlanner.Client.Pages {
    public partial class Planner {
        [Inject] IJSRuntime JSRuntime { get; set; } = default!;
        [Inject] IHttpClientFactory Http { get; set; } = default!;
        protected override async Task OnAfterRenderAsync(bool firstRender) {
            if (firstRender) {
                var client = Http.CreateClient("api");
                var cities = await client.GetFromJsonAsync<List<City>>("api/cities");
                var trips = await client.GetFromJsonAsync<List<Trip>>("api/trips");

                var nodes = cities?.Select(c => new {
                    id = c.Name
                });
                var links = trips?.Select(t => new {
                    source = t.Origin,
                    target = t.Destination
                });

                var graphData = new {
                    nodes,
                    links
                };

                await JSRuntime.InvokeVoidAsync("renderGraph", graphData);
            }
            await base.OnAfterRenderAsync(firstRender);
        }
    }
}
