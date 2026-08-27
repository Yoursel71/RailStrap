using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;

using CommunityToolkit.Mvvm.Input;
using RailStrap.Integrations;
using RailStrap.Models.APIs.Roblox;

namespace RailStrap.UI.ViewModels.Dialogs
{
    public class FriendActivityViewModel : NotifyPropertyChangedViewModel
    {
        public ObservableCollection<FriendActivityEntry> Friends { get; set; } = new();

        public bool Loading { get; set; } = true;
        public Visibility LoadingVisibility => Loading ? Visibility.Visible : Visibility.Collapsed;

        public string ErrorMessage { get; set; } = "";
        public Visibility ErrorVisibility => !Loading && !string.IsNullOrEmpty(ErrorMessage) ? Visibility.Visible : Visibility.Collapsed;

        public Visibility ContentVisibility => !Loading && string.IsNullOrEmpty(ErrorMessage) && Friends.Any() ? Visibility.Visible : Visibility.Collapsed;

        public Visibility EmptyVisibility => !Loading && string.IsNullOrEmpty(ErrorMessage) && !Friends.Any() ? Visibility.Visible : Visibility.Collapsed;

        public ICommand RefreshCommand => new RelayCommand(async () => await Load());

        public FriendActivityViewModel()
        {
            _ = Load();
        }

        private async Task Load()
        {
            Loading = true;
            ErrorMessage = "";
            NotifyStateChanged();

            try
            {
                var friends = await FriendActivityService.GetFriendActivity(App.Settings.Prop.FriendActivityCookieEncrypted);

                Friends.Clear();
                foreach (var friend in friends)
                    Friends.Add(friend);
            }
            catch (Exception ex)
            {
                App.Logger.WriteException("FriendActivityViewModel::Load", ex);
                ErrorMessage = ex.Message;
            }

            Loading = false;
            NotifyStateChanged();
        }

        private void NotifyStateChanged()
        {
            OnPropertyChanged(nameof(Loading));
            OnPropertyChanged(nameof(ErrorMessage));
            OnPropertyChanged(nameof(LoadingVisibility));
            OnPropertyChanged(nameof(ErrorVisibility));
            OnPropertyChanged(nameof(ContentVisibility));
            OnPropertyChanged(nameof(EmptyVisibility));
            OnPropertyChanged(nameof(Friends));
        }
    }
}
