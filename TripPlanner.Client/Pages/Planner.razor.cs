using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Radzen;
using System.Net.Http.Json;
using TripPlanner.Client.Components;
using TripPlanner.Client.Models;
using static TripPlanner.Client.Enums.Enums;

namespace TripPlanner.Client.Pages {
    public partial class Planner {
        [Inject] IJSRuntime JSRuntime { get; set; } = default!;
        [Inject] IHttpClientFactory Http { get; set; } = default!;
        [Inject] DialogService dialogService { get; set; } = default!;
        [Inject] NotificationService NotificationService { get; set; } = default!;
        HttpClient? client { get; set; }
        private List<City> citiesList { get; set; } = new();
        private List<Trip> tripsList { get; set; } = new();
        private List<List<Trip>> Paths { get; set; } = new();
        private List<Trip> CurrentTrip { get; set; } = new();
        private Optimize Optimize { get; set; } = new();
        private List<Trip> OptimizedTrip { get; set; } = new();
        private string lastOrigin { get; set; } = string.Empty;
        private string typeTransport { get; set; } = "Todos";

        protected override async Task OnInitializedAsync() {
            client = Http.CreateClient("api");
            citiesList = await client.GetFromJsonAsync<List<City>>("api/cities") ?? new();
            tripsList = await client.GetFromJsonAsync<List<Trip>>("api/trips") ?? new();
            await base.OnInitializedAsync();
        }

        protected override async Task OnAfterRenderAsync(bool firstRender) {
            if (firstRender) {
                await RenderGraphs(typeTransport);
            }
            await base.OnAfterRenderAsync(firstRender);
        }

        public async Task RenderGraphs(string type) {
            citiesList = await client.GetFromJsonAsync<List<City>>("api/cities") ?? new();
            tripsList = await client.GetFromJsonAsync<List<Trip>>("api/trips") ?? new();
            if (type != "Todos") {
                tripsList = tripsList.Where(trip => trip.TypeTransport == type).ToList();
            }
            var nodes = citiesList?.Select(c => new {
                id = c.Name,
                highlighted = OptimizedTrip != null && OptimizedTrip.Any(ht => ht.Origin == c.Name || ht.Destination == c.Name) ? "true" : "false"
            });
            object links;
            if (Optimize.OptimizeField == "Tempo") {
                links = tripsList.Select(t => new {
                    source = t.Origin,
                    target = t.Destination,
                    label = t.Time.ToString(),
                    highlighted = OptimizedTrip != null && OptimizedTrip.Any(ht => ht.Id == t.Id) ? "true" : "false"
                });
            } else if (Optimize.OptimizeField == "Custo") {
                links = tripsList.Select(t => new {
                    source = t.Origin,
                    target = t.Destination,
                    label = t.Price.ToString(),
                    highlighted = OptimizedTrip != null && OptimizedTrip.Any(ht => ht.Id == t.Id) ? "true" : "false"
                });
            } else if (Optimize.OptimizeField == "Emissao") {
                links = tripsList.Select(t => new {
                    source = t.Origin,
                    target = t.Destination,
                    label = t.Emissions.ToString(),
                    highlighted = OptimizedTrip != null && OptimizedTrip.Any(ht => ht.Id == t.Id) ? "true" : "false"
                });
            } else {
                links = tripsList.Select(t => new {
                    source = t.Origin,
                    target = t.Destination,
                    highlighted = OptimizedTrip != null && OptimizedTrip.Any(ht => ht.Id == t.Id) ? "true" : "false"
                });
            }


            var graphData = new {
                nodes,
                links
            };

            await JSRuntime.InvokeVoidAsync("renderGraph", graphData, type);
            await JSRuntime.InvokeVoidAsync("setupGraphInterop", DotNetObjectReference.Create(this));
        }

        public async Task OnChange(int index) {
            if (index == 0) {
                typeTransport = "Todos";
            } else if (index == 1) {
                typeTransport = "Carro";
            } else if (index == 2) {
                typeTransport = "Autocarro";
            } else if (index == 3) {
                typeTransport = "Aviao";
            }
            await ResetFields();
        }

