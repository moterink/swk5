namespace NextStop.Dtos;

public class CheckinDto
{
    public int RouteId { get; set; }
    public int StopId { get; set; }
    public DateTime Timestamp { get; set; }
}