using ICSharpCode.SharpZipLib.Zip;

using RailStrap.Models.APIs.Config;

namespace RailStrap.Utility
{
    static class GalleryDownloader
    {
        // hosted alongside the app itself so no separate infrastructure is needed
        private const string MANIFEST_URL = "https://raw.githubusercontent.com/Yoursel71/RailStrap/main/gallery/manifest.json";

        public static async Task<GalleryManifest?> GetManifest()
        {
            const string LOG_IDENT = "GalleryDownloader::GetManifest";

            try
            {
                return await Http.GetJson<GalleryManifest>(MANIFEST_URL);
            }
            catch (Exception ex)
            {
                App.Logger.WriteException(LOG_IDENT, ex);
                return null;
            }
        }

        /// <summary>
        /// Downloads a gallery item's zip and extracts it into <paramref name="targetDir"/>,
        /// preserving the zip's internal relative folder structure. Returns the list of files written.
        /// </summary>
        public static async Task<List<string>> DownloadAndExtract(GalleryItem item, string targetDir)
        {
            var response = await App.HttpClient.GetAsync(item.DownloadUrl);
            response.EnsureSuccessStatusCode();

            using var zipStream = await response.Content.ReadAsStreamAsync();
            using var zipInputStream = new ZipInputStream(zipStream);

            var extractedFiles = new List<string>();

            ZipEntry? entry;
            while ((entry = zipInputStream.GetNextEntry()) is not null)
            {
                if (!entry.IsFile)
                    continue;

                string outputPath = Path.Combine(targetDir, entry.Name);
                Directory.CreateDirectory(Path.GetDirectoryName(outputPath)!);

                using (var outputStream = File.Create(outputPath))
                    zipInputStream.CopyTo(outputStream);

                extractedFiles.Add(outputPath);
            }

            return extractedFiles;
        }
    }
}
