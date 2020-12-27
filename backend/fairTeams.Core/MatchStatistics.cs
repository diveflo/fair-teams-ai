using System;

namespace fairTeams.Core
{
    public class MultipleKills : IEquatable<MultipleKills>
    {
        public int OneKill { get; set; }
        public int TwoKill { get; set; }
        public int ThreeKill { get; set; }
        public int FourKill { get; set; }
        public int FiveKill { get; set; }

        public bool Equals(MultipleKills other)
        {
            return OneKill == other.OneKill && TwoKill == other.TwoKill && ThreeKill == other.ThreeKill && FourKill == other.FourKill && FiveKill == other.FiveKill;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as MultipleKills);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(OneKill, TwoKill, ThreeKill, FourKill, FiveKill);
        }
    }

    public class MatchStatistics : IEquatable<MatchStatistics>
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

        public bool Equals(MatchStatistics other)
        {
            return Kills == other.Kills && Deaths == other.Deaths && Rounds == other.Rounds && MultipleKills.Equals(other.MultipleKills);
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as MatchStatistics);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Kills, Deaths, Rounds, HLTVScore);
        }
    }
}
