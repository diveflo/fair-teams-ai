using fairTeams.Core;
using System.Linq;

namespace fairTeams.API.Rating
{
    public class HLTVRating : IRating
    {
        public string Name => "HLTV";
        public double Score { get; }

        public HLTVRating(long steamID, MatchRepository matchRepository)
        {
            var allMatchStatisticsForPlayer = matchRepository.GetAllMatchStatisticsForSteamId(steamID);
            Score = allMatchStatisticsForPlayer.Average(z => z.HLTVScore);
        }
    }
}