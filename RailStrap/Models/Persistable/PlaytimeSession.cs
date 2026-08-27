namespace RailStrap.Models.Persistable
{
    public class PlaytimeSession
    {
        public string PlaceName { get; set; } = "";

        public long UniverseId { get; set; }

        public DateTime TimeJoined { get; set; }

        public int DurationMinutes { get; set; }
    }

    public class PlaytimeStats
    {
        public List<PlaytimeSession> Sessions { get; set; } = new();
    }
}
