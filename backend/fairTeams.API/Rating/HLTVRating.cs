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
            Score = matchRepository.Matches.SelectMany(x => x.PlayerResults).Where(y => y.SteamID == steamID).ToList().Average(z => z.HLTVScore);
        }
    }
}