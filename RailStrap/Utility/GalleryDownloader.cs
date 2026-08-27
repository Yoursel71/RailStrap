using ICSharpCode.SharpZipLib.Zip;

using RailStrap.Models.APIs.Config;

namespace RailStrap.Utility
{
    static class GalleryDownloader
    {
        private const long MaxDownloadBytes = 50 * 1024 * 1024;
        private const long MaxExtractedBytes = 200 * 1024 * 1024;
        private const int MaxFileCount = 1000;

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
            if (!Uri.TryCreate(item.DownloadUrl, UriKind.Absolute, out Uri? downloadUri) || downloadUri.Scheme != Uri.UriSchemeHttps)
                throw new InvalidDataException("Gallery downloads must use a valid HTTPS URL.");

            using var response = await App.HttpClient.GetAsync(downloadUri, HttpCompletionOption.ResponseHeadersRead);
            response.EnsureSuccessStatusCode();

            if (response.Content.Headers.ContentLength > MaxDownloadBytes)
                throw new InvalidDataException("The gallery download is larger than the allowed 50 MB limit.");

            using var zipStream = await response.Content.ReadAsStreamAsync();
            using var zipInputStream = new ZipInputStream(zipStream);

            var extractedFiles = new List<string>();
            string targetRoot = Path.GetFullPath(targetDir).TrimEnd(Path.DirectorySeparatorChar) + Path.DirectorySeparatorChar;
            long extractedBytes = 0;

            try
            {
                ZipEntry? entry;
                while ((entry = zipInputStream.GetNextEntry()) is not null)
                {
                    if (!entry.IsFile)
                        continue;

                    if (extractedFiles.Count >= MaxFileCount)
                        throw new InvalidDataException($"The gallery archive contains more than {MaxFileCount} files.");

                    string outputPath = Path.GetFullPath(Path.Combine(targetRoot, entry.Name));

                    if (!outputPath.StartsWith(targetRoot, StringComparison.OrdinalIgnoreCase))
                        throw new InvalidDataException($"The archive contains an unsafe path: {entry.Name}");

                    if (File.Exists(outputPath))
                        throw new IOException($"The file already exists and will not be overwritten: {outputPath}");

                    Directory.CreateDirectory(Path.GetDirectoryName(outputPath)!);

                    using (var outputStream = new FileStream(outputPath, FileMode.CreateNew, FileAccess.Write, FileShare.None))
                    {
                        extractedFiles.Add(outputPath);
                        var buffer = new byte[81920];
                        int read;

                        while ((read = await zipInputStream.ReadAsync(buffer)) > 0)
                        {
                            extractedBytes += read;

                            if (extractedBytes > MaxExtractedBytes)
                                throw new InvalidDataException("The extracted gallery item is larger than the allowed 200 MB limit.");

                            await outputStream.WriteAsync(buffer.AsMemory(0, read));
                        }
                    }

                }
            }
            catch
            {
                DeleteExtractedFiles(extractedFiles);
                throw;
            }

            if (extractedFiles.Count == 0)
                throw new InvalidDataException("The gallery archive does not contain any files.");

            return extractedFiles;
        }

        public static void DeleteExtractedFiles(IEnumerable<string> files)
        {
            foreach (string file in files.OrderByDescending(x => x.Length))
            {
                try
                {
                    if (File.Exists(file))
                        File.Delete(file);
                }
                catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
                {
                    App.Logger.WriteException("GalleryDownloader::DeleteExtractedFiles", ex);
                }
            }
        }
    }
}
