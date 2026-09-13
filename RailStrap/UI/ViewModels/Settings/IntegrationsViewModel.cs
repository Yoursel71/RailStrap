using System.Collections.ObjectModel;
using System.Windows.Input;

using Microsoft.Win32;

using CommunityToolkit.Mvvm.Input;

using RailStrap.Integrations;
using RailStrap.UI.Elements.Dialogs;
using RailStrap.Utility;

namespace RailStrap.UI.ViewModels.Settings
{
    /// <summary>
    /// Checkbox-friendly wrapper around a <see cref="ServerRegion"/>, backed directly by the
    /// persisted preference list so ticking a box is the whole interaction.
    /// </summary>
    public class ServerRegionSelection : NotifyPropertyChangedViewModel
    {
        private readonly ServerRegion _region;

        public ServerRegionSelection(ServerRegion region) => _region = region;

        public string DisplayName => _region.DisplayName;

        public bool IsSelected
        {
            get => App.Settings.Prop.PreferredServerRegions.Contains(_region.Key, StringComparer.OrdinalIgnoreCase);
            set
            {
                var preferred = App.Settings.Prop.PreferredServerRegions;

                if (value && !IsSelected)
                    preferred.Add(_region.Key);
                else if (!value)
                    foreach (string key in preferred.Where(x => x.Equals(_region.Key, StringComparison.OrdinalIgnoreCase)).ToList())
                        preferred.Remove(key);
            }
        }

        public void Refresh() => OnPropertyChanged(nameof(IsSelected));
    }

    public class IntegrationsViewModel : NotifyPropertyChangedViewModel
    {
        public ICommand AddIntegrationCommand => new RelayCommand(AddIntegration);

        public ICommand DeleteIntegrationCommand => new RelayCommand(DeleteIntegration);

        public ICommand BrowseIntegrationLocationCommand => new RelayCommand(BrowseIntegrationLocation);

        public ICommand ViewFriendActivityCommand => new RelayCommand(ViewFriendActivity);

        public ICommand SaveFriendActivityCookieCommand => new RelayCommand(SaveFriendActivityCookie);

        public ICommand ClearFriendActivityCookieCommand => new RelayCommand(ClearFriendActivityCookie);

        private void ViewFriendActivity() => new FriendActivityWindow().ShowDialog();

        private void AddIntegration()
        {
            CustomIntegrations.Add(new CustomIntegration()
            {
                Name = Strings.Menu_Integrations_Custom_NewIntegration
            });

            SelectedCustomIntegrationIndex = CustomIntegrations.Count - 1;

            OnPropertyChanged(nameof(SelectedCustomIntegrationIndex));
            OnPropertyChanged(nameof(IsCustomIntegrationSelected));
        }

        private void DeleteIntegration()
        {
            if (SelectedCustomIntegration is null)
                return;

            CustomIntegrations.Remove(SelectedCustomIntegration);

            if (CustomIntegrations.Count > 0)
            {
                SelectedCustomIntegrationIndex = CustomIntegrations.Count - 1;
                OnPropertyChanged(nameof(SelectedCustomIntegrationIndex));
            }

            OnPropertyChanged(nameof(IsCustomIntegrationSelected));
        }

        private void BrowseIntegrationLocation()
        {
            if (SelectedCustomIntegration is null)
                return;

            var dialog = new OpenFileDialog
            {
                Filter = $"{Strings.Menu_AllFiles}|*.*"
            };

            if (dialog.ShowDialog() != true)
                return;

            SelectedCustomIntegration.Name = dialog.SafeFileName;
            SelectedCustomIntegration.Location = dialog.FileName;
            OnPropertyChanged(nameof(SelectedCustomIntegration));
        }

