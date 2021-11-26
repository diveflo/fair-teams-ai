using fairTeams.Core;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace fairTeams.Steamworks
{
    public class SteamworksApi
    {
        public virtual async Task<IDictionary<string, string>> ParseSteamUsernames(IList<string> steamIDs)
        {
            var commaDelimitedSteamIDs = string.Join(",", steamIDs);

            var httpClient = new HttpClient();
            var response = await httpClient.GetStreamAsync($"https://api.steampowered.com/ISteamUser/GetPlayerSummaries/v2/?key={Settings.SteamWebAPIKey}&steamids={commaDelimitedSteamIDs}");

            using var responseStreamReader = new StreamReader(response);

            var parsedResponse = JsonSerializer.Deserialize<PlayerSummariesResponse>(responseStreamReader.ReadToEnd(), new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            var parsedPlayers = parsedResponse.Response.Players;

            var steamIDsWithUsernames = new Dictionary<string, string>();

            foreach (var player in parsedPlayers)
            {
                steamIDsWithUsernames.Add(player.SteamID, player.PersonaName);
            }

            return steamIDsWithUsernames;
        }

        public virtual async Task<IList<Statistic>> ParsePlayerStatistics(string steamID)
        {
            var httpClient = new HttpClient();

            try
            {
                var response = await httpClient.GetStreamAsync($"https://api.steampowered.com/ISteamUserStats/GetUserStatsForGame/v2/?appid=730&key={Settings.SteamWebAPIKey}&steamid={steamID}");
                using var responseStreamReader = new StreamReader(response);

                var content = responseStreamReader.ReadToEnd();

                return JsonSerializer.Deserialize<UserStatsResponse>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }).Playerstats.Statistics;
            }
            catch (HttpRequestException)
            {
                throw new ProfileNotPublicException();
            }
        }

        public virtual async Task<string> GetNextMatchSharingCode(string steamID, string authenticationCodeForUser, string previousSharingCodeForUser)
        {
            var httpClient = new HttpClient();

            try
            {
                var response = await httpClient.GetStreamAsync($"https://api.steampowered.com/ICSGOPlayers_730/GetNextMatchSharingCode/v1?key={Settings.SteamWebAPIKey}&steamid={steamID}&steamidkey={authenticationCodeForUser}&knowncode={previousSharingCodeForUser}");
                using var responseStreamReader = new StreamReader(response);

                var content = responseStreamReader.ReadToEnd();

                return JsonSerializer.Deserialize<NextMatchSharingCode>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }).Result.NextCode;
            }
            catch (HttpRequestException)
            {
                throw new SteamIdAuthCodeShareCodeMismatchException(
                    $"Steam API request returned forbidden for steam id: {steamID}, auth code: {authenticationCodeForUser}, previous share code: {previousSharingCodeForUser}");
            }
        }
    }
}