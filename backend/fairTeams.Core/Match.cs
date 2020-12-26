using System;
using System.Collections.Generic;

namespace fairTeams.Core
{
    public class MatchPlayer
    {
        public long SteamID { get; set; }
        public string Name { get; set; }
    }

    public class Match
    {
        public Demo Demo { get; set; }
        public IDictionary<MatchPlayer, MatchStatistics> PlayerResults { get; set; }
        public DateTime Date { get; set; }
        public string Map { get; set; }

        public int TScore { get; set; }

        public int CTScore { get; set; }

        public int Rounds { get; set; }
    }
}
