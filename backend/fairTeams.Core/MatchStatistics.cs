using System;

namespace fairTeams.Core
{
    public class MatchStatistics
    {
        public long SteamID { get; set; }
        public string Id { get; set; }
        public int Kills { get; set; }
        public int Deaths { get; set; }
        public int Rounds { get; set; }
        public double HLTVScore => CalculateHLTVScore();
        public int OneKill { get; set; }
        public int TwoKill { get; set; }
        public int ThreeKill { get; set; }
        public int FourKill { get; set; }
        public int FiveKill { get; set; }

        private double CalculateHLTVScore()
        {
            const double averageKillsPerRound = 0.679;
            const double averageSurvivedRoundsPerRound = 0.317;
            const double averageMultipleKillRounds = 1.277;

            var killRating = Kills / (double)Rounds / averageKillsPerRound;
            var survivalRating = (Rounds - Deaths) / (double)Rounds / averageSurvivedRoundsPerRound;
            var roundsWithMultipleKillsRating = (OneKill + 4 * TwoKill + 9 * ThreeKill + 16 * FourKill + 25 * FiveKill) / (double)Rounds / averageMultipleKillRounds;

            return Math.Round((killRating + 0.7 * survivalRating + roundsWithMultipleKillsRating) / 2.7, 3);
        }

        public bool Equals(MatchStatistics other)
        {
            return other.Id.Equals(Id);
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as MatchStatistics);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }
}
