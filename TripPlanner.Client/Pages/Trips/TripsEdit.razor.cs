using Microsoft.AspNetCore.Components;
using System.Net.Http.Json;
using TripPlanner.Client.Enums;
using TripPlanner.Client.Models;

namespace TripPlanner.Client.Pages.Trips {
    public partial class TripsEdit {
        [Inject] IHttpClientFactory Http { get; set; } = default!;
        [Inject] NavigationManager NavigationManager { get; set; } = default!;
        [Parameter] public string Id { get; set; } = string.Empty;
        private HttpClient? httpClient { get; set; }
        public Trip trip { get; set; } = new();
        public List<City> cities { get; set; } = new();

        protected override async Task OnInitializedAsync() {
            httpClient = Http.CreateClient("api");
            cities = await httpClient.GetFromJsonAsync<List<City>>("api/cities") ?? new();
            await base.OnInitializedAsync();
        }

        public async Task OnSubmit() {
            var response = await httpClient.PostAsJsonAsync("api/trips", trip);
            NavigationManager.NavigateTo(PageRoutes.TripsList);
        }
    }
}
