using Microsoft.AspNetCore.Components;
using TripPlanner.Client.Models;

namespace TripPlanner.Client.Components {
    public partial class NodesDialog {
        [Parameter] public List<Trip> TripsList { get; set; } = new();
        [Parameter] public string typeTransport { get; set; } = string.Empty;

        protected override Task OnInitializedAsync() {
            return base.OnInitializedAsync();
        }
    }
}
