using System.Collections.ObjectModel;
using System.Windows.Input;

using Microsoft.Win32;

using CommunityToolkit.Mvvm.Input;

using RailStrap.UI.Elements.Dialogs;
using RailStrap.Utility;

namespace RailStrap.UI.ViewModels.Settings
{
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
