using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using Newtonsoft.Json;

namespace backend.SteamworksApi
{
    public static class SteamworksApi
    {
        private const string steamAPIKey = "B0E3E0ED2572C01223E0ED7043E9678C";

        public static IList<Player> ParseSteamUsernames(IList<Player> players)
        {
            var commaDelimitedSteamIDs = string.Join(",", players.Select(x => x.SteamID));

            var webRequest = WebRequest.Create($"https://api.steampowered.com/ISteamUser/GetPlayerSummaries/v0002/?key={steamAPIKey}&steamids={commaDelimitedSteamIDs}");
            webRequest.ContentType = "application/json";

            using var responseStream = webRequest.GetResponse().GetResponseStream();
            using var responseStreamReader = new StreamReader(responseStream);

            var parsedResponse = JsonConvert.DeserializeObject<GetPlayerSummariesResponse>(responseStreamReader.ReadToEnd());
            var parsedPlayers = parsedResponse.PlayerSummaries.Players;

            foreach (var player in parsedPlayers)
            {
                var correspondingPlayer = players.SingleOrDefault(x => x.SteamID == player.SteamID);
                correspondingPlayer.SteamName = player.SteamName;
            }

            return players;
        }

        public static IList<Statistic> ParsePlayerStatistics(string steamID)
        {
            var webRequest = WebRequest.Create($"https://api.steampowered.com/ISteamUserStats/GetUserStatsForGame/v2/?appid=730&key={steamAPIKey}&steamid={steamID}");
            webRequest.ContentType = "application/json";

            try
            {
                using var responseStream = webRequest.GetResponse().GetResponseStream();
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