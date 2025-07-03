using Microsoft.AspNetCore.Components;
using TripPlanner.Client.Models;
using static TripPlanner.Client.Enums.Enums;

namespace TripPlanner.Client.Components {
    public partial class OptimizedTripDetails {
        [Parameter]
        public List<Trip> trips { get; set; } = new();
        [Parameter]
        public string optimization { get; set; } = string.Empty;
        public decimal value { get; set; } = 0;
        public string time { get; set; } = string.Empty;

        protected override void OnInitialized() {
            if(optimization == nameof(OptimizeField.Tempo)) {
                trips.ForEach(x => value += x.Time);
                var horas = (int)value;
                var minutos = (int)((value - horas)*100);
                time = $"{horas}h{minutos:D2}min";
            }else if(optimization == nameof(OptimizeField.Custo)) {
                trips.ForEach(x => value += x.Price);
            } else if(optimization == nameof(OptimizeField.Emissao)) {
                trips.ForEach(x => value += x.Emissions);
            }
            base.OnInitialized();
        }
    }
}
