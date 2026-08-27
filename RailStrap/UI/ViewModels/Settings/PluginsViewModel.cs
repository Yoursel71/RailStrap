using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;

using CommunityToolkit.Mvvm.Input;
using Microsoft.Win32;

using RailStrap.Models.Entities;

namespace RailStrap.UI.ViewModels.Settings
{
    public class PluginsViewModel : NotifyPropertyChangedViewModel
    {
        private const string DISABLED_SUFFIX = ".disabled";
        private static readonly HashSet<string> SupportedExtensions = new(StringComparer.OrdinalIgnoreCase)
        {
            ".rbxm",
            ".rbxmx"
        };

        public ObservableCollection<StudioPluginEntry> Plugins { get; set; } = new();

        public Visibility EmptyVisibility => Plugins.Any() ? Visibility.Collapsed : Visibility.Visible;

        public ICommand OpenFolderCommand => new RelayCommand(OpenFolder);

        public ICommand RefreshCommand => new RelayCommand(Populate);

        public ICommand ImportCommand => new RelayCommand(Import);

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
                string fileName = Path.GetFileName(file);
                bool disabled = fileName.EndsWith(DISABLED_SUFFIX, StringComparison.OrdinalIgnoreCase);
                string pluginFileName = disabled ? fileName[..^DISABLED_SUFFIX.Length] : fileName;

                if (!SupportedExtensions.Contains(Path.GetExtension(pluginFileName)))
                    continue;

                Plugins.Add(new StudioPluginEntry
                {
                    Name = Path.GetFileNameWithoutExtension(pluginFileName),
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

        private void Import()
        {
            var dialog = new OpenFileDialog
            {
                Filter = "Roblox Studio plugins (*.rbxm;*.rbxmx)|*.rbxm;*.rbxmx",
                Multiselect = true
            };

            if (dialog.ShowDialog() != true)
                return;

            Directory.CreateDirectory(Paths.RobloxStudioPlugins);

            foreach (string sourcePath in dialog.FileNames)
            {
                try
                {
                    string destinationPath = Path.Combine(Paths.RobloxStudioPlugins, Path.GetFileName(sourcePath));
                    File.Copy(sourcePath, destinationPath, false);
                }
                catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
                {
                    App.Logger.WriteException("PluginsViewModel::Import", ex);
                    Frontend.ShowMessageBox(
                        string.Format(Strings.Menu_Plugins_ImportFailed, Path.GetFileName(sourcePath), ex.Message),
                        MessageBoxImage.Error);
                }
            }

            Populate();
        }

        private void ToggleEnabled(StudioPluginEntry? entry)
        {
            if (entry is null)
                return;

            try
            {
                string newPath = entry.Enabled
                    ? entry.FullPath + DISABLED_SUFFIX
                    : entry.FullPath[..^DISABLED_SUFFIX.Length];

                File.Move(entry.FullPath, newPath);
            }
            catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
            {
                App.Logger.WriteException("PluginsViewModel::ToggleEnabled", ex);
                Frontend.ShowMessageBox(ex.Message, MessageBoxImage.Error);
            }

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

            try
            {
                File.Delete(entry.FullPath);
            }
            catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
            {
                App.Logger.WriteException("PluginsViewModel::Delete", ex);
                Frontend.ShowMessageBox(ex.Message, MessageBoxImage.Error);
                return;
            }

            Populate();
        }
    }
}
