using System;

namespace fairTeams.Core
{
    public class MultipleKills
    {
        public int OneKill { get; set; }
        public int TwoKill { get; set; }
        public int ThreeKill { get; set; }
        public int FourKill { get; set; }
        public int FiveKill { get; set; }
    }

    public class MatchStatistics
    {
        public int Kills { get; set; }
        public int Deaths { get; set; }
        public int Rounds { get; set; }
        public double HLTVScore => CalculateHLTVScore();
        public MultipleKills MultipleKills { get; set; }

        public MatchStatistics()
        {
            MultipleKills = new MultipleKills();
        }

        private double CalculateHLTVScore()
        {
            const double averageKillsPerRound = 0.679;
            const double averageSurvivedRoundsPerRound = 0.317;
            const double averageMultipleKillRounds = 1.277;

            var killRating = Kills / (double)Rounds / averageKillsPerRound;
            var survivalRating = (Rounds - Deaths) / (double)Rounds / averageSurvivedRoundsPerRound;
            var roundsWithMultipleKillsRating = (MultipleKills.OneKill + 4 * MultipleKills.TwoKill + 9 * MultipleKills.ThreeKill + 16 * MultipleKills.FourKill + 25 * MultipleKills.FiveKill) / (double)Rounds / averageMultipleKillRounds;

            return Math.Round((killRating + 0.7 * survivalRating + roundsWithMultipleKillsRating) / 2.7, 3);
        }
    }
}
