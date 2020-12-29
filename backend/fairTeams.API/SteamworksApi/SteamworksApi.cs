using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace fairTeams.API.SteamworksApi
{
    public static class SteamworksApi
    {
        private const string steamAPIKey = "1E7C82407DBE9BD03F2343B063713894";

        public static async Task<IList<Player>> ParseSteamUsernames(IList<Player> players)
        {
            var commaDelimitedSteamIDs = string.Join(",", players.Select(x => x.SteamID));

            var webRequest = WebRequest.Create($"https://api.steampowered.com/ISteamUser/GetPlayerSummaries/v0002/?key={steamAPIKey}&steamids={commaDelimitedSteamIDs}");
            webRequest.ContentType = "application/json";

            using var response = await webRequest.GetResponseAsync();
            using var responseStream = response.GetResponseStream();
            using var responseStreamReader = new StreamReader(responseStream);

            var parsedResponse = JsonConvert.DeserializeObject<GetPlayerSummariesResponse>(responseStreamReader.ReadToEnd());
            var parsedPlayers = parsedResponse.PlayerSummaries.Players;

            foreach (var player in parsedPlayers)
            {
                var correspondingPlayer = players.SingleOrDefault(x => x.SteamID == player.SteamID);
                correspondingPlayer.SteamName = player.SteamName;
            }

            if (parsedPlayers.Count != players.Count)
            {
                var notFoundPlayers = players.Where(x => !parsedPlayers.Select(y => y.SteamID).Contains(x.SteamID));
                throw new PlayerNotFoundException($"Did not find player(s) with steam id(s): {string.Join(", ", notFoundPlayers.Select(x => x.SteamID))}");
            }

            return players;
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