        public bool ActivityTrackingEnabled
        {
            get => App.Settings.Prop.EnableActivityTracking;
            set
            {
                App.Settings.Prop.EnableActivityTracking = value;

                if (!value)
                {
                    ShowServerDetailsEnabled = value;
                    DisableAppPatchEnabled = value;
                    DiscordActivityEnabled = value;
                    DiscordActivityJoinEnabled = value;

                    OnPropertyChanged(nameof(ShowServerDetailsEnabled));
                    OnPropertyChanged(nameof(DisableAppPatchEnabled));
                    OnPropertyChanged(nameof(DiscordActivityEnabled));
                    OnPropertyChanged(nameof(DiscordActivityJoinEnabled));
                }
            }
        }

        public bool ShowServerDetailsEnabled
        {
            get => App.Settings.Prop.ShowServerDetails;
            set => App.Settings.Prop.ShowServerDetails = value;
        }

        public bool DiscordActivityEnabled
        {
            get => App.Settings.Prop.UseDiscordRichPresence;
            set
            {
                App.Settings.Prop.UseDiscordRichPresence = value;

                if (!value)
                {
                    DiscordActivityJoinEnabled = value;
                    DiscordAccountOnProfile = value;
                    OnPropertyChanged(nameof(DiscordActivityJoinEnabled));
                    OnPropertyChanged(nameof(DiscordAccountOnProfile));
                }
            }
        }

        public bool DiscordActivityJoinEnabled
        {
            get => !App.Settings.Prop.HideRPCButtons;
            set => App.Settings.Prop.HideRPCButtons = !value;
        }

        public bool DiscordAccountOnProfile
        {
            get => App.Settings.Prop.ShowAccountOnRichPresence;
            set => App.Settings.Prop.ShowAccountOnRichPresence = value;
        }

        public bool DisableAppPatchEnabled
        {
            get => App.Settings.Prop.UseDisableAppPatch;
            set => App.Settings.Prop.UseDisableAppPatch = value;
        }

        public bool AutoRestartOnCrash
        {
            get => App.Settings.Prop.AutoRestartOnCrash;
            set => App.Settings.Prop.AutoRestartOnCrash = value;
        }

        public int CrashRestartMaxAttempts
        {
            get => App.Settings.Prop.CrashRestartMaxAttempts;
            set => App.Settings.Prop.CrashRestartMaxAttempts = value;
        }

        public bool CrashRestartRequireInGame
        {
            get => App.Settings.Prop.CrashRestartRequireInGame;
            set => App.Settings.Prop.CrashRestartRequireInGame = value;
        }

        // server regions

        /// <summary>
        /// Roblox exposes no way to ask for a datacenter, so this is a preference rather than a
        /// guarantee: RailStrap looks up where the server it landed in actually is, and can roll
        /// for another one when it isn't somewhere you wanted.
        /// </summary>
        public ObservableCollection<ServerRegionSelection> ServerRegions { get; } =
            new(ServerRegion.All.Select(x => new ServerRegionSelection(x)));

        public bool AutoRerollUnpreferredServer
        {
            get => App.Settings.Prop.AutoRerollUnpreferredServer;
            set => App.Settings.Prop.AutoRerollUnpreferredServer = value;
        }

        public int ServerRegionMaxRerolls
        {
            get => App.Settings.Prop.ServerRegionMaxRerolls;
            set => App.Settings.Prop.ServerRegionMaxRerolls = value;
        }

        public ICommand ClearServerRegionsCommand => new RelayCommand(ClearServerRegions);

        private void ClearServerRegions()
        {
            App.Settings.Prop.PreferredServerRegions.Clear();

            foreach (var region in ServerRegions)
                region.Refresh();
        }

        // connection helper (GoodbyeDPI)

        public bool DpiBypassEnabled
        {
            get => App.Settings.Prop.EnableDpiBypass;
            set
            {
                App.Settings.Prop.EnableDpiBypass = value;
                OnPropertyChanged(nameof(DpiBypassStatus));
            }
        }

        public int DpiBypassMode
        {
            get => App.Settings.Prop.DpiBypassMode;
            set
            {
                if (DpiBypassManager.IsValidMode(value))
                    App.Settings.Prop.DpiBypassMode = value;
            }
        }

