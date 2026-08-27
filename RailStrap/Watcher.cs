using RailStrap.AppData;
using RailStrap.Integrations;
using RailStrap.Models;
using RailStrap.UI.Elements.Overlay;

namespace RailStrap
{
    public class Watcher : IDisposable
    {
        private const int MaxCrashRestartAttempts = 2;

        private readonly InterProcessLock _lock = new("Watcher");

        private readonly WatcherData? _watcherData;
        
        private readonly NotifyIconWrapper? _notifyIcon;

        public readonly ActivityWatcher? ActivityWatcher;

        public readonly DiscordRichPresence? RichPresence;

        public readonly PingOverlay? PingOverlay;

        public Watcher()
        {
            const string LOG_IDENT = "Watcher";

            if (!_lock.IsAcquired)
            {
                App.Logger.WriteLine(LOG_IDENT, "Watcher instance already exists");
                return;
            }

            string? watcherDataArg = App.LaunchSettings.WatcherFlag.Data;

            if (String.IsNullOrEmpty(watcherDataArg))
            {
#if DEBUG
                string path = new RobloxPlayerData().ExecutablePath;
                if (!File.Exists(path))
                    throw new ApplicationException("Roblox player is not been installed");

                using var gameClientProcess = Process.Start(path);

                _watcherData = new() { ProcessId = gameClientProcess.Id };
#else
                throw new Exception("Watcher data not specified");
#endif
            }
            else
            {
                _watcherData = JsonSerializer.Deserialize<WatcherData>(Encoding.UTF8.GetString(Convert.FromBase64String(watcherDataArg)));
            }

            if (_watcherData is null)
                throw new Exception("Watcher data is invalid");

            bool activityWatcherRequired =
                App.Settings.Prop.EnableActivityTracking ||
                App.Settings.Prop.EnablePingOverlay ||
                App.Settings.Prop.EnablePlaytimeStats ||
                App.Settings.Prop.AutoRestartOnCrash;

            if (activityWatcherRequired)
            {
                ActivityWatcher = new(_watcherData.LogFile);

                if (App.Settings.Prop.EnableActivityTracking && App.Settings.Prop.UseDisableAppPatch)
                {
                    ActivityWatcher.OnAppClose += delegate
                    {
                        App.Logger.WriteLine(LOG_IDENT, "Received desktop app exit, closing Roblox");
                        CloseProcess(_watcherData.ProcessId);
                    };
                }

                if (App.Settings.Prop.EnableActivityTracking && App.Settings.Prop.UseDiscordRichPresence)
                    RichPresence = new(ActivityWatcher);

                if (App.Settings.Prop.EnablePingOverlay)
                    PingOverlay = new(ActivityWatcher);

                if (App.Settings.Prop.EnablePlaytimeStats)
                    ActivityWatcher.OnGameLeave += (_, _) => RecordPlaytimeSession(ActivityWatcher.History.FirstOrDefault());
            }

            _notifyIcon = new(this);
        }

        private bool _intentionalClose = false;

        public void KillRobloxProcess() => CloseProcess(_watcherData!.ProcessId, true);

        private void RecordPlaytimeSession(ActivityData? activity)
        {
            const string LOG_IDENT = "Watcher::RecordPlaytimeSession";

            if (activity is null || activity.TimeJoined == default)
                return;

            DateTime timeLeft = activity.TimeLeft ?? DateTime.Now;
            TimeSpan duration = timeLeft - activity.TimeJoined;

            // ignore near-instant joins (connection errors, accidental clicks) but round
            // any real session up to at least a minute instead of truncating it away
            if (duration.TotalSeconds < 10)
                return;

            int minutes = Math.Max(1, (int)Math.Round(duration.TotalMinutes));

            try
            {
                App.PlaytimeStats.Prop.Sessions.Add(new PlaytimeSession
                {
                    PlaceName = activity.UniverseDetails?.Data.Name ?? $"Place {activity.PlaceId}",
                    UniverseId = activity.UniverseId,
                    TimeJoined = activity.TimeJoined,
                    DurationMinutes = minutes
                });

                App.PlaytimeStats.Save();
            }
            catch (Exception ex)
            {
                App.Logger.WriteException(LOG_IDENT, ex);
            }
        }

