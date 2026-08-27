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
        public ObservableCollection<GalleryItem> Themes { get; set; } = new();
        public ObservableCollection<GalleryItem> Mods { get; set; } = new();

        public bool Loading { get; set; } = true;
        public Visibility LoadingVisibility => Loading ? Visibility.Visible : Visibility.Collapsed;

        public bool LoadFailed { get; set; } = false;
        public Visibility LoadFailedVisibility => !Loading && LoadFailed ? Visibility.Visible : Visibility.Collapsed;

        public Visibility ContentVisibility => !Loading && !LoadFailed ? Visibility.Visible : Visibility.Collapsed;

        public ICommand InstallCommand => new RelayCommand<GalleryItem>(async item => await Install(item));

        public ICommand UninstallCommand => new RelayCommand<GalleryItem>(Uninstall);

        public GalleryViewModel()
        {
            _ = LoadManifest();
        }

        private async Task LoadManifest()
        {
            var manifest = await GalleryDownloader.GetManifest();

            if (manifest is null)
            {
                Loading = false;
                LoadFailed = true;
                OnPropertyChanged(nameof(LoadingVisibility));
                OnPropertyChanged(nameof(LoadFailedVisibility));
                OnPropertyChanged(nameof(ContentVisibility));
                return;
            }

            Themes.Clear();
            foreach (var item in manifest.Themes)
            {
                item.Kind = GalleryItemKind.Theme;
                Themes.Add(item);
            }

            Mods.Clear();
            foreach (var item in manifest.Mods)
            {
                item.Kind = GalleryItemKind.Mod;
                Mods.Add(item);
            }

            Loading = false;
            OnPropertyChanged(nameof(Loading));
            OnPropertyChanged(nameof(LoadingVisibility));
            OnPropertyChanged(nameof(ContentVisibility));
            OnPropertyChanged(nameof(Themes));
            OnPropertyChanged(nameof(Mods));
        }

        public bool IsInstalled(GalleryItem item) =>
            App.Gallery.Prop.Installed.Any(x => x.Name == item.Name && x.Kind == item.Kind);

        private async Task Install(GalleryItem? item)
        {
            if (item is null || IsInstalled(item))
                return;

            try
            {
                string targetDir = item.Kind == GalleryItemKind.Theme
                    ? Path.Combine(Paths.CustomThemes, item.Name)
                    : Paths.Modifications;

                var files = await GalleryDownloader.DownloadAndExtract(item, targetDir);

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
                App.Logger.WriteException("GalleryViewModel::Install", ex);
                Frontend.ShowMessageBox(string.Format(Strings.Menu_Gallery_InstallFailed, item.Name, ex.Message), System.Windows.MessageBoxImage.Error);
                return;
            }

            OnPropertyChanged(nameof(Themes));
            OnPropertyChanged(nameof(Mods));
        }

        private void Uninstall(GalleryItem? item)
        {
            if (item is null)
                return;

            var installed = App.Gallery.Prop.Installed.FirstOrDefault(x => x.Name == item.Name && x.Kind == item.Kind);

            if (installed is null)
                return;

            foreach (string file in installed.Files)
            {
                try
                {
                    if (File.Exists(file))
                        File.Delete(file);
                }
                catch (Exception ex)
                {
                    App.Logger.WriteException("GalleryViewModel::Uninstall", ex);
                }
            }

            if (item.Kind == GalleryItemKind.Theme)
            {
                string themeDir = Path.Combine(Paths.CustomThemes, item.Name);

                if (Directory.Exists(themeDir) && !Directory.EnumerateFileSystemEntries(themeDir).Any())
                    Directory.Delete(themeDir);
            }

            App.Gallery.Prop.Installed.Remove(installed);
            App.Gallery.Save();

            OnPropertyChanged(nameof(Themes));
            OnPropertyChanged(nameof(Mods));
        }
    }
}
