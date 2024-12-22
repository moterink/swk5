namespace NextStop.Infrastructure.Persistence.Entities
{
    public class RouteStop
    {
        public int RouteId { get; set; }
        public int StopId { get; set; }
        public int SequenceNumber { get; set; }
        public TimeOnly ScheduledDepartureTime { get; set; }
    }
}