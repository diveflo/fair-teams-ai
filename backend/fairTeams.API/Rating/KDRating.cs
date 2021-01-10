using fairTeams.Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;

namespace fairTeams.API.Rating
{
    public class KDRating : IRating
    {
        public string Name => "KD";
        public double Score { get; }

        public Trend Trend { get; }

        public KDRating(IEnumerable<Statistic> playerStatistics)
        {
            CheckRequiredStatistics(playerStatistics);

            var kills = playerStatistics.Single(x => x.Name == "total_kills").Value;
            var deaths = playerStatistics.Single(x => x.Name == "total_deaths").Value;
            Score = (double)kills / deaths;

            var lastMatchKills = playerStatistics.Single(x => x.Name == "last_match_kills").Value;
            var lastMatchDeaths = playerStatistics.Single(x => x.Name == "last_match_deaths").Value;
            var lastMatchKD = (double)lastMatchKills / lastMatchDeaths;

            Trend = TrendHelper.GetTrend(Score, lastMatchKD);
        }

        private static void CheckRequiredStatistics(IEnumerable<Statistic> playerStatistics)
        {
            var requiredStatistics = new List<string> { "total_kills", "last_match_kills", "total_deaths", "last_match_deaths" };

            foreach (var requiredStatistic in requiredStatistics)
            {
                if (!playerStatistics.Any(x => x.Name.Equals(requiredStatistic)))
                {
                    throw new ArgumentException($"The player statistics didn't contain the required statistic {requiredStatistic}");
                }
            }
        }
    }
}