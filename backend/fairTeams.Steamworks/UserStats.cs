using System.Text.Json.Serialization;

namespace fairTeams.Steamworks
{
    public class UserStatsResponse
    {
        [JsonPropertyName("playerstats")]
        public Playerstats Playerstats { get; set; }
    }

    public class Playerstats
    {
        [JsonPropertyName("steamID")]
        public string SteamID { get; set; }
        [JsonPropertyName("gameName")]
        public string GameName { get; set; }
        [JsonPropertyName("stats")]
        public Statistic[] Statistics { get; set; }
        [JsonPropertyName("achievements")]
        public Achievement[] Achievements { get; set; }
    }

    public class Statistic
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }
        [JsonPropertyName("Value")]
        public int Value { get; set; }
    }

    public class Achievement
    {
        public string name { get; set; }
        public int achieved { get; set; }
    }
}