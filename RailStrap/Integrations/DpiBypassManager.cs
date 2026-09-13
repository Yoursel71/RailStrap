using System.ComponentModel;
using System.IO.Compression;
using System.Security.Cryptography;

namespace RailStrap.Integrations
{
    /// <summary>
    /// Optional helper that runs GoodbyeDPI alongside Roblox.
    ///
    /// Some ISPs - Turkey's in particular, where Roblox has been blocked since August 2024 - break
    /// the connection with DNS poisoning and SNI-based deep packet inspection rather than by
    /// blocking addresses. GoodbyeDPI works around that locally, and mode 6
    /// (-f 2 -e 2 --wrong-seq --reverse-frag --max-payload) paired with the any-country DNS
    /// redirect is the combination Turkish players have settled on.
    ///
    /// RailStrap does not bundle GoodbyeDPI. It is fetched from the upstream GitHub release on
    /// demand and the archive hash is checked before anything is extracted or run. It needs
    /// administrator rights because it loads the WinDivert driver, so starting it raises a UAC
    /// prompt - that is expected, and it is skipped entirely when it's already running.
    /// </summary>
    static class DpiBypassManager
    {
        private const string Version = "0.2.2";

        private const string DownloadUrl = "https://github.com/ValdikSS/GoodbyeDPI/releases/download/0.2.2/goodbyedpi-0.2.2.zip";

        // sha256 of goodbyedpi-0.2.2.zip as published by ValdikSS, verified against the live
        // release asset. A mismatch means the download is not what we expect, so it is discarded.
        private const string DownloadSha256 = "00a2f8b99cd817f8c7fc4c449033015f039d18af213de78cb66bf202277c0628";

        private const long MaxDownloadBytes = 16 * 1024 * 1024;

        private const string ProcessName = "goodbyedpi";

        /// <summary>
        /// The any-country DNS redirect that ships as 2_any_country_dnsredir.cmd, minus the mode
        /// number. Yandex's resolver on a non-standard port sidesteps ISP DNS interception.
        /// </summary>
        private const string DnsRedirectArguments = "--dns-addr 77.88.8.8 --dns-port 1253 --dnsv6-addr 2a02:6b8::feed:0ff --dnsv6-port 1253";

        public static string InstallDirectory => Path.Combine(Paths.Integrations, "GoodbyeDPI", Version);

        private static string ArchitectureFolder => Environment.Is64BitOperatingSystem ? "x86_64" : "x86";

        public static string ExecutablePath => Path.Combine(InstallDirectory, $"goodbyedpi-{Version}", ArchitectureFolder, "goodbyedpi.exe");

        public static bool IsInstalled => File.Exists(ExecutablePath);

        public static bool IsRunning
        {
            get
            {
                foreach (var process in Utilities.GetProcessesSafe())
                {
                    using (process)
                    {
                        try
                        {
                            if (process.ProcessName.Equals(ProcessName, StringComparison.OrdinalIgnoreCase) && !process.HasExited)
                                return true;
                        }
                        catch (Exception)
                        {
                            // processes we can't query are not ours to care about
                        }
                    }
                }

                return false;
            }
        }

        /// <summary>
        /// Valid GoodbyeDPI modesets. 1-4 are the legacy sets, 5-9 the modern ones.
        /// </summary>
        public static bool IsValidMode(int mode) => mode >= 1 && mode <= 9;

