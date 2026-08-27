using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;

using CommunityToolkit.Mvvm.Input;
using Microsoft.Win32;
using RailStrap.Models.Entities;

namespace RailStrap.UI.ViewModels.Settings
{
    public class StatsViewModel : NotifyPropertyChangedViewModel
    {
        private const double MAX_BAR_WIDTH = 400;

        public ObservableCollection<PlaytimeSummaryEntry> Entries { get; set; } = new();

        public Visibility EmptyVisibility => Entries.Any() ? Visibility.Collapsed : Visibility.Visible;

        public bool HasEntries => Entries.Any();

        public int TotalMinutes => App.PlaytimeStats.Prop.Sessions.Sum(x => x.DurationMinutes);

        public string TotalPlaytimeText => FormatDuration(TotalMinutes);

        public int SessionCount => App.PlaytimeStats.Prop.Sessions.Count;

        public ICommand ExportCommand => new RelayCommand(Export);

        public ICommand ClearCommand => new RelayCommand(Clear);

        public bool EnablePlaytimeStats
        {
            get => App.Settings.Prop.EnablePlaytimeStats;
            set => App.Settings.Prop.EnablePlaytimeStats = value;
        }

        public StatsViewModel()
        {
            Populate();
        }

        private void Populate()
        {
            Entries.Clear();

            var summary = App.PlaytimeStats.Prop.Sessions
                .GroupBy(x => x.UniverseId != 0 ? $"universe:{x.UniverseId}" : $"place:{x.PlaceName}")
                .Select(g => new PlaytimeSummaryEntry
                {
                    PlaceName = g.LastOrDefault(x => !string.IsNullOrWhiteSpace(x.PlaceName))?.PlaceName ?? Strings.Common_NotAvailable,
                    TotalMinutes = g.Sum(x => x.DurationMinutes),
                    SessionCount = g.Count()
                })
                .OrderByDescending(x => x.TotalMinutes)
                .ToList();

            int max = summary.Count > 0 ? summary[0].TotalMinutes : 1;

            foreach (var entry in summary)
            {
                entry.BarWidth = max > 0 ? MAX_BAR_WIDTH * entry.TotalMinutes / max : 0;
                Entries.Add(entry);
            }

            OnPropertyChanged(nameof(Entries));
            OnPropertyChanged(nameof(EmptyVisibility));
            OnPropertyChanged(nameof(HasEntries));
            OnPropertyChanged(nameof(TotalMinutes));
            OnPropertyChanged(nameof(TotalPlaytimeText));
            OnPropertyChanged(nameof(SessionCount));
        }

        private void Export()
        {
            if (!HasEntries)
                return;

            var dialog = new SaveFileDialog
            {
                FileName = $"RailStrap-Playtime-{DateTime.Now:yyyy-MM-dd}.csv",
                Filter = "CSV (*.csv)|*.csv"
            };

            if (dialog.ShowDialog() != true)
                return;

            var csv = new StringBuilder("Game,UniverseId,Joined,Minutes\r\n");

            foreach (var session in App.PlaytimeStats.Prop.Sessions.OrderBy(x => x.TimeJoined))
            {
                csv.Append(EscapeCsv(session.PlaceName)).Append(',')
                    .Append(session.UniverseId).Append(',')
                    .Append(session.TimeJoined.ToString("O", CultureInfo.InvariantCulture)).Append(',')
                    .Append(session.DurationMinutes).Append("\r\n");
            }

            File.WriteAllText(dialog.FileName, csv.ToString(), new UTF8Encoding(true));
            Process.Start("explorer.exe", $"/select,\"{dialog.FileName}\"");
        }

        private void Clear()
        {
            if (!HasEntries)
                return;

            MessageBoxResult result = Frontend.ShowMessageBox(
                Strings.Menu_Stats_ClearConfirm,
                MessageBoxImage.Warning,
                MessageBoxButton.YesNo);

            if (result != MessageBoxResult.Yes)
                return;

            try
            {
                if (File.Exists(App.PlaytimeStats.FileLocation))
                    File.Copy(App.PlaytimeStats.FileLocation, App.PlaytimeStats.FileLocation + ".bak", true);
            }
            catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
            {
                App.Logger.WriteException("StatsViewModel::ClearBackup", ex);
            }

            App.PlaytimeStats.Prop.Sessions.Clear();
            App.PlaytimeStats.Save();
            Populate();
        }

        private static string EscapeCsv(string value)
        {
            if (!string.IsNullOrEmpty(value) && "=+-@".Contains(value[0]))
                value = "'" + value;

            return $"\"{value.Replace("\"", "\"\"")}\"";
        }

        private static string FormatDuration(int totalMinutes) => totalMinutes >= 60
            ? $"{totalMinutes / 60}h {totalMinutes % 60}m"
            : $"{totalMinutes}m";
    }
}
