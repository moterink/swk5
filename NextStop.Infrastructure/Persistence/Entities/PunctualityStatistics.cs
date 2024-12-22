namespace NextStop.Infrastructure.Persistence.Entities;

public class PunctualityStatistics
{
    public string RouteNumber { get; set; }
    public double AverageDelaySeconds { get; set; }
    public double PercentPunctual { get; set; }
    public double PercentSlightlyDelayed { get; set; }
    public double PercentDelayed { get; set; }
    public double PercentSignificantlyDelayed { get; set; }
}