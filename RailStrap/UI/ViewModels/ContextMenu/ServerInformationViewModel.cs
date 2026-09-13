using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using RailStrap.Integrations;
using CommunityToolkit.Mvvm.Input;

namespace RailStrap.UI.ViewModels.ContextMenu
{
    internal class ServerInformationViewModel : NotifyPropertyChangedViewModel
    {
        private readonly ActivityWatcher _activityWatcher;
        private readonly DispatcherTimer _pingTimer;

        public string InstanceId => _activityWatcher.Data.JobId;

        public string ServerType => _activityWatcher.Data.ServerType.ToTranslatedString();

        public string ServerLocation { get; private set; } = Strings.Common_Loading;

        public string Ping { get; private set; } = Strings.Common_Loading;

        public Visibility ServerLocationVisibility => App.Settings.Prop.ShowServerDetails ? Visibility.Visible : Visibility.Collapsed;

        public Visibility RerollVisibility => _activityWatcher.Data.CanReroll ? Visibility.Visible : Visibility.Collapsed;

        public ICommand CopyInstanceIdCommand => new RelayCommand(CopyInstanceId);

        public ICommand RerollServerCommand => _activityWatcher.Data.RerollServerCommand;

        public ServerInformationViewModel(Watcher watcher)
        {
            _activityWatcher = watcher.ActivityWatcher!;

            if (ServerLocationVisibility == Visibility.Visible)
                QueryServerLocation();

            // created before the first query so the query's continuation can always stop it
            _pingTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(5) };
            _pingTimer.Tick += (_, _) => QueryPing();
            _pingTimer.Start();

            QueryPing();
        }

        public async void QueryServerLocation()
        {
            string? location = await _activityWatcher.Data.QueryServerLocation();

            if (String.IsNullOrEmpty(location))
                ServerLocation = Strings.Common_NotAvailable;
            else
                ServerLocation = location;

            OnPropertyChanged(nameof(ServerLocation));
        }

        public async void QueryPing()
        {
            var activity = _activityWatcher.Data;
            long? ping = await activity.QueryPing();

            if (ping is not null)
            {
                Ping = $"{ping} ms";
            }
            else if (ServerPing.IsUnreachable(activity.MachineAddress))
            {
                // Roblox's servers drop ICMP, so there is nothing left to poll for - say where a
                // real figure can be found instead of sitting on "not available" forever
                Ping = Strings.Overlay_Ping_UseInGameStats;
                _pingTimer.Stop();
            }
            else
            {
                Ping = Strings.Common_NotAvailable;
            }

            OnPropertyChanged(nameof(Ping));
        }

        public void StopPingTimer() => _pingTimer.Stop();

        private void CopyInstanceId() => Clipboard.SetDataObject(InstanceId);
    }
}
