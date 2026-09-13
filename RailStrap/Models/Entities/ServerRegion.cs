namespace RailStrap.Models.Entities
{
    /// <summary>
    /// One of Roblox's known datacenter locations.
    ///
    /// Roblox exposes no API for choosing where a server lives, so region preference works by
    /// matching the location ipinfo.io reports for the joined server's address against this
    /// catalogue, and rerolling into a fresh public server when it isn't one the user wants.
    ///
    /// Note there is deliberately no Turkey entry: Roblox has never run a datacenter there. The
    /// closest options to Turkey are Frankfurt, Warsaw, Amsterdam, London and Paris, which is why
    /// they lead the list.
    /// </summary>
    public class ServerRegion
    {
        public string Key { get; init; } = string.Empty;

        public string DisplayName { get; init; } = string.Empty;

        /// <summary>
        /// City names as ipinfo.io reports them. Matched case-insensitively as a substring, so
        /// "Frankfurt" also catches "Frankfurt am Main".
        /// </summary>
        public IReadOnlyList<string> Cities { get; init; } = Array.Empty<string>();

        /// <summary>
        /// Ordered roughly by distance from Turkey, since that's where this was first needed.
        /// </summary>
        public static readonly IReadOnlyList<ServerRegion> All = new List<ServerRegion>
        {
            new() { Key = "frankfurt", DisplayName = "Frankfurt, Germany",     Cities = new[] { "Frankfurt" } },
            new() { Key = "warsaw",    DisplayName = "Warsaw, Poland",         Cities = new[] { "Warsaw", "Warszawa" } },
            new() { Key = "amsterdam", DisplayName = "Amsterdam, Netherlands", Cities = new[] { "Amsterdam" } },
            new() { Key = "london",    DisplayName = "London, United Kingdom", Cities = new[] { "London" } },
            new() { Key = "paris",     DisplayName = "Paris, France",          Cities = new[] { "Paris" } },
            new() { Key = "mumbai",    DisplayName = "Mumbai, India",          Cities = new[] { "Mumbai" } },
            new() { Key = "singapore", DisplayName = "Singapore",              Cities = new[] { "Singapore" } },
            new() { Key = "tokyo",     DisplayName = "Tokyo, Japan",           Cities = new[] { "Tokyo" } },
            new() { Key = "sydney",    DisplayName = "Sydney, Australia",      Cities = new[] { "Sydney" } },
            new() { Key = "ashburn",   DisplayName = "Ashburn, US East",       Cities = new[] { "Ashburn" } },
            new() { Key = "newyork",   DisplayName = "New York, US East",      Cities = new[] { "New York", "Secaucus", "Newark" } },
            new() { Key = "miami",     DisplayName = "Miami, US East",         Cities = new[] { "Miami" } },
            new() { Key = "atlanta",   DisplayName = "Atlanta, US East",       Cities = new[] { "Atlanta" } },
            new() { Key = "chicago",   DisplayName = "Chicago, US Central",    Cities = new[] { "Chicago" } },
            new() { Key = "dallas",    DisplayName = "Dallas, US Central",     Cities = new[] { "Dallas" } },
            new() { Key = "losangeles",DisplayName = "Los Angeles, US West",   Cities = new[] { "Los Angeles" } },
            new() { Key = "sanjose",   DisplayName = "San Jose, US West",      Cities = new[] { "San Jose", "Santa Clara" } },
            new() { Key = "seattle",   DisplayName = "Seattle, US West",       Cities = new[] { "Seattle" } },
            new() { Key = "saopaulo",  DisplayName = "Sao Paulo, Brazil",      Cities = new[] { "Sao Paulo", "São Paulo" } },
            new() { Key = "santiago",  DisplayName = "Santiago, Chile",        Cities = new[] { "Santiago" } },
        };

        public static ServerRegion? FromKey(string key) =>
            All.FirstOrDefault(x => x.Key.Equals(key, StringComparison.OrdinalIgnoreCase));

        /// <summary>
        /// Resolves the region for a location string produced by
        /// <see cref="ActivityData.QueryServerLocation"/> ("City, Region, CC"). Returns null when
        /// the server sits somewhere this catalogue doesn't know about, which is treated as
        /// "no opinion" rather than "not preferred".
        /// </summary>
        public static ServerRegion? FromLocation(string? location)
        {
            if (string.IsNullOrWhiteSpace(location))
                return null;

            return All.FirstOrDefault(region =>
                region.Cities.Any(city => location.Contains(city, StringComparison.OrdinalIgnoreCase)));
        }
    }
}
