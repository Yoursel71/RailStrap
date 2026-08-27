using System.Net.Http.Json;

using RailStrap.Models.APIs.Roblox;
using RailStrap.Utility;

namespace RailStrap.Integrations
{
    /// <summary>
    /// Fetches friends' current online/in-game status using the user's own Roblox session cookie,
    /// which they provide explicitly and opt-in via Settings. The cookie never leaves this process -
    /// it's only ever attached as a request header to roblox.com API calls.
    /// </summary>
    static class FriendActivityService
    {
        private const string LOG_IDENT = "FriendActivityService";

        private static void AttachCookie(HttpRequestMessage request, string cookie) =>
            request.Headers.TryAddWithoutValidation("Cookie", $".ROBLOSECURITY={cookie}");

        private static async Task<HttpResponseMessage> SendAuthenticated(HttpMethod method, string url, string cookie, object? body = null)
        {
            string? csrfToken = null;

            for (int attempt = 0; attempt < 2; attempt++)
            {
                var request = new HttpRequestMessage(method, url);
                AttachCookie(request, cookie);

                if (csrfToken is not null)
                    request.Headers.TryAddWithoutValidation("X-CSRF-TOKEN", csrfToken);

                if (body is not null)
                    request.Content = JsonContent.Create(body);

                var response = await App.HttpClient.SendAsync(request);

                if (response.StatusCode == System.Net.HttpStatusCode.Forbidden && response.Headers.TryGetValues("x-csrf-token", out var values))
                {
                    csrfToken = values.FirstOrDefault();
                    continue;
                }

                return response;
            }

            throw new InvalidOperationException("Failed to obtain a valid CSRF token from Roblox");
        }

        public static async Task<List<FriendActivityEntry>> GetFriendActivity(string encryptedCookie)
        {
            string cookie = SecureStorage.Unprotect(encryptedCookie);

            if (string.IsNullOrEmpty(cookie))
                throw new InvalidOperationException("No cookie configured");

            var userResponse = await SendAuthenticated(HttpMethod.Get, "https://users.roblox.com/v1/users/authenticated", cookie);
            userResponse.EnsureSuccessStatusCode();
            var user = await userResponse.Content.ReadFromJsonAsync<AuthenticatedUserResponse>();

            if (user is null)
                throw new InvalidOperationException("Could not resolve the authenticated user");

            var friendsResponse = await SendAuthenticated(HttpMethod.Get, $"https://friends.roblox.com/v1/users/{user.Id}/friends", cookie);
            friendsResponse.EnsureSuccessStatusCode();
            var friends = await friendsResponse.Content.ReadFromJsonAsync<FriendsListResponse>() ?? new();

            if (friends.Data.Count == 0)
                return new List<FriendActivityEntry>();

            var presenceResponse = await SendAuthenticated(
                HttpMethod.Post,
                "https://presence.roblox.com/v1/presence/users",
                cookie,
                new { userIds = friends.Data.Select(x => x.Id).ToArray() }
            );
            presenceResponse.EnsureSuccessStatusCode();
            var presences = await presenceResponse.Content.ReadFromJsonAsync<PresenceListResponse>() ?? new();

            var result = new List<FriendActivityEntry>();

            foreach (var friend in friends.Data)
            {
                var presence = presences.UserPresences.FirstOrDefault(x => x.UserId == friend.Id);

                string status = presence?.PresenceType switch
                {
                    2 => string.IsNullOrEmpty(presence.LastLocation) ? Strings.Menu_FriendActivity_InGame : presence.LastLocation,
                    3 => Strings.Menu_FriendActivity_InStudio,
                    1 => Strings.Menu_FriendActivity_Online,
                    _ => Strings.Menu_FriendActivity_Offline
                };

                result.Add(new FriendActivityEntry { Name = friend.Name, Status = status });
            }

            return result
                .OrderByDescending(x => x.Status != Strings.Menu_FriendActivity_Offline)
                .ToList();
        }
    }
}
