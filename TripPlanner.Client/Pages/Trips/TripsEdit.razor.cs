using Microsoft.AspNetCore.Components;
using System.Net.Http.Json;
using TripPlanner.Client.Enums;
using TripPlanner.Client.Models;
using static TripPlanner.Client.Enums.Enums;

namespace TripPlanner.Client.Pages.Trips {
    public partial class TripsEdit {
        [Inject] IHttpClientFactory Http { get; set; } = default!;
        [Inject] NavigationManager NavigationManager { get; set; } = default!;
        [Parameter] public string Id { get; set; } = string.Empty;
        private HttpClient? httpClient { get; set; }
        public Trip trip { get; set; } = new();
        public List<City> cities { get; set; } = new();
        public PageModes PageMode { get; set; } = PageModes.Create;

        protected override async Task OnInitializedAsync() {
            httpClient = Http.CreateClient("api");
            cities = await httpClient.GetFromJsonAsync<List<City>>("api/cities") ?? new();
            if (!string.IsNullOrWhiteSpace(Id)) {
                trip = await httpClient.GetFromJsonAsync<Trip>($"api/trips/{Id}") ?? new();
                PageMode = PageModes.Update;
            }
            await base.OnInitializedAsync();
        }

        public async Task OnSubmit() {
            if (PageMode == PageModes.Create) {
                var response = await httpClient.PostAsJsonAsync("api/trips", trip);
            } else if (PageMode == PageModes.Update) {
                httpClient?.PatchAsJsonAsync($"api/trips/{trip.Id}", trip);
            }
            NavigationManager.NavigateTo(PageRoutes.TripsList);
        }
    }
}
