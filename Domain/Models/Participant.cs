namespace Domain.Models
{
    public class Participant
    {
        public string Name { get; set; }
        public int Id { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public string RaceType { get; set; }
    }
}
