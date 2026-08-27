namespace RailStrap.Models.APIs.Roblox
{
    public class AuthenticatedUserResponse
    {
        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; } = "";
    }

    public class FriendsListResponse
    {
        [JsonPropertyName("data")]
        public List<FriendEntry> Data { get; set; } = new();

        [JsonPropertyName("nextPageCursor")]
        public string? NextPageCursor { get; set; }
    }

    public class FriendEntry
    {
        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; } = "";
    }

    public class PresenceListResponse
    {
        [JsonPropertyName("userPresences")]
        public List<PresenceEntry> UserPresences { get; set; } = new();
    }

    public class PresenceEntry
    {
        [JsonPropertyName("userId")]
        public long UserId { get; set; }

        // 0 = offline, 1 = online, 2 = in game, 3 = in studio
        [JsonPropertyName("userPresenceType")]
        public int PresenceType { get; set; }

        [JsonPropertyName("lastLocation")]
        public string LastLocation { get; set; } = "";

        [JsonPropertyName("placeId")]
        public long? PlaceId { get; set; }
    }

    /// <summary>
    /// Combined view of a friend's name and current presence, for display in the friend activity panel
    /// </summary>
    public class FriendActivityEntry
    {
        public string Name { get; set; } = "";

        public string Status { get; set; } = "";

        public int PresenceType { get; set; }
    }
}
