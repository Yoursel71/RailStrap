namespace RailStrap.Models.Entities
{
    public class PlaytimeSummaryEntry
    {
        public string PlaceName { get; set; } = "";

        public int TotalMinutes { get; set; }

        public string DurationText => TotalMinutes >= 60
            ? $"{TotalMinutes / 60}h {TotalMinutes % 60}m"
            : $"{TotalMinutes}m";

        public double BarWidth { get; set; }
    }
}