        public string DpiBypassStatus
        {
            get
            {
                if (DpiBypassManager.IsRunning)
                    return Strings.Menu_Connection_DpiBypass_Status_Running;

                return DpiBypassManager.IsInstalled
                    ? Strings.Menu_Connection_DpiBypass_Status_Installed
                    : Strings.Menu_Connection_DpiBypass_Status_NotInstalled;
            }
        }

        public ICommand StartDpiBypassCommand => new RelayCommand(StartDpiBypass);

        public ICommand StopDpiBypassCommand => new RelayCommand(StopDpiBypass);

        private async void StartDpiBypass()
        {
            const string LOG_IDENT = "IntegrationsViewModel::StartDpiBypass";

            try
            {
                await DpiBypassManager.EnsureInstalled();
                DpiBypassManager.Start(App.Settings.Prop.DpiBypassMode);
            }
            catch (Exception ex)
            {
                App.Logger.WriteException(LOG_IDENT, ex);
                Frontend.ShowMessageBox(Strings.Menu_Connection_DpiBypass_Failed, System.Windows.MessageBoxImage.Error);
            }

            OnPropertyChanged(nameof(DpiBypassStatus));
        }

        private void StopDpiBypass()
        {
            DpiBypassManager.Stop();
            OnPropertyChanged(nameof(DpiBypassStatus));
        }

        public bool FriendActivityEnabled
        {
            get => App.Settings.Prop.EnableFriendActivityPanel;
            set => App.Settings.Prop.EnableFriendActivityPanel = value;
        }

        public bool HasFriendActivityCookie => !string.IsNullOrEmpty(App.Settings.Prop.FriendActivityCookieEncrypted);

        public System.Windows.Visibility CookieSetVisibility => HasFriendActivityCookie ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;

        // Never round-trip the decrypted cookie back into the UI. The input is only encrypted
        // after the user explicitly saves it, so partial text is not persisted on every keypress.
        private string _friendActivityCookieInput = "";
        public string FriendActivityCookieInput
        {
            get => _friendActivityCookieInput;
            set => _friendActivityCookieInput = value;
        }

        private void SaveFriendActivityCookie()
        {
            string cookie = NormalizeRobloxCookie(FriendActivityCookieInput);

            if (string.IsNullOrEmpty(cookie))
                return;

            App.Settings.Prop.FriendActivityCookieEncrypted = SecureStorage.Protect(cookie);
            _friendActivityCookieInput = "";

            OnPropertyChanged(nameof(FriendActivityCookieInput));
            OnPropertyChanged(nameof(HasFriendActivityCookie));
            OnPropertyChanged(nameof(CookieSetVisibility));
        }

        private void ClearFriendActivityCookie()
        {
            App.Settings.Prop.FriendActivityCookieEncrypted = "";
            _friendActivityCookieInput = "";

            OnPropertyChanged(nameof(FriendActivityCookieInput));
            OnPropertyChanged(nameof(HasFriendActivityCookie));
            OnPropertyChanged(nameof(CookieSetVisibility));
        }

        private static string NormalizeRobloxCookie(string input)
        {
            string cookie = input.Trim().Trim('"', '\'');
            const string cookieName = ".ROBLOSECURITY=";
            int cookieNameIndex = cookie.IndexOf(cookieName, StringComparison.OrdinalIgnoreCase);

            if (cookieNameIndex >= 0)
                cookie = cookie[(cookieNameIndex + cookieName.Length)..];

            int separatorIndex = cookie.IndexOf(';');
            if (separatorIndex >= 0)
                cookie = cookie[..separatorIndex];

            return cookie.Trim();
        }
        public ObservableCollection<CustomIntegration> CustomIntegrations
        {
            get => App.Settings.Prop.CustomIntegrations;
            set => App.Settings.Prop.CustomIntegrations = value;
        }

        public CustomIntegration? SelectedCustomIntegration { get; set; }
        public int SelectedCustomIntegrationIndex { get; set; }
        public bool IsCustomIntegrationSelected => SelectedCustomIntegration is not null;
    }
}
