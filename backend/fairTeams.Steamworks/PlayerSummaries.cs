using Newtonsoft.Json;
using System.Collections.Generic;

namespace fairTeams.Steamworks
{
    internal class PlayerSummary
    {
        [JsonProperty("steamid")]
        internal string SteamID { get; set; }

        [JsonProperty("personaname")]
        internal string SteamName { get; set; }
    }

    internal class PlayerSummaries
    {
        [JsonProperty("players")]
        internal List<PlayerSummary> Players { get; set; }
    }

    internal class GetPlayerSummariesResponse
    {
        [JsonProperty("response")]
        internal PlayerSummaries PlayerSummaries { get; set; }
    }
}
