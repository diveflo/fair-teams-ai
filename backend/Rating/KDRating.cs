using backend.SteamworksApi;
using System.Collections.Generic;
using System.Linq;

namespace backend.Rating
{
    public class KDRating : IRating
    {
        public string Name => "KD";
        public double Score { get; }

        public KDRating(IEnumerable<Statistic> playerStatistics)
        {
            var kills = playerStatistics.SingleOrDefault(x => x.Name == "total_kills").Value;
            var deaths = playerStatistics.SingleOrDefault(x => x.Name == "total_deaths").Value;

            Score = (double)kills / deaths;
        }
    }
}