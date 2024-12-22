namespace NextStop.Infrastructure.Persistence.Entities
{
    public class Checkin
    {
        public int Id { get; set; }
        public int RouteId { get; set; }
        public int StopId { get; set; }
        public DateTime Timestamp { get; set; }
    }
}