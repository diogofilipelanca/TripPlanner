using Microsoft.AspNetCore.Components;
using System.Net.Http.Json;
using TripPlanner.Client.Enums;
using TripPlanner.Client.Models;
using static TripPlanner.Client.Enums.Enums;

namespace TripPlanner.Client.Pages.Cities {
    public partial class CitiesEdit {
        [Inject] IHttpClientFactory Http { get; set; } = default!;
        [Inject] NavigationManager NavigationManager { get; set; } = default!;
        [Parameter] public string Name { get; set; } = string.Empty;
        private PageModes PageMode { get; set; } = PageModes.Create;
        private HttpClient HttpClient { get; set; }
        public City city { get; set; } = new();
        public string OriginalCityName { get; set; } = string.Empty;

        protected override async Task OnInitializedAsync() {
            HttpClient = Http.CreateClient("api");
            if (!string.IsNullOrWhiteSpace(Name)) {
                city = await HttpClient.GetFromJsonAsync<City>($"api/cities/{Name}") ?? new();
                OriginalCityName = city.Name;
                PageMode = PageModes.Update;
            }
            await base.OnInitializedAsync();
        }

        public async Task OnSubmit() {
            if (PageMode == PageModes.Create) {
                var response = await HttpClient.PostAsJsonAsync("api/cities", city);
            } else if (PageMode == PageModes.Update) {
                var response = await HttpClient.PatchAsJsonAsync($"api/cities/{OriginalCityName}", city);
            }
            NavigationManager.NavigateTo(PageRoutes.CitiesList);
        }
    }
}