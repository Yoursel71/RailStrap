using System.Windows;
using System.Windows.Input;
using System.Text.Json.Nodes;
using CommunityToolkit.Mvvm.Input;
using ICSharpCode.SharpZipLib.Zip;
using Microsoft.Win32;

namespace RailStrap.UI.ViewModels.Settings
{
    public class RailStrapViewModel : NotifyPropertyChangedViewModel
    {
        public WebEnvironment[] WebEnvironments => Enum.GetValues<WebEnvironment>();

        public bool UpdateCheckingEnabled
        {
            get => App.Settings.Prop.CheckForUpdates;
            set => App.Settings.Prop.CheckForUpdates = value;
        }

        public bool AnalyticsEnabled
        {
            get => App.Settings.Prop.EnableAnalytics;
            set => App.Settings.Prop.EnableAnalytics = value;
        }

        public WebEnvironment WebEnvironment
        {
            get => App.Settings.Prop.WebEnvironment;
            set => App.Settings.Prop.WebEnvironment = value;
        }

        public Visibility WebEnvironmentVisibility => App.Settings.Prop.DeveloperMode ? Visibility.Visible : Visibility.Collapsed;

        public bool ShouldExportConfig { get; set; } = true;

        public bool ShouldExportLogs { get; set; } = true;

        public bool ShouldExportPlugins { get; set; } = true;

        public ICommand ExportDataCommand => new RelayCommand(ExportData);

        public ICommand ImportDataCommand => new RelayCommand(async () => await ImportData());

        public ICommand ClearDownloadCacheCommand => new RelayCommand(ClearDownloadCache);

        private void ClearDownloadCache()
        {
            const string LOG_IDENT = "RailStrapViewModel::ClearDownloadCache";

            if (!Directory.Exists(Paths.Downloads) || !Directory.EnumerateFileSystemEntries(Paths.Downloads).Any())
            {
                Frontend.ShowMessageBox(Strings.Menu_RailStrap_ClearDownloadCache_Empty, MessageBoxImage.Information);
                return;
            }

            MessageBoxResult result = Frontend.ShowMessageBox(
                Strings.Menu_RailStrap_ClearDownloadCache_Confirm,
                MessageBoxImage.Warning,
                MessageBoxButton.YesNo);

            if (result != MessageBoxResult.Yes)
                return;

            try
            {
                Directory.Delete(Paths.Downloads, true);
                Directory.CreateDirectory(Paths.Downloads);

                Frontend.ShowMessageBox(Strings.Menu_RailStrap_ClearDownloadCache_Success, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                App.Logger.WriteLine(LOG_IDENT, "Failed to clear download cache");
                App.Logger.WriteException(LOG_IDENT, ex);

                Frontend.ShowMessageBox(string.Format(Strings.Menu_RailStrap_ClearDownloadCache_Failed, ex.Message), MessageBoxImage.Error);
            }
        }

        private void ExportData()
        {
            string timestamp = DateTime.UtcNow.ToString("yyyyMMdd'T'HHmmss'Z'");

            var dialog = new SaveFileDialog 
            { 
                FileName = $"RailStrap-export-{timestamp}.zip",
                Filter = $"{Strings.FileTypes_ZipArchive}|*.zip" 
            };

            if (dialog.ShowDialog() != true)
                return;

            using var memStream = new MemoryStream();
            using var zipStream = new ZipOutputStream(memStream);

            if (ShouldExportConfig)
            {
                var files = new List<string>()
                {
                    App.State.FileLocation,
                    App.FastFlags.FileLocation,
                    App.Gallery.FileLocation,
                    App.PlaytimeStats.FileLocation
                };

                AddFilesToZipStream(zipStream, files, "Config/");
                AddRedactedSettingsToZipStream(zipStream, "Config/");
            }

            if (ShouldExportLogs && Directory.Exists(Paths.Logs))
            {
                var files = Directory.GetFiles(Paths.Logs)
                    .Where(x => !x.Equals(App.Logger.FileLocation, StringComparison.OrdinalIgnoreCase));

                AddFilesToZipStream(zipStream, files, "Logs/");
            }

            if (ShouldExportPlugins && Directory.Exists(Paths.RobloxStudioPlugins))
                AddFilesToZipStream(zipStream, Directory.GetFiles(Paths.RobloxStudioPlugins), "Plugins/");

            zipStream.CloseEntry();
            zipStream.Finish();
            memStream.Position = 0;

            using var outputStream = File.Create(dialog.FileName);
            memStream.CopyTo(outputStream);

            Process.Start("explorer.exe", $"/select,\"{dialog.FileName}\"");
        }

        private async Task ImportData()
        {
            const string LOG_IDENT = "RailStrapViewModel::ImportData";

            var dialog = new OpenFileDialog
            {
                Filter = $"{Strings.FileTypes_ZipArchive}|*.zip"
            };

            if (dialog.ShowDialog() != true)
                return;

            try
            {
                using var fileStream = File.OpenRead(dialog.FileName);
                using var zipStream = new ZipInputStream(fileStream);

                int imported = 0;
                ZipEntry? entry;

                while ((entry = zipStream.GetNextEntry()) is not null)
                {
                    if (!entry.IsFile)
                        continue;

                    string? targetPath;

                    if (entry.Name.StartsWith("Plugins/", StringComparison.OrdinalIgnoreCase))
                    {
                        string fileName = Path.GetFileName(entry.Name);

                        if (string.IsNullOrEmpty(fileName))
                            continue;

                        Directory.CreateDirectory(Paths.RobloxStudioPlugins);
                        targetPath = Path.Combine(Paths.RobloxStudioPlugins, fileName);
                    }
                    else
                    {
                        targetPath = Path.GetFileName(entry.Name) switch
                        {
                            "Settings.json" => App.Settings.FileLocation,
                            "State.json" => App.State.FileLocation,
                            "ClientAppSettings.json" or "FastFlags.json" => App.FastFlags.FileLocation,
                            "Gallery.json" => App.Gallery.FileLocation,
                            "PlaytimeStats.json" => App.PlaytimeStats.FileLocation,
                            _ => null
                        };
                    }

                    if (targetPath is null)
                        continue;

                    using (var outputStream = File.Create(targetPath))
                        zipStream.CopyTo(outputStream);

                    imported++;
                }

                if (imported == 0)
                {
                    Frontend.ShowMessageBox(Strings.Menu_RailStrap_ImportData_NoneFound, MessageBoxImage.Warning);
                    return;
                }

                App.Settings.Load(false);
                App.State.Load(false);
                App.FastFlags.Load(false);
                App.Gallery.Load(false);
                App.PlaytimeStats.Load(false);

                await ReinstallMissingGalleryItems();

                Frontend.ShowMessageBox(Strings.Menu_RailStrap_ImportData_Success, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                App.Logger.WriteLine(LOG_IDENT, "Failed to import data");
                App.Logger.WriteException(LOG_IDENT, ex);

                Frontend.ShowMessageBox(string.Format(Strings.Menu_RailStrap_ImportData_Failed, ex.Message), MessageBoxImage.Error);
            }
        }

        // Gallery.json round-trips as part of Config, but the actual mod/theme content it
        // references does not (redownloading beats bundling potentially large binary content).
        private async Task ReinstallMissingGalleryItems()
        {
            const string LOG_IDENT = "RailStrapViewModel::ReinstallMissingGalleryItems";

            var missing = App.Gallery.Prop.Installed.Where(x => !x.Files.All(File.Exists)).ToList();

            if (missing.Count == 0)
                return;

            var manifest = await GalleryDownloader.GetManifest();

            if (manifest is null)
                return;

            foreach (var installed in missing)
            {
                var source = installed.Kind == GalleryItemKind.Theme ? manifest.Themes : manifest.Mods;
                var match = source.FirstOrDefault(x => x.Name == installed.Name);

                if (match is null)
                    continue;

                try
                {
                    string targetDir = installed.Kind == GalleryItemKind.Theme
                        ? Path.Combine(Paths.CustomThemes, installed.Name)
                        : Paths.Modifications;

                    installed.Files = await GalleryDownloader.DownloadAndExtract(match, targetDir);
                }
                catch (Exception ex)
                {
                    App.Logger.WriteLine(LOG_IDENT, $"Failed to reinstall gallery item '{installed.Name}'");
                    App.Logger.WriteException(LOG_IDENT, ex);
                }
            }

            App.Gallery.Save();
        }

        // Settings.json's FriendActivityCookieEncrypted is DPAPI-encrypted per machine/user, so it
        // can never be decrypted after an import onto a different machine or account - export a
        // redacted copy instead of a blob that would silently fail to decrypt.
        private void AddRedactedSettingsToZipStream(ZipOutputStream zipStream, string directory)
        {
            const string LOG_IDENT = "RailStrapViewModel::AddRedactedSettingsToZipStream";

            if (!File.Exists(App.Settings.FileLocation))
                return;

            try
            {
                string json = File.ReadAllText(App.Settings.FileLocation);
                JsonObject? root = JsonNode.Parse(json)?.AsObject();

                if (root is not null && root.ContainsKey(nameof(RailStrap.Models.Persistable.Settings.FriendActivityCookieEncrypted)))
                    root[nameof(RailStrap.Models.Persistable.Settings.FriendActivityCookieEncrypted)] = "";

                byte[] bytes = Encoding.UTF8.GetBytes(root?.ToJsonString(new JsonSerializerOptions { WriteIndented = true }) ?? json);

                var entry = new ZipEntry(directory + Path.GetFileName(App.Settings.FileLocation));
                entry.DateTime = DateTime.Now;

                zipStream.PutNextEntry(entry);
                zipStream.Write(bytes, 0, bytes.Length);
            }
            catch (Exception ex)
            {
                App.Logger.WriteLine(LOG_IDENT, "Failed to redact settings for export");
                App.Logger.WriteException(LOG_IDENT, ex);
            }
        }

        private void AddFilesToZipStream(ZipOutputStream zipStream, IEnumerable<string> files, string directory)
        {
            const string LOG_IDENT = "RailStrapViewModel::AddFilesToZipStream";

            foreach (string file in files)
            {
                if (!File.Exists(file))
                    continue;

                try
                {
                    using FileStream fileStream = File.OpenRead(file);

                    var entry = new ZipEntry(directory + Path.GetFileName(file));
                    entry.DateTime = DateTime.Now;

                    zipStream.PutNextEntry(entry);

                    fileStream.CopyTo(zipStream);
                }
                catch (IOException ex)
                {
                    App.Logger.WriteLine(LOG_IDENT, $"Failed to open '{file}'");
                    App.Logger.WriteException(LOG_IDENT, ex);
                }
            }
        }
    }
}
