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

            var workArea = SystemParameters.WorkArea;
            Left = workArea.Right - Width - 16;
            Top = workArea.Bottom - Height - 16;

            _timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(3) };
            _timer.Tick += (_, _) => QueryPing();

            _activityWatcher.OnGameJoin += (_, _) => Dispatcher.Invoke(Show_);
            _activityWatcher.OnGameLeave += (_, _) => Dispatcher.Invoke(Hide_);
        }

        private void Show_()
        {
            QueryPing();
            _timer.Start();
            Show();
        }

        private void Hide_()
        {
            _timer.Stop();
            Hide();
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

                if (activity == _activityWatcher.Data)
                    PingText.Text = ping is null ? $"{Strings.ContextMenu_ServerInformation_Ping}: --" : $"{Strings.ContextMenu_ServerInformation_Ping}: {ping} ms";
            }
            finally
            {
                _queryInProgress = false;
            }
        }
    }
}
