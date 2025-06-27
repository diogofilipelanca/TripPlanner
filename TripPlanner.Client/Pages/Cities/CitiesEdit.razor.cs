using Microsoft.AspNetCore.Components;
using System.Net.Http.Json;
using TripPlanner.Client.Enums;
using TripPlanner.Client.Models;

namespace TripPlanner.Client.Pages.Cities {
    public partial class CitiesEdit {
        [Inject] IHttpClientFactory Http { get; set; } = default!;
        [Inject] NavigationManager NavigationManager { get; set; } = default!;
        [Parameter] public string Id { get; set; } = string.Empty;
        public City city { get; set; } = new();

        public async Task OnSubmit() {
            var client = Http.CreateClient("api");
            var response = await client.PostAsJsonAsync("api/cities", city);
            NavigationManager.NavigateTo(PageRoutes.CitiesList);
        }
    }
}