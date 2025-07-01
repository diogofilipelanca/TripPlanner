using static TripPlanner.Client.Enums.Enums;

namespace TripPlanner.Client.Models {
    public class Optimize {
        public Optimize() {

        }

        public string Origin { get; set; } = string.Empty;
        public string Destination { get; set; } = string.Empty;
        public TypeTransport Transport { get; set; }
        public string OptimizeField { get; set; }

    }
}
