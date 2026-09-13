using System.Collections.Concurrent;
using System.Net.NetworkInformation;

namespace RailStrap.Utility
{
    /// <summary>
    /// ICMP latency measurement against a Roblox game server.
    ///
    /// Roblox's datacenters drop ICMP echo requests outright and expose no listening TCP port, so
    /// on most connections this simply cannot produce a number - measured against live servers
    /// (128.116.21.33, 128.116.5.33) it is 100% loss, while unrelated hosts answer normally from
    /// the same machine. Some networks and some regions do get replies, so it is still worth
    /// attempting, but a null result is the expected case rather than an error.
    ///
    /// When this can't measure anything, the honest fallback is Roblox's own in-game performance
    /// stats overlay (Shift+F5), which RailStrap can switch on through
    /// <see cref="GlobalSettingsManager.ApplyPerformanceStats"/>. That reads the real client-side
    /// round trip instead of guessing at it from outside the process.
    /// </summary>
    static class ServerPing
    {
        private const int TimeoutMs = 2000;

        /// <summary>
        /// Servers that have already refused to answer. Retrying them every few seconds for the
        /// whole session just burns timeouts, so the first two failures settle it for that address.
        /// </summary>
        private static readonly ConcurrentDictionary<string, int> _failures = new();

        private const int FailureThreshold = 2;

        public static bool IsUnreachable(string address) =>
            !string.IsNullOrEmpty(address) && _failures.TryGetValue(address, out int count) && count >= FailureThreshold;

        public static async Task<long?> Measure(string address, bool addressValid)
        {
            const string LOG_IDENT = "ServerPing::Measure";

            if (!addressValid || IsUnreachable(address))
                return null;

            try
            {
                using var ping = new Ping();
                var reply = await ping.SendPingAsync(address, TimeoutMs);

                if (reply.Status == IPStatus.Success)
                {
                    _failures.TryRemove(address, out _);
                    return reply.RoundtripTime;
                }

                int failures = _failures.AddOrUpdate(address, 1, (_, count) => count + 1);

                if (failures == FailureThreshold)
                    App.Logger.WriteLine(LOG_IDENT, $"{address} does not answer ICMP ({reply.Status}), giving up on it for this session");
            }
            catch (Exception ex)
            {
                _failures.AddOrUpdate(address, 1, (_, count) => count + 1);

                App.Logger.WriteLine(LOG_IDENT, $"Failed to ping {address}");
                App.Logger.WriteException(LOG_IDENT, ex);
            }

            return null;
        }
    }
}
