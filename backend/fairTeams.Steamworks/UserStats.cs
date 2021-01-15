using System.Text.Json.Serialization;

namespace fairTeams.Steamworks
{
    public class UserStatsResponse
    {
        public Playerstats Playerstats { get; set; }
    }

    public class Playerstats
    {
        public string SteamID { get; set; }
        public string GameName { get; set; }
        [JsonPropertyName("stats")]
        public Statistic[] Statistics { get; set; }
        public Achievement[] Achievements { get; set; }
    }

    public class Statistic
    {
        public string Name { get; set; }
        public int Value { get; set; }
    }

    public class Achievement
    {
        public string Name { get; set; }
        public int Achieved { get; set; }
    }
}