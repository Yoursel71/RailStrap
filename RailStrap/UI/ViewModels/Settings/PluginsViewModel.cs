using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;

using CommunityToolkit.Mvvm.Input;

using RailStrap.Models.Entities;

namespace RailStrap.UI.ViewModels.Settings
{
    public class PluginsViewModel : NotifyPropertyChangedViewModel
    {
        private const string DISABLED_SUFFIX = ".disabled";

        public ObservableCollection<StudioPluginEntry> Plugins { get; set; } = new();

        public Visibility EmptyVisibility => Plugins.Any() ? Visibility.Collapsed : Visibility.Visible;

        public ICommand OpenFolderCommand => new RelayCommand(OpenFolder);

        public ICommand RefreshCommand => new RelayCommand(Populate);

        public ICommand ToggleEnabledCommand => new RelayCommand<StudioPluginEntry>(ToggleEnabled);

        public ICommand DeleteCommand => new RelayCommand<StudioPluginEntry>(Delete);

        public PluginsViewModel()
        {
            Populate();
        }

        private void Populate()
        {
            Plugins.Clear();

            Directory.CreateDirectory(Paths.RobloxStudioPlugins);

            foreach (string file in Directory.GetFiles(Paths.RobloxStudioPlugins))
            {
                string name = Path.GetFileName(file);
                bool disabled = name.EndsWith(DISABLED_SUFFIX, StringComparison.OrdinalIgnoreCase);

                Plugins.Add(new StudioPluginEntry
                {
                    Name = disabled ? name[..^DISABLED_SUFFIX.Length] : name,
                    FullPath = file,
                    Enabled = !disabled
                });
            }

            OnPropertyChanged(nameof(Plugins));
            OnPropertyChanged(nameof(EmptyVisibility));
        }

        private void OpenFolder()
        {
            Directory.CreateDirectory(Paths.RobloxStudioPlugins);
            Utilities.ShellExecute(Paths.RobloxStudioPlugins);
        }

        private void ToggleEnabled(StudioPluginEntry? entry)
        {
            if (entry is null)
                return;

            string newPath = entry.Enabled
                ? entry.FullPath + DISABLED_SUFFIX
                : entry.FullPath[..^DISABLED_SUFFIX.Length];

            File.Move(entry.FullPath, newPath);

            Populate();
        }

        private void Delete(StudioPluginEntry? entry)
        {
            if (entry is null)
                return;

            MessageBoxResult result = Frontend.ShowMessageBox(
                string.Format(Strings.Menu_Plugins_DeleteConfirm, entry.Name),
                MessageBoxImage.Warning,
                MessageBoxButton.YesNo
            );

            if (result != MessageBoxResult.Yes)
                return;

            File.Delete(entry.FullPath);

            Populate();
        }
    }
}
