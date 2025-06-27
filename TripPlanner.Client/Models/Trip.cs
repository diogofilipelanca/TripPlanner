namespace TripPlanner.Client.Models {
    public class Trip {
        public Trip() { }

        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Origin { get; set; } = string.Empty;
        public string Destination { get; set; } = string.Empty;
        public decimal Price { get; set; } = 0;
        public decimal Time { get; set; } = 0;
        public decimal Emissions { get; set; } = 0;
    }
}
