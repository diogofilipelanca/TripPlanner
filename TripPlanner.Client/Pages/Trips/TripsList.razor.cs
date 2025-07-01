using Microsoft.AspNetCore.Components;
using System.Net.Http.Json;
using TripPlanner.Client.Enums;
using TripPlanner.Client.Models;

namespace TripPlanner.Client.Pages.Trips {
    public partial class TripsList {
        [Inject] IHttpClientFactory Http { get; set; } = default!;
        [Inject] NavigationManager NavigationManager { get; set; } = default!;
        public HttpClient? client { get; set; }
        public List<Trip> trips { get; set; } = new();
        protected override async Task OnInitializedAsync() {
            client = Http.CreateClient("api");
            trips = await client.GetFromJsonAsync<List<Trip>>("api/trips") ?? new();
            await base.OnInitializedAsync();
        }
        public void AddTrip() {
            NavigationManager.NavigateTo(PageRoutes.TripsCreate);
        }
        public void EditTrip(Trip trip) {
            NavigationManager.NavigateTo($"{PageRoutes.TripsEdit}/{trip.Id}");
        }
        public void DeleteTrip(Trip trip) {
            client?.DeleteFromJsonAsync<Trip>($"api/trips/{trip.Id}");
            NavigationManager.NavigateTo(PageRoutes.TripsList, true);
        }
    }
}
