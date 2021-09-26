using System.Collections.Generic;
using System.Linq;

namespace fairTeams.Core
{
    public static class MatchStatisticsCalculator
    {
        public static double MatchTypeNormalizedHLTVScore(IEnumerable<MatchStatistics> matches)
        {
            return matches.Select(x => x.MatchType == CompetitiveMatchType.Short ? 0.5 * x.HLTVScore : x.HLTVScore).Average();
        }
    }
}
