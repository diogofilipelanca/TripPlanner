using Microsoft.AspNetCore.Components;
using System.Net.Http.Json;
using TripPlanner.Client.Enums;
using TripPlanner.Client.Models;

namespace TripPlanner.Client.Pages.Trips {
    public partial class TripsList {
        [Inject] IHttpClientFactory Http { get; set; } = default!;
        [Inject] NavigationManager NavigationManager { get; set; } = default!;
        public List<Trip> trips { get; set; } = new();
        protected override async Task OnInitializedAsync() {
            var client = Http.CreateClient("api");
            trips = await client.GetFromJsonAsync<List<Trip>>("api/trips") ?? new();
            await base.OnInitializedAsync();
        }
        public void AddTrip() {
            NavigationManager.NavigateTo(PageRoutes.TripsCreate);
        }
    }
}
