using System.Collections.ObjectModel;

namespace RailStrap.Models.Persistable
{
    public class Settings
    {
        // bloxstrap configuration
        public BootstrapperStyle BootstrapperStyle { get; set; } = BootstrapperStyle.VideoDialog;
        public BootstrapperIcon BootstrapperIcon { get; set; } = BootstrapperIcon.IconRailStrap;
        public string BootstrapperTitle { get; set; } = App.ProjectName;
        public string BootstrapperIconCustomLocation { get; set; } = "";
        public Theme Theme { get; set; } = Theme.Default;
        public AccentStyle AccentStyle { get; set; } = AccentStyle.RailMono;
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public bool DeveloperMode { get; set; } = false;
        public bool CheckForUpdates { get; set; } = true;
        public bool ConfirmLaunches { get; set; } = false;
        public string Locale { get; set; } = "nil";
        public bool UseFastFlagManager { get; set; } = true;
        public bool WPFSoftwareRender { get; set; } = false;
        public bool EnableAnalytics { get; set; } = false;
        public bool BackgroundUpdatesEnabled { get; set; } = false;
        public bool DebugDisableVersionPackageCleanup { get; set; } = false;
        public string? SelectedCustomTheme { get; set; } = null;
        public WebEnvironment WebEnvironment { get; set; } = WebEnvironment.Production;

        // integration configuration
        public bool EnableActivityTracking { get; set; } = true;
        public bool UseDiscordRichPresence { get; set; } = true;
        public bool HideRPCButtons { get; set; } = true;
        public bool ShowAccountOnRichPresence { get; set; } = false;
        public bool ShowServerDetails { get; set; } = false;
        public ObservableCollection<CustomIntegration> CustomIntegrations { get; set; } = new();

        // mod preset configuration
        public bool UseDisableAppPatch { get; set; } = false;

        // performance / overlay
        public int MaxFPSValue { get; set; } = 0;
        public bool EnablePingOverlay { get; set; } = false;

        // Roblox's own Shift+F5 stats overlay. RailStrap can't measure ping from outside the client
        // (Roblox's servers drop ICMP), so this is the only way to actually see a real ping figure.
        public bool ShowRobloxPerformanceStats { get; set; } = false;

        // GlobalBasicSettings_13.xml frame rate cap - unlike DFIntTaskSchedulerTargetFps (MaxFPSValue
        // above), this isn't subject to Roblox's Sept 2025 FastFlag allowlist. 0 = don't manage it.
        public int GlobalFrameRateCap { get; set; } = 0;

        // preferred server regions - Roblox has no API to pick a region, so RailStrap matches the
        // joined server's location against this list and can reroll into a fresh server when it
        // doesn't match. Empty means no preference.
        public ObservableCollection<string> PreferredServerRegions { get; set; } = new();
        public bool AutoRerollUnpreferredServer { get; set; } = false;
        public int ServerRegionMaxRerolls { get; set; } = 3;

        // connection helper (GoodbyeDPI) - off unless explicitly enabled; see Integrations/DpiBypassManager.cs
        public bool EnableDpiBypass { get; set; } = false;
        public int DpiBypassMode { get; set; } = 6;

        // reliability
        public bool AutoRestartOnCrash { get; set; } = false;
        public int CrashRestartMaxAttempts { get; set; } = 2;
        public bool CrashRestartRequireInGame { get; set; } = false;

        // friend activity panel (opt-in; see UI/ViewModels/Settings/FriendActivityViewModel.cs)
        public bool EnableFriendActivityPanel { get; set; } = false;
        public string FriendActivityCookieEncrypted { get; set; } = "";

        // playtime stats
        public bool EnablePlaytimeStats { get; set; } = true;
    }
}
