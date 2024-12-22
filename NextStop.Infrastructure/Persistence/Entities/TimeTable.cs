namespace NextStop.Infrastructure.Persistence.Entities
{
    public class TimetableResult
    {
        public int RouteId { get; set; }
        public int StartStopId { get; set; }
        public int EndStopId { get; set; }
        public DateTime ScheduledDepartureTime { get; set; }
        public DateTime ScheduledArrivalTime { get; set; }
        public DateTime RealDepartureTime { get; set; }
        public DateTime RealArrivalTime { get; set; }
    }
}