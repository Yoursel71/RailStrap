using System.Collections.ObjectModel;
using System.Windows;

using RailStrap.Models.Entities;

namespace RailStrap.UI.ViewModels.Settings
{
    public class StatsViewModel : NotifyPropertyChangedViewModel
    {
        private const double MAX_BAR_WIDTH = 400;

        public ObservableCollection<PlaytimeSummaryEntry> Entries { get; set; } = new();

        public Visibility EmptyVisibility => Entries.Any() ? Visibility.Collapsed : Visibility.Visible;

        public bool EnablePlaytimeStats
        {
            get => App.Settings.Prop.EnablePlaytimeStats;
            set => App.Settings.Prop.EnablePlaytimeStats = value;
        }

        public StatsViewModel()
        {
            var summary = App.PlaytimeStats.Prop.Sessions
                .GroupBy(x => x.PlaceName)
                .Select(g => new PlaytimeSummaryEntry { PlaceName = g.Key, TotalMinutes = g.Sum(x => x.DurationMinutes) })
                .OrderByDescending(x => x.TotalMinutes)
                .ToList();

            int max = summary.Count > 0 ? summary[0].TotalMinutes : 1;

            foreach (var entry in summary)
            {
                entry.BarWidth = max > 0 ? MAX_BAR_WIDTH * entry.TotalMinutes / max : 0;
                Entries.Add(entry);
            }
        }
    }
}
