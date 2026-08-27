using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;

using CommunityToolkit.Mvvm.Input;

using RailStrap.Models.APIs.Config;
using RailStrap.Models.Persistable;
using RailStrap.Utility;

namespace RailStrap.UI.ViewModels.Settings
{
    public class GalleryViewModel : NotifyPropertyChangedViewModel
    {
        private readonly HashSet<string> _busyItems = new(StringComparer.OrdinalIgnoreCase);
        private GalleryManifest? _manifest;

        public ObservableCollection<GalleryItem> Themes { get; set; } = new();
        public ObservableCollection<GalleryItem> Mods { get; set; } = new();

        public bool Loading { get; set; } = true;
        public Visibility LoadingVisibility => Loading ? Visibility.Visible : Visibility.Collapsed;

        public bool LoadFailed { get; set; } = false;
        public Visibility LoadFailedVisibility => !Loading && LoadFailed ? Visibility.Visible : Visibility.Collapsed;

        public Visibility ContentVisibility => !Loading && !LoadFailed ? Visibility.Visible : Visibility.Collapsed;

        public ICommand InstallCommand => new RelayCommand<GalleryItem>(async item => await Install(item));

        public ICommand UninstallCommand => new RelayCommand<GalleryItem>(Uninstall);

        public ICommand RetryCommand => new RelayCommand(async () => await LoadManifest());

        public GalleryViewModel()
        {
            _ = LoadManifest();
        }

        private async Task LoadManifest()
        {
            Loading = true;
            LoadFailed = false;
            NotifyStateChanged();

            _manifest = await GalleryDownloader.GetManifest();

            if (_manifest is null)
            {
                Loading = false;
                LoadFailed = true;
                NotifyStateChanged();
                return;
            }

            PopulateItems();

            Loading = false;
            NotifyStateChanged();
        }

        private void PopulateItems()
        {
            if (_manifest is null)
                return;

            Themes.Clear();
            foreach (var item in _manifest.Themes)
            {
                item.Kind = GalleryItemKind.Theme;
                item.IsInstalled = IsInstalled(item);
                Themes.Add(item);
            }

            Mods.Clear();
            foreach (var item in _manifest.Mods)
            {
                item.Kind = GalleryItemKind.Mod;
                item.IsInstalled = IsInstalled(item);
                Mods.Add(item);
            }

            OnPropertyChanged(nameof(Themes));
            OnPropertyChanged(nameof(Mods));
        }

        private void NotifyStateChanged()
        {
            OnPropertyChanged(nameof(Loading));
            OnPropertyChanged(nameof(LoadFailed));
            OnPropertyChanged(nameof(LoadingVisibility));
            OnPropertyChanged(nameof(LoadFailedVisibility));
            OnPropertyChanged(nameof(ContentVisibility));
        }

        public bool IsInstalled(GalleryItem item) =>
            App.Gallery.Prop.Installed.Any(x => x.Name == item.Name && x.Kind == item.Kind);

        private async Task Install(GalleryItem? item)
        {
            if (item is null || IsInstalled(item) || !_busyItems.Add(GetItemKey(item)))
                return;

            List<string> files = new();

            try
            {
                if (string.IsNullOrWhiteSpace(item.Name) || Path.GetFileName(item.Name) != item.Name)
                    throw new InvalidDataException("The gallery item has an invalid name.");

                string targetDir = item.Kind == GalleryItemKind.Theme
                    ? Path.Combine(Paths.CustomThemes, item.Name)
                    : Paths.Modifications;

                files = await GalleryDownloader.DownloadAndExtract(item, targetDir);

                if (item.Kind == GalleryItemKind.Theme && !files.Any(x => Path.GetFileName(x).Equals("Theme.xml", StringComparison.OrdinalIgnoreCase)))
                    throw new InvalidDataException(Strings.CustomTheme_Add_Errors_ZipMissingThemeFile);

                App.Gallery.Prop.Installed.Add(new InstalledGalleryItem
                {
                    Name = item.Name,
                    Kind = item.Kind,
                    Files = files
                });

                App.Gallery.Save();
            }
            catch (Exception ex)
            {
                try
                {
                    GalleryDownloader.DeleteExtractedFiles(files);
                }
                catch (Exception cleanupEx)
                {
                    App.Logger.WriteException("GalleryViewModel::InstallCleanup", cleanupEx);
                }

                App.Logger.WriteException("GalleryViewModel::Install", ex);
                Frontend.ShowMessageBox(string.Format(Strings.Menu_Gallery_InstallFailed, item.Name, ex.Message), System.Windows.MessageBoxImage.Error);
            }
            finally
            {
                _busyItems.Remove(GetItemKey(item));
                PopulateItems();
            }
        }

        private void Uninstall(GalleryItem? item)
        {
            if (item is null)
                return;

            var installed = App.Gallery.Prop.Installed.FirstOrDefault(x => x.Name == item.Name && x.Kind == item.Kind);

            if (installed is null)
                return;

            MessageBoxResult result = Frontend.ShowMessageBox(
                string.Format(Strings.Menu_Gallery_UninstallConfirm, item.Name),
                MessageBoxImage.Warning,
                MessageBoxButton.YesNo);

            if (result != MessageBoxResult.Yes)
                return;

            string allowedRoot = item.Kind == GalleryItemKind.Theme
                ? Path.Combine(Paths.CustomThemes, item.Name)
                : Paths.Modifications;
            bool deleteFailed = false;

            foreach (string file in installed.Files)
            {
                try
                {
                    if (!IsPathInsideRoot(file, allowedRoot))
                    {
                        App.Logger.WriteLine("GalleryViewModel::Uninstall", $"Refusing to delete gallery path outside its install root: {file}");
                        deleteFailed = true;
                        continue;
                    }

                    if (File.Exists(file))
                        File.Delete(file);
                }
                catch (Exception ex)
                {
                    App.Logger.WriteException("GalleryViewModel::Uninstall", ex);
                    deleteFailed = true;
                }
            }

            if (deleteFailed)
            {
                Frontend.ShowMessageBox(
                    string.Format(Strings.Menu_Gallery_UninstallFailed, item.Name),
                    MessageBoxImage.Error);
                return;
            }

            if (item.Kind == GalleryItemKind.Theme)
            {
                string themeDir = Path.Combine(Paths.CustomThemes, item.Name);

                if (Directory.Exists(themeDir) && !Directory.EnumerateFileSystemEntries(themeDir).Any())
                    Directory.Delete(themeDir);

                if (App.Settings.Prop.SelectedCustomTheme == item.Name)
                    App.Settings.Prop.SelectedCustomTheme = null;
            }

            App.Gallery.Prop.Installed.Remove(installed);
            App.Gallery.Save();

            PopulateItems();
        }

        private static string GetItemKey(GalleryItem item) => $"{item.Kind}:{item.Name}";

        private static bool IsPathInsideRoot(string path, string root)
        {
            string fullPath = Path.GetFullPath(path);
            string fullRoot = Path.GetFullPath(root).TrimEnd(Path.DirectorySeparatorChar) + Path.DirectorySeparatorChar;
            return fullPath.StartsWith(fullRoot, StringComparison.OrdinalIgnoreCase);
        }
    }
}
