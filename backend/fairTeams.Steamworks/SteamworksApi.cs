using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace fairTeams.Steamworks
{
    public static class SteamworksApi
    {
        private const string steamAPIKey = "B0E3E0ED2572C01223E0ED7043E9678C";

        public static async Task<IDictionary<string, string>> ParseSteamUsernames(IList<string> steamIDs)
        {
            var commaDelimitedSteamIDs = string.Join(",", steamIDs);

            var webRequest = WebRequest.Create($"https://api.steampowered.com/ISteamUser/GetPlayerSummaries/v0002/?key={steamAPIKey}&steamids={commaDelimitedSteamIDs}");
            webRequest.ContentType = "application/json";

            using var response = await webRequest.GetResponseAsync();
            using var responseStream = response.GetResponseStream();
            using var responseStreamReader = new StreamReader(responseStream);

            var parsedResponse = JsonConvert.DeserializeObject<GetPlayerSummariesResponse>(responseStreamReader.ReadToEnd());
            var parsedPlayers = parsedResponse.PlayerSummaries.Players;

            var steamIDsWithUsernames = new Dictionary<string, string>();

            foreach (var player in parsedPlayers)
            {
                steamIDsWithUsernames.Add(player.SteamID, player.SteamName);
            }

            return steamIDsWithUsernames;
        }

        public static async Task<IList<Statistic>> ParsePlayerStatistics(string steamID)
        {
            var webRequest = WebRequest.Create($"https://api.steampowered.com/ISteamUserStats/GetUserStatsForGame/v2/?appid=730&key={steamAPIKey}&steamid={steamID}");
            webRequest.ContentType = "application/json";

            try
            {
                using var response = await webRequest.GetResponseAsync();
                using var responseStream = response.GetResponseStream();
                using var responseStreamReader = new StreamReader(responseStream);

                return JsonConvert.DeserializeObject<PlayerStatistics>(responseStreamReader.ReadToEnd()).Statistics.Stats;
            }
            catch (WebException)
            {
                throw new ProfileNotPublicException();
            }
        }
    }
}