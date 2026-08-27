using System.Windows;
using System.Windows.Input;
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

        public ICommand ExportDataCommand => new RelayCommand(ExportData);

        public ICommand ImportDataCommand => new RelayCommand(ImportData);

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
                    App.Settings.FileLocation,
                    App.State.FileLocation,
                    App.FastFlags.FileLocation
                };

                AddFilesToZipStream(zipStream, files, "Config/");
            }

            if (ShouldExportLogs && Directory.Exists(Paths.Logs))
            {
                var files = Directory.GetFiles(Paths.Logs)
                    .Where(x => !x.Equals(App.Logger.FileLocation, StringComparison.OrdinalIgnoreCase));

                AddFilesToZipStream(zipStream, files, "Logs/");
            }

            zipStream.CloseEntry();
            zipStream.Finish();
            memStream.Position = 0;

            using var outputStream = File.OpenWrite(dialog.FileName);
            memStream.CopyTo(outputStream);

            Process.Start("explorer.exe", $"/select,\"{dialog.FileName}\"");
        }

        private void ImportData()
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

                    string? targetPath = Path.GetFileName(entry.Name) switch
                    {
                        "Settings.json" => App.Settings.FileLocation,
                        "State.json" => App.State.FileLocation,
                        "FastFlags.json" => App.FastFlags.FileLocation,
                        _ => null
                    };

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

                Frontend.ShowMessageBox(Strings.Menu_RailStrap_ImportData_Success, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                App.Logger.WriteLine(LOG_IDENT, "Failed to import data");
                App.Logger.WriteException(LOG_IDENT, ex);

                Frontend.ShowMessageBox(string.Format(Strings.Menu_RailStrap_ImportData_Failed, ex.Message), MessageBoxImage.Error);
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
