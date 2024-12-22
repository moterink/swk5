namespace NextStop.Infrastructure.Persistence.Entities
{
    public class Route
    {
        public int Id { get; set; }
        public string Number { get; set; }
        public DateOnly ValidFrom { get; set; }
        public DateOnly ValidTo { get; set; }
        public string DaysOfOperation { get; set; }
        public List<RouteStop> Stops { get; set; } = new();
    }
}