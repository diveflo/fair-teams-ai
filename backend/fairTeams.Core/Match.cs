using System;
using System.Collections.Generic;

namespace fairTeams.Core
{
    public class MatchPlayer : IEquatable<MatchPlayer>
    {
        public long SteamID { get; set; }
        public string Name { get; set; }

        public bool Equals(MatchPlayer other)
        {
            return SteamID == other.SteamID;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as MatchPlayer);
        }

        public override int GetHashCode()
        {
            return SteamID.GetHashCode();
        }
    }

    public class Match
    {
        public Demo Demo { get; set; }
        public IDictionary<MatchPlayer, MatchStatistics> PlayerResults { get; }
        public DateTime Date { get; set; }
        public string Map { get; set; }
        public int TScore { get; set; }
        public int CTScore { get; set; }
        public int Rounds { get; set; }

        public Match()
        {
            PlayerResults = new Dictionary<MatchPlayer, MatchStatistics>();
        }
    }
}