        public static async Task EnsureInstalled()
        {
            const string LOG_IDENT = "DpiBypassManager::EnsureInstalled";

            if (IsInstalled)
                return;

            App.Logger.WriteLine(LOG_IDENT, $"Downloading GoodbyeDPI {Version}");

            using var response = await App.HttpClient.GetAsync(DownloadUrl, HttpCompletionOption.ResponseHeadersRead);
            response.EnsureSuccessStatusCode();

            if (response.Content.Headers.ContentLength > MaxDownloadBytes)
                throw new InvalidDataException("The GoodbyeDPI download is larger than expected.");

            byte[] archive = await response.Content.ReadAsByteArrayAsync();

            if (archive.LongLength > MaxDownloadBytes)
                throw new InvalidDataException("The GoodbyeDPI download is larger than expected.");

            string hash = Convert.ToHexString(SHA256.HashData(archive)).ToLowerInvariant();

            if (hash != DownloadSha256)
            {
                App.Logger.WriteLine(LOG_IDENT, $"Hash mismatch - got {hash}, expected {DownloadSha256}");
                throw new InvalidDataException("The GoodbyeDPI download did not match its expected hash and was discarded.");
            }

            // extract into a staging folder first so a half-written install can never be mistaken
            // for a complete one by IsInstalled
            string staging = InstallDirectory + ".tmp";

            if (Directory.Exists(staging))
                Directory.Delete(staging, true);

            Directory.CreateDirectory(staging);

            using (var stream = new MemoryStream(archive))
            using (var zip = new ZipArchive(stream, ZipArchiveMode.Read))
                zip.ExtractToDirectory(staging);

            if (Directory.Exists(InstallDirectory))
                Directory.Delete(InstallDirectory, true);

            Directory.CreateDirectory(Path.GetDirectoryName(InstallDirectory)!);
            Directory.Move(staging, InstallDirectory);

            App.Logger.WriteLine(LOG_IDENT, $"Installed to {InstallDirectory}");

            if (!IsInstalled)
                throw new FileNotFoundException($"GoodbyeDPI was extracted but {ExecutablePath} is missing.");
        }

        /// <summary>
        /// Starts GoodbyeDPI if it isn't already up. Returns false when the user declined the UAC
        /// prompt, which is a normal outcome rather than a failure worth shouting about.
        /// </summary>
        public static bool Start(int mode)
        {
            const string LOG_IDENT = "DpiBypassManager::Start";

            if (IsRunning)
            {
                App.Logger.WriteLine(LOG_IDENT, "Already running, leaving it alone");
                return true;
            }

            if (!IsValidMode(mode))
                mode = 6;

            var startInfo = new ProcessStartInfo
            {
                FileName = ExecutablePath,
                Arguments = $"-{mode} {DnsRedirectArguments}",
                // WinDivert.dll and the .sys driver sit next to the executable and are resolved
                // relative to the working directory, exactly as the shipped .cmd files do it
                WorkingDirectory = Path.GetDirectoryName(ExecutablePath)!,
                UseShellExecute = true,
                Verb = "runas",
                WindowStyle = ProcessWindowStyle.Hidden
            };

            try
            {
                Process.Start(startInfo);
                App.Logger.WriteLine(LOG_IDENT, $"Started GoodbyeDPI in mode {mode}");
                return true;
            }
            catch (Win32Exception ex) when (ex.NativeErrorCode == 1223)
            {
                App.Logger.WriteLine(LOG_IDENT, "UAC prompt was declined");
                return false;
            }
        }

        /// <summary>
        /// GoodbyeDPI runs elevated, so an unelevated RailStrap can't terminate it directly - this
        /// shells out to an elevated taskkill, which means another UAC prompt.
        /// </summary>
        public static bool Stop()
        {
            const string LOG_IDENT = "DpiBypassManager::Stop";

            if (!IsRunning)
                return true;

            var startInfo = new ProcessStartInfo
            {
                FileName = "taskkill.exe",
                Arguments = $"/F /IM {ProcessName}.exe",
                UseShellExecute = true,
                Verb = "runas",
                WindowStyle = ProcessWindowStyle.Hidden
            };

            try
            {
                using var process = Process.Start(startInfo);
                process?.WaitForExit(10000);

                App.Logger.WriteLine(LOG_IDENT, "Stopped GoodbyeDPI");
                return true;
            }
            catch (Win32Exception ex) when (ex.NativeErrorCode == 1223)
            {
                App.Logger.WriteLine(LOG_IDENT, "UAC prompt was declined");
                return false;
            }
            catch (Exception ex)
            {
                App.Logger.WriteException(LOG_IDENT, ex);
                return false;
            }
        }
    }
}
