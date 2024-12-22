namespace NextStop.Infrastructure.Persistence.Entities
{
    public class Holiday
    {
        public int Id { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Name { get; set; }
        public bool IsSchoolHoliday { get; set; }
    }
}