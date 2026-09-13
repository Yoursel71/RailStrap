using System.Windows;
using System.Windows.Threading;

using RailStrap.Integrations;

namespace RailStrap.UI.Elements.Overlay
{
    /// <summary>
    /// Interaction logic for PingOverlay.xaml
    /// </summary>
    public partial class PingOverlay : Window
    {
        private readonly ActivityWatcher _activityWatcher;
        private readonly DispatcherTimer _timer;
        private bool _queryInProgress;

        public PingOverlay(ActivityWatcher activityWatcher)
        {
            InitializeComponent();

            _activityWatcher = activityWatcher;

            SizeChanged += (_, _) => Reposition();
            Reposition();

            _timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(3) };
            _timer.Tick += (_, _) => QueryPing();

            _activityWatcher.OnGameJoin += (_, _) => Dispatcher.Invoke(Show_);
            _activityWatcher.OnGameLeave += (_, _) => Dispatcher.Invoke(Hide_);
        }

        private void Reposition()
        {
            var workArea = SystemParameters.WorkArea;

            Left = workArea.Right - ActualWidth - 16;
            Top = workArea.Bottom - ActualHeight - 16;
        }

        private void Show_()
        {
            PingText.Text = $"{Strings.ContextMenu_ServerInformation_Ping}: --";
            LocationText.Visibility = Visibility.Collapsed;

            QueryPing();
            QueryLocation();

            _timer.Start();
            Show();
        }

        private void Hide_()
        {
            _timer.Stop();
            Hide();
        }

        private async void QueryLocation()
        {
            // reuses the same ipinfo.io opt-in as the server details notification, so this never
            // reaches out to a third party the user hasn't already agreed to
            if (!App.Settings.Prop.ShowServerDetails)
                return;

            ActivityData activity = _activityWatcher.Data;

            if (!activity.MachineAddressValid)
                return;

            string? location = await activity.QueryServerLocation();

            if (string.IsNullOrEmpty(location) || activity != _activityWatcher.Data)
                return;

            LocationText.Text = location;
            LocationText.Visibility = Visibility.Visible;
        }

        private async void QueryPing()
        {
            if (_queryInProgress)
                return;

            _queryInProgress = true;
            ActivityData activity = _activityWatcher.Data;

            try
            {
                long? ping = await activity.QueryPing();

                if (activity != _activityWatcher.Data)
                    return;

                if (ping is not null)
                {
                    PingText.Text = $"{Strings.ContextMenu_ServerInformation_Ping}: {ping} ms";
                }
                else if (ServerPing.IsUnreachable(activity.MachineAddress))
                {
                    // Roblox drops ICMP on practically every server, so rather than sitting on a
                    // dead '--' forever, point at the in-game overlay that does report a real value
                    PingText.Text = $"{Strings.ContextMenu_ServerInformation_Ping}: {Strings.Overlay_Ping_UseInGameStats}";
                    _timer.Stop();
                }
            }
            finally
            {
                _queryInProgress = false;
            }
        }
    }
}
