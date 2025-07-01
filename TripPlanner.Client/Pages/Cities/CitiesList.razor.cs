using Microsoft.AspNetCore.Components;
using Radzen.Blazor;
using System.Net.Http.Json;
using TripPlanner.Client.Enums;
using TripPlanner.Client.Models;

namespace TripPlanner.Client.Pages.Cities {
    public partial class CitiesList {
        [Inject] IHttpClientFactory Http { get; set; } = default!;
        [Inject] NavigationManager Navigation { get; set; } = default!;
        public List<City> cities { get; set; } = new();
        RadzenDataGrid<City> Grid { get; set; } = new();
        HttpClient? HttpClient { get; set; }

        protected override async Task OnInitializedAsync() {
            HttpClient = Http.CreateClient("api");
            cities = await HttpClient.GetFromJsonAsync<List<City>>("api/cities") ?? new();
            await base.OnInitializedAsync();
        }

        public void AddCity() {
            Navigation.NavigateTo(PageRoutes.CitiesCreate);
        }

        public void EditCity(City city) {
            Navigation.NavigateTo($"{PageRoutes.CitiesEdit}/{city.Name}");
        }

        public async void DeleteCity(City city) {
            await HttpClient!.DeleteAsync($"api/cities/{city.Name}");
            Navigation.NavigateTo(PageRoutes.CitiesList, true);
        }
    }
}
