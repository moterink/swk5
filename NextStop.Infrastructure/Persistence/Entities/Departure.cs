namespace NextStop.Infrastructure.Persistence.Entities
{
    public class Departure
    {
        public int RouteId { get; set; }
        public string RouteNumber { get; set; }
        public TimeOnly ScheduledDepartureTime { get; set; }
        public TimeOnly RealDepartureTime { get; set; } 
        public string EndStop { get; set; }
        public int Delay { get; set; }
    }
}