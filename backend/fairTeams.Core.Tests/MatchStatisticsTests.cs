using System;
using Xunit;

namespace fairTeams.Core.Tests
{
    public class MatchStatisticsTests
    {
        [Fact]
        public void Equals_SameObject_ReturnsTrue()
        {
            var first = new MatchStatistics { Kills = 5, Deaths = 5, Rounds = 10, MultipleKills = new MultipleKills { OneKill = 1, TwoKill = 2 } };

            Assert.Equal(first, first);
        }

        [Fact]
        public void Equals_DifferentObjectsSameValues_ReturnsTrue()
        {
            var first = new MatchStatistics { Kills = 5, Deaths = 5, Rounds = 10, MultipleKills = new MultipleKills { OneKill = 1, TwoKill = 2 } };
            var second = new MatchStatistics { Kills = 5, Deaths = 5, Rounds = 10, MultipleKills = new MultipleKills { OneKill = 1, TwoKill = 2 } };

            Assert.Equal(first, second);
        }
    }
}
