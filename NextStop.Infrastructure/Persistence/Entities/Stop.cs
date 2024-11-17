namespace NextStop.Infrastructure.Persistence.Entities
{
    public class Stop
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string ShortName { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }
}