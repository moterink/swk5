namespace NextStop.Infrastructure.Persistence.Entities
{
    public class Route
    {
        public int Id { get; set; }
        public string Number { get; set; }
        public DateTime ValidFrom { get; set; }
        public DateTime ValidTo { get; set; }
        public string DaysOfOperation { get; set; }
    }
}