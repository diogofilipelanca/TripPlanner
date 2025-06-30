using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Radzen;
using Radzen.Blazor;
using System.Net.Http.Json;
using TripPlanner.Client.Components;
using TripPlanner.Client.Models;

namespace TripPlanner.Client.Pages {
    public partial class Planner {
        [Inject] IJSRuntime JSRuntime { get; set; } = default!;
        [Inject] IHttpClientFactory Http { get; set; } = default!;
        [Inject] DialogService dialogService { get; set; } = default!;
        private List<City> citiesList { get; set; } = new();
        private List<Trip> tripsList { get; set; } = new();
        protected override async Task OnAfterRenderAsync(bool firstRender) {
            if (firstRender) {
                var client = Http.CreateClient("api");
                citiesList = await client.GetFromJsonAsync<List<City>>("api/cities");
                tripsList = await client.GetFromJsonAsync<List<Trip>>("api/trips");

                var nodes = citiesList?.Select(c => new {
                    id = c.Name
                });
                var links = tripsList?.Select(t => new {
                    source = t.Origin,
                    target = t.Destination
                });

                var graphData = new {
                    nodes,
                    links
                };

                await JSRuntime.InvokeVoidAsync("renderGraph", graphData);
            }
            await JSRuntime.InvokeVoidAsync("setupGraphInterop", DotNetObjectReference.Create(this));
            await base.OnAfterRenderAsync(firstRender);
        }

        [JSInvokable]
        public void ShowCityModal(string cityName) {
            var newtripList = tripsList.Where(t => t.Origin == cityName || t.Destination == cityName).ToList();
            dialogService.OpenAsync<NodesDialog>($"Detalhes de {cityName}", new Dictionary<string, object> { { "TripsList", newtripList} });
        }

        /*Fazer um loop em que uma funcao chama a si propria, a funcao tem como tarefa de procurar todas as viagem de origem do nod escolhido, 
          se a viagem de destino nao for o requerido entao chamar a mesma funcao mas com todas as viagens de destino erradas ate encontrar a viagem de destino escolhido
          Problema: guardar o caminho - ?? List<List<Trip>> ?? */
    }
}
