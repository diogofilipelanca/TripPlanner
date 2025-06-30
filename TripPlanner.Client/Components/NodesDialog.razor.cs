using Microsoft.AspNetCore.Components;
using TripPlanner.Client.Models;
using TripPlanner.Client.Pages.Trips;

namespace TripPlanner.Client.Components {
    public partial class NodesDialog {
        [Parameter]public List<Trip> TripsList { get; set; } = new();

        protected override Task OnInitializedAsync() {
            return base.OnInitializedAsync();
        }
    }
}
