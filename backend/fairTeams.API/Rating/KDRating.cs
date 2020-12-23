using fairTeams.API.SteamworksApi;
using System;
using System.Collections.Generic;
using System.Linq;

namespace fairTeams.API.Rating
{
    public class KDRating : IRating
    {
        public string Name => "KD";
        public double Score { get; }

        public KDRating(IEnumerable<Statistic> playerStatistics)
        {
            if (!playerStatistics.Any(x => x.Name == "total_kills") || !playerStatistics.Any(x => x.Name == "total_deaths"))
            {
                throw new ArgumentException("The player statistics have to contain 'total_kills' and 'total_deaths' to compute the KD rating.");
            }

            var kills = playerStatistics.Single(x => x.Name == "total_kills").Value;
            var deaths = playerStatistics.Single(x => x.Name == "total_deaths").Value;

            Score = (double)kills / deaths;
        }
    }
}