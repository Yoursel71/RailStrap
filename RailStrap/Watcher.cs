using RailStrap.AppData;
using RailStrap.Integrations;
using RailStrap.Models;
using RailStrap.UI.Elements.Overlay;

namespace RailStrap
{
    public class Watcher : IDisposable
    {
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

            if (App.Settings.Prop.EnableActivityTracking)
            {
                ActivityWatcher = new(_watcherData.LogFile);

                if (App.Settings.Prop.UseDisableAppPatch)
                {
                    ActivityWatcher.OnAppClose += delegate
                    {
                        App.Logger.WriteLine(LOG_IDENT, "Received desktop app exit, closing Roblox");
                        CloseProcess(_watcherData.ProcessId);
                    };
                }

                if (App.Settings.Prop.UseDiscordRichPresence)
                    RichPresence = new(ActivityWatcher);

                if (App.Settings.Prop.EnablePingOverlay)
                    PingOverlay = new(ActivityWatcher);

                if (App.Settings.Prop.EnablePlaytimeStats)
                    ActivityWatcher.OnGameLeave += (_, _) => RecordPlaytimeSession();
            }

            _notifyIcon = new(this);
        }

        private bool _intentionalClose = false;

        public void KillRobloxProcess() => CloseProcess(_watcherData!.ProcessId, true);

        private void RecordPlaytimeSession()
        {
            const string LOG_IDENT = "Watcher::RecordPlaytimeSession";

            var activity = ActivityWatcher?.History.FirstOrDefault();

            if (activity is null || activity.TimeLeft is null)
                return;

            int minutes = (int)(activity.TimeLeft.Value - activity.TimeJoined).TotalMinutes;

            if (minutes < 1)
                return;

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

            if (App.Settings.Prop.AutoRestartOnCrash && !_intentionalClose && gameProcess is not null)
            {
                int exitCode = 0;

                try
                {
                    exitCode = gameProcess.ExitCode;
                }
                catch (Exception ex)
                {
                    App.Logger.WriteException(LOG_IDENT, ex);
                }

                // a clean exit (either the user closed the window, or ActivityWatcher already
                // saw a graceful disconnect) reports 0; anything else we treat as a crash
                bool cleanExit = exitCode == 0 || (ActivityWatcher?.History.FirstOrDefault()?.TimeLeft is not null);

                if (!cleanExit)
                {
                    App.Logger.WriteLine(LOG_IDENT, $"Roblox exited abnormally (code {exitCode}), attempting restart");
                    RestartAfterCrash();
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

        private void RestartAfterCrash()
        {
            const string LOG_IDENT = "Watcher::RestartAfterCrash";

            try
            {
                string playerPath = new RobloxPlayerData().ExecutablePath;
                var lastActivity = ActivityWatcher?.Data.PlaceId != 0 ? ActivityWatcher?.Data : ActivityWatcher?.History.FirstOrDefault();

                if (lastActivity is not null && lastActivity.PlaceId != 0)
                    Process.Start(playerPath, lastActivity.GetInviteDeeplink(false));
                else
                    Process.Start(playerPath);
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
            PingOverlay?.Close();

            GC.SuppressFinalize(this);
        }
    }
}
