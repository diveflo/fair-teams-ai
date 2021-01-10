using fairTeams.API.Rating;
using fairTeams.Steamworks;
using System;
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
            var lastMatchKills = new Statistic { Name = "last_match_kills", Value = 0 };
            var deaths = new Statistic { Name = "total_deaths", Value = 10 };
            var lastMatchDeaths = new Statistic { Name = "last_match_deaths", Value = 1 };

            var kdRating = new KDRating(new List<Statistic>() { kills, lastMatchKills, deaths, lastMatchDeaths });

            Assert.Equal(1.0d, kdRating.Score);
        }

        [Fact]
        public void Score_TwiceAsManyKillsAsDeaths_ReturnsTwo()
        {
            var kills = new Statistic { Name = "total_kills", Value = 20 };
            var lastMatchKills = new Statistic { Name = "last_match_kills", Value = 0 };
            var deaths = new Statistic { Name = "total_deaths", Value = 10 };
            var lastMatchDeaths = new Statistic { Name = "last_match_deaths", Value = 1 };

            var kdRating = new KDRating(new List<Statistic>() { kills, lastMatchKills, deaths, lastMatchDeaths });

            Assert.Equal(2.0d, kdRating.Score);
        }

        [Fact]
        public void Score_MissingKillsStatistic_ThrowsArgumentException()
        {
            var deaths = new Statistic { Name = "total_deaths", Value = 10 };
            var statistics = new List<Statistic>() { deaths };

            Assert.Throws<ArgumentException>(() => new KDRating(statistics));
        }

        [Fact]
        public void Score_MissingLastMatchKillsStatistic_ThrowsArgumentException()
        {
            var kills = new Statistic { Name = "total_kills", Value = 10 };
            var deaths = new Statistic { Name = "total_deaths", Value = 10 };
            var lastMatchDeaths = new Statistic { Name = "last_match_deaths", Value = 10 };

            var statistics = new List<Statistic>() { kills, deaths, lastMatchDeaths };

            Assert.Throws<ArgumentException>(() => new KDRating(statistics));
        }

        [Fact]
        public void Score_MissingDeathsStatistic_ThrowsArgumentException()
        {
            var kills = new Statistic { Name = "total_kills", Value = 20 };
            var statistics = new List<Statistic>() { kills };

            Assert.Throws<ArgumentException>(() => new KDRating(statistics));
        }

        [Fact]
        public void Score_MissingLastMatchDeathsStatistic_ThrowsArgumentException()
        {
            var kills = new Statistic { Name = "total_kills", Value = 10 };
            var deaths = new Statistic { Name = "total_deaths", Value = 10 };
            var lastMatchKills = new Statistic { Name = "last_match_kills", Value = 10 };

            var statistics = new List<Statistic>() { kills, deaths, lastMatchKills };

            Assert.Throws<ArgumentException>(() => new KDRating(statistics));
        }

        [Fact]
        public void Score_ZeroDeaths_ReturnsInfinity()
        {
            var kills = new Statistic { Name = "total_kills", Value = 20 };
            var lastMatchKills = new Statistic { Name = "last_match_kills", Value = 0 };
            var deaths = new Statistic { Name = "total_deaths", Value = 0 };
            var lastMatchDeaths = new Statistic { Name = "last_match_deaths", Value = 1 };

            var kdRating = new KDRating(new List<Statistic>() { kills, lastMatchKills, deaths, lastMatchDeaths });

            Assert.Equal(double.PositiveInfinity, kdRating.Score);
        }
    }
}
