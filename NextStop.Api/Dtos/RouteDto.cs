namespace NextStop.Dtos;

public class RouteDto
{
    public string Number { get; set; } = string.Empty;
    public DateOnly ValidFrom { get; set; }
    public DateOnly ValidTo { get; set; }
    public string DaysOfOperation { get; set; } = string.Empty;
    public List<RouteStopDto> Stops { get; set; } = new();
}

public class RouteStopDto
{
    public int StopId { get; set; }
    public TimeOnly ScheduledDepartureTime { get; set; }
}