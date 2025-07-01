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
                    highlighted = OptimizedTrip != null && OptimizedTrip.Any(ht => ht.Origin == t.Origin && ht.Destination == t.Destination) ? "true" : "false"
                });
            } else if (Optimize.OptimizeField == "Custo") {
                links = tripsList.Select(t => new {
                    source = t.Origin,
                    target = t.Destination,
                    label = t.Price.ToString(),
                    highlighted = OptimizedTrip != null && OptimizedTrip.Any(ht => ht.Origin == t.Origin && ht.Destination == t.Destination) ? "true" : "false"
                });
            } else if (Optimize.OptimizeField == "Emissao") {
                links = tripsList.Select(t => new {
                    source = t.Origin,
                    target = t.Destination,
                    label = t.Emissions.ToString(),
                    highlighted = OptimizedTrip != null && OptimizedTrip.Any(ht => ht.Origin == t.Origin && ht.Destination == t.Destination) ? "true" : "false"
                });
            } else {
                links = tripsList.Select(t => new {
                    source = t.Origin,
                    target = t.Destination,
                    highlighted = OptimizedTrip != null && OptimizedTrip.Any(ht => ht.Origin == t.Origin && ht.Destination == t.Destination) ? "true" : "false"
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
            PathOptimized(Optimize.Origin, Optimize.Destination, Optimize.OptimizeField);
            await RenderGraphs(typeTransport);
        }

        public void FindAllPaths(string origin, string destination) {
            List<Trip> _tripsList = new();
            if (typeTransport != "Todos") {
                _tripsList = tripsList.Where(trip => trip.TypeTransport == typeTransport).ToList();
            } else {
                _tripsList = tripsList;
            }
            var allOriginPaths = _tripsList.Where(t => origin.Contains(t.Origin)).ToList();
            if (allOriginPaths.Any()) {
                allOriginPaths.ForEach(path => {
                    if (path.Destination != destination) {
                        if (path.Destination != lastOrigin || CurrentTrip.Contains(path)) {
                            CurrentTrip.Add(path);
                            lastOrigin = path.Origin;
                            FindAllPaths(path.Destination, destination);
                        }
                    } else {
                        CurrentTrip.Add(path);
                        if (!Paths.Contains(CurrentTrip)) {
                            Paths.Add(CurrentTrip);
                        }
                        CurrentTrip = new();
                    }
                });
            }
        }

        public void PathOptimized(string Origin, string Destination, string optimizeField) {
            var trip = new List<Trip>();
            decimal menor = int.MaxValue;
            if (optimizeField == nameof(OptimizeField.Tempo)) {
                FindAllPaths(Origin, Destination);
                Paths.ForEach(path => {
                    decimal _time = 0;
                    var timelist = path.Select(x => x.Time).ToList();
                    timelist.ForEach(time => {
                        _time += time;
                    });
                    if (menor > _time) {
                        menor = _time;
                        trip = path;
                    }
                });
            }
            if (optimizeField == nameof(OptimizeField.Custo)) {
                FindAllPaths(Origin, Destination);
                Paths.ForEach(path => {
                    decimal _price = 0;
                    var timelist = path.Select(x => x.Price).ToList();
                    timelist.ForEach(price => {
                        _price += price;
                    });
                    if (menor > _price) {
                        menor = _price;
                        trip = path;
                    }
                });
            }
            if (optimizeField == nameof(OptimizeField.Emissao)) {
                FindAllPaths(Origin, Destination);
                Paths.ForEach(path => {
                    decimal _emission = 0;
                    var timelist = path.Select(x => x.Emissions).ToList();
                    timelist.ForEach(emission => {
                        _emission += emission;
                    });
                    if (menor > _emission) {
                        menor = _emission;
                        trip = path;
                    }
                });
            }

            object obj = new {
                trip,
                optimizeField,
                menor
            };
            OptimizedTrip = trip;
        }
    }
}
