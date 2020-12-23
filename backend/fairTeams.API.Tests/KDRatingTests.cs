using fairTeams.API.Rating;
using fairTeams.API.SteamworksApi;
using System.Collections.Generic;
using Xunit;

namespace fairTeams.API.Tests
{
    public class KDRatingTests
    {
        [Fact]
        public void Score_EqualNumberOfKillsAndDeaths_ReturnsOne()
        {
            var kills = new Statistic { Name = "total_kills", Value = 10 };
            var deathts = new Statistic { Name = "total_deaths", Value = 10 };

            var kdRating = new KDRating(new List<Statistic>() { kills, deathts });

            Assert.Equal(1.0d, kdRating.Score);
        }

        [Fact]
        public void Score_TwiceAsManyKillsAsDeaths_ReturnsTwo()
        {
            var kills = new Statistic { Name = "total_kills", Value = 20 };
            var deathts = new Statistic { Name = "total_deaths", Value = 10 };

            var kdRating = new KDRating(new List<Statistic>() { kills, deathts });

            Assert.Equal(2.0d, kdRating.Score);
        }
    }
}
