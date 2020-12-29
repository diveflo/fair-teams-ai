using fairTeams.Core;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;

namespace fairTeams.Steamworks
{
    public static class SteamworksApi
    {
        public static async Task<IDictionary<string, string>> ParseSteamUsernames(IList<string> steamIDs)
        {
            var commaDelimitedSteamIDs = string.Join(",", steamIDs);

            var webRequest = WebRequest.Create($"https://api.steampowered.com/ISteamUser/GetPlayerSummaries/v0002/?key={Settings.SteamWebAPIKey}&steamids={commaDelimitedSteamIDs}");
            webRequest.ContentType = "application/json";

            using var response = await webRequest.GetResponseAsync();
            using var responseStream = response.GetResponseStream();
            using var responseStreamReader = new StreamReader(responseStream);

            var parsedResponse = JsonSerializer.Deserialize<PlayerSummariesResponse>(responseStreamReader.ReadToEnd(), new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            var parsedPlayers = parsedResponse.Response.Players;

            var steamIDsWithUsernames = new Dictionary<string, string>();

            foreach (var player in parsedPlayers)
            {
                steamIDsWithUsernames.Add(player.SteamID, player.PersonaName);
            }

            return steamIDsWithUsernames;
        }

        public static async Task<IList<Statistic>> ParsePlayerStatistics(string steamID)
        {
            var webRequest = WebRequest.Create($"https://api.steampowered.com/ISteamUserStats/GetUserStatsForGame/v2/?appid=730&key={Settings.SteamWebAPIKey}&steamid={steamID}");
            webRequest.ContentType = "application/json";

            try
            {
                using var response = await webRequest.GetResponseAsync();
                using var responseStream = response.GetResponseStream();
                using var responseStreamReader = new StreamReader(responseStream);

                var content = responseStreamReader.ReadToEnd();

                return JsonSerializer.Deserialize<UserStatsResponse>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }).Playerstats.Statistics;
            }
            catch (WebException)
            {
                throw new ProfileNotPublicException();
            }
        }

        public static async Task<string> GetNextMatchSharingCode(string steamID, string authenticationCodeForUser, string previousSharingCodeForUser)
        {
            var webRequest = WebRequest.Create($"https://api.steampowered.com/ICSGOPlayers_730/GetNextMatchSharingCode/v1?key={Settings.SteamWebAPIKey}&steamid={steamID}&steamidkey={authenticationCodeForUser}&knowncode={previousSharingCodeForUser}");
            webRequest.ContentType = "application/json";

            using var response = await webRequest.GetResponseAsync();
            using var responseStream = response.GetResponseStream();
            using var responseStreamReader = new StreamReader(responseStream);

            var content = responseStreamReader.ReadToEnd();

            return JsonSerializer.Deserialize<NextMatchSharingCode>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }).Result.NextCode;
        }
    }
}