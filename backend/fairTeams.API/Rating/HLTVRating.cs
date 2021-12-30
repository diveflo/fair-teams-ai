using fairTeams.Core;
using System.Linq;

namespace fairTeams.API.Rating
{
    public class HLTVRating : IRating
    {
        public string Name => "HLTV";
        public double Score { get; }

        public Trend Trend { get; }

        public HLTVRating(long steamID, MatchRepository matchRepository)
        {
            var allMatchStatisticsForPlayer = matchRepository.GetLastMatchStatisticsForSteamId(steamID, 50);

            Score = MatchStatisticsCalculator.MatchTypeNormalizedHLTVScore(allMatchStatisticsForPlayer);

            var latestMatchStatisticForPlayer = matchRepository.GetLatestMatchStatisticForSteamId(steamID);
            Trend = TrendHelper.GetTrend(Score, latestMatchStatisticForPlayer.HLTVScore);
        }
    }
}