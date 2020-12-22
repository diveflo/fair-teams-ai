using Newtonsoft.Json;
using System.Collections.Generic;

namespace backend.SteamworksApi
{
    public class Statistic
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("value")]
        public int Value { get; set; }
    }

    internal class Statistics
    {
        [JsonProperty("stats")]
        internal List<Statistic> Stats { get; set; }
    }

    internal class PlayerStatistics
    {
        [JsonProperty("playerstats")]
        internal Statistics Statistics { get; set; }
    }
}