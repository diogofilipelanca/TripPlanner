using Microsoft.AspNetCore.Components;
using TripPlanner.Client.Models;

namespace TripPlanner.Client.Components {
    public partial class OptimizedTripDetails {
        [Parameter]
        public List<Trip> trips { get; set; } = new();
    }
}