        public async Task ResetFields() {
            OptimizedTrip = new();
            Optimize = new();
            Paths = new();
            CurrentTrip = new();
            await RenderGraphs(typeTransport);
        }

        [JSInvokable]
        public void ShowCityModal(string cityName) {
            var newtripList = tripsList.Where(t => t.Origin == cityName || t.Destination == cityName).ToList();
            dialogService.OpenAsync<NodesDialog>($"Detalhes de {cityName}", new Dictionary<string, object> { { "TripsList", newtripList }, { "typeTransport", typeTransport } });
        }

        public async Task OnSubmit() {
            if (Optimize.Origin != Optimize.Destination) {
                PathOptimized(Optimize.Origin, Optimize.Destination, Optimize.OptimizeField);
                await RenderGraphs(typeTransport);
            } else {
                ShowNotification(new NotificationMessage { Severity = NotificationSeverity.Error, Summary = "Error Summary", Detail = "Origem não deve ser o mesmo que o Destino", Duration = 3000 });
            }
        }

        public void ShowNotification(NotificationMessage message) {
            NotificationService.Notify(message);
        }

        public List<Trip> Dijkstra(string start, string end, string optimizeField) {
            var distances = new Dictionary<string, decimal>();
            var previous = new Dictionary<string, Trip>();
            var unvisited = new HashSet<string>();

            List<Trip> _tripsList = new();
            if (typeTransport != "Todos") {
                _tripsList = tripsList.Where(trip => trip.TypeTransport == typeTransport).ToList();
            } else {
                _tripsList = tripsList;
            }

            foreach (var trip in _tripsList) {
                distances[trip.Origin] = decimal.MaxValue;
                unvisited.Add(trip.Origin);
                unvisited.Add(trip.Destination);
            }

            var keys = new List<string>(distances.Keys);

            foreach (var chave in keys) {
                if (chave == start)
                    distances[chave] = 0;
                else
                    distances[chave] = int.MaxValue - 1000;
            }

            while (unvisited.Count > 0) {
                var current = unvisited
                    .Where(n => distances.ContainsKey(n))
                    .OrderBy(n => distances[n])
                    .FirstOrDefault();

                if (current == null || current == end)
                    break;

                unvisited.Remove(current);

                var neighbors = tripsList.Where(t => t.Origin == current && (typeTransport == "Todos" || t.TypeTransport == typeTransport));

                foreach (var trip in neighbors) {
                    decimal cost = optimizeField switch {
                        nameof(OptimizeField.Tempo) => trip.Time,
                        nameof(OptimizeField.Custo) => trip.Price,
                        nameof(OptimizeField.Emissao) => trip.Emissions,
                        _ => throw new Exception("Campo de otimização inválido")
                    };

                    var alt = distances[current] + cost;
                    if (!distances.ContainsKey(trip.Destination) || alt < distances[trip.Destination]) {
                        distances[trip.Destination] = alt;
                        previous[trip.Destination] = trip;
                    }
                }
            }

            var path = new List<Trip>();
            var curr = end;
            while (previous.ContainsKey(curr)) {
                var trip = previous[curr];
                path.Insert(0, trip);
                curr = trip.Origin;
            }

            return path;
        }

        public async Task PathOptimized(string Origin, string Destination, string optimizeField) {

            var trip = Dijkstra(Origin, Destination, optimizeField);
            decimal menor = trip.Sum(t => optimizeField switch {
                nameof(OptimizeField.Tempo) => t.Time,
                nameof(OptimizeField.Custo) => t.Price,
                nameof(OptimizeField.Emissao) => t.Emissions,
                _ => 0
            });

            if (trip.Count() != 0) {
                OptimizedTrip = trip;
                await dialogService.OpenAsync<OptimizedTripDetails>($"{Optimize.Origin} até {Optimize.Destination}", new Dictionary<string, object> { { "trips", OptimizedTrip }, { "optimization", Optimize.OptimizeField} });
            } else {
                ShowNotification(new NotificationMessage { Severity = NotificationSeverity.Error, Summary = "Error Summary", Detail = "Não foi possível encontrar um caminho até esse Destino", Duration = 3000 });
            }
            var obj = new {
                trip,
                optimizeField,
                menor
            };
        }
    }
}
