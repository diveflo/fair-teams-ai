using Xunit;

namespace fairTeams.Core.Tests
{
    public class MatchStatisticsTests
    {
        [Fact]
        public void Equals_SameObject_ReturnsTrue()
        {
            var first = new MatchStatistics { Kills = 5, Deaths = 5, Rounds = 10, OneKill = 1, TwoKill = 2 };

            Assert.Equal(first, first);
        }

        [Fact]
        public void Equals_DifferentObjectsSameId_ReturnsTrue()
        {
            var first = new MatchStatistics { Id = "ibims1id" };
            var second = new MatchStatistics { Id = "ibims1id" };

            Assert.Equal(first, second);
        }

        [Fact]
        public void HLTVScore_RealStatsFromPastGame_CorrectScore()
        {
            var expectedHLTVScore = 1.831;
            var playerStatistics = new MatchStatistics { Kills = 36, Deaths = 17, Rounds = 28, OneKill = 11, TwoKill = 6, ThreeKill = 3, FourKill = 1 };

            var hltvScore = playerStatistics.HLTVScore;

            Assert.Equal(expectedHLTVScore, hltvScore);
        }
    }
}