        public void CloseProcess(int pid, bool force = false)
        {
            const string LOG_IDENT = "Watcher::CloseProcess";

            if (pid == _watcherData?.ProcessId)
                _intentionalClose = true;

            try
            {
                using var process = Process.GetProcessById(pid);

                App.Logger.WriteLine(LOG_IDENT, $"Killing process '{process.ProcessName}' (pid={pid}, force={force})");

                if (process.HasExited)
                {
                    App.Logger.WriteLine(LOG_IDENT, $"PID {pid} has already exited");
                    return;
                }

                if (force)
                    process.Kill();
                else
                    process.CloseMainWindow();
            }
            catch (Exception ex)
            {
                App.Logger.WriteLine(LOG_IDENT, $"PID {pid} could not be closed");
                App.Logger.WriteException(LOG_IDENT, ex);
            }
        }

        public async Task Run()
        {
            const string LOG_IDENT = "Watcher::Run";

            if (!_lock.IsAcquired || _watcherData is null)
                return;

            ActivityWatcher?.Start();

            Process? gameProcess = null;

            try
            {
                gameProcess = Process.GetProcessById(_watcherData.ProcessId);
            }
            catch (ArgumentException)
            {
                // process has already exited by the time we got here
            }

            if (gameProcess is not null)
            {
                try
                {
                    await gameProcess.WaitForExitAsync();
                }
                catch
                {
                    // ignore - fall through to the exit check below
                }
            }

            // A hard crash does not write the normal disconnect log entry, so preserve the
            // active session before deciding whether Roblox should be restarted.
            if (App.Settings.Prop.EnablePlaytimeStats && ActivityWatcher?.InGame == true)
                RecordPlaytimeSession(ActivityWatcher.Data);

            if (App.Settings.Prop.AutoRestartOnCrash && !_intentionalClose)
            {
                int exitCode = -1;

                try
                {
                    if (gameProcess is not null)
                        exitCode = gameProcess.ExitCode;
                }
                catch (Exception ex)
                {
                    App.Logger.WriteException(LOG_IDENT, ex);
                }

                // a clean exit (either the user closed the window, or ActivityWatcher already
                // saw a graceful disconnect) reports 0; anything else we treat as a crash
                bool cleanExit = gameProcess is not null && exitCode == 0;

                if (!cleanExit && _watcherData.CrashRestartAttempt < MaxCrashRestartAttempts)
                {
                    App.Logger.WriteLine(LOG_IDENT, $"Roblox exited abnormally (code {exitCode}), attempting restart {_watcherData.CrashRestartAttempt + 1}/{MaxCrashRestartAttempts}");
                    await RestartAfterCrash();
                }
                else if (!cleanExit)
                {
                    App.Logger.WriteLine(LOG_IDENT, $"Roblox exited abnormally (code {exitCode}), but the automatic restart limit was reached");
                }
            }

            if (_watcherData.AutoclosePids is not null)
            {
                foreach (int pid in _watcherData.AutoclosePids)
                    CloseProcess(pid);
            }

            if (App.LaunchSettings.TestModeFlag.Active)
                Process.Start(Paths.Process, "-settings -testmode");
        }

        private async Task RestartAfterCrash()
        {
            const string LOG_IDENT = "Watcher::RestartAfterCrash";

            try
            {
                var lastActivity = ActivityWatcher?.Data.PlaceId != 0 ? ActivityWatcher?.Data : ActivityWatcher?.History.FirstOrDefault();
                string? launchArgument = null;

                if (lastActivity is not null && lastActivity.PlaceId != 0)
                    launchArgument = lastActivity.GetInviteDeeplink(false);
                else if (_watcherData?.LaunchArguments?.StartsWith("roblox", StringComparison.OrdinalIgnoreCase) == true)
                    launchArgument = _watcherData.LaunchArguments;

                // A short delay avoids immediately colliding with Roblox's process/mutex cleanup.
                await Task.Delay(TimeSpan.FromSeconds(3));

                var startInfo = new ProcessStartInfo(Paths.Process);

                if (launchArgument is not null)
                    startInfo.ArgumentList.Add(launchArgument);
                else
                    startInfo.ArgumentList.Add("-player");

                startInfo.ArgumentList.Add("-crashrestart");
                startInfo.ArgumentList.Add((_watcherData!.CrashRestartAttempt + 1).ToString(CultureInfo.InvariantCulture));

                Process.Start(startInfo);
            }
            catch (Exception ex)
            {
                App.Logger.WriteException(LOG_IDENT, ex);
            }
        }

        public void Dispose()
        {
            App.Logger.WriteLine("Watcher::Dispose", "Disposing Watcher");

            _notifyIcon?.Dispose();
            RichPresence?.Dispose();
            if (PingOverlay is not null)
                PingOverlay.Dispatcher.Invoke(PingOverlay.Close);

            ActivityWatcher?.Dispose();

            GC.SuppressFinalize(this);
        }
    }
}
