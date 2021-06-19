using fairTeams.API.Rating;
using fairTeams.Core;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using Xunit;

using Match = fairTeams.Core.Match;

namespace fairTeams.API.Tests
{
    public sealed class HLTVRatingTests : IDisposable
    {
        private readonly MatchRepository myMatchRepository;
        private readonly List<MatchStatistics> myRealUweStatistics = new()
        {
            new MatchStatistics { SteamID = 76561198053826525, Kills = 25, Deaths = 21, Rounds = 29, OneKill = 12, TwoKill = 5, ThreeKill = 1, FourKill = 0, FiveKill = 0, Id = Guid.NewGuid().ToString() },
            new MatchStatistics { SteamID = 76561198053826525, Kills = 12, Deaths = 21, Rounds = 23, OneKill = 8, TwoKill = 2, ThreeKill = 0, FourKill = 0, FiveKill = 0, Id = Guid.NewGuid().ToString() },
            new MatchStatistics { SteamID = 76561198053826525, Kills = 14, Deaths = 26, Rounds = 30, OneKill = 6, TwoKill = 4, ThreeKill = 0, FourKill = 0, FiveKill = 0, Id = Guid.NewGuid().ToString() },
            new MatchStatistics { SteamID = 76561198053826525, Kills = 13, Deaths = 22, Rounds = 27, OneKill = 13, TwoKill = 0, ThreeKill = 0, FourKill = 0, FiveKill = 0, Id = Guid.NewGuid().ToString() },
            new MatchStatistics { SteamID = 76561198053826525, Kills = 11, Deaths = 22, Rounds = 24, OneKill = 7, TwoKill = 2, ThreeKill = 0, FourKill = 0, FiveKill = 0, Id = Guid.NewGuid().ToString() },
            new MatchStatistics { SteamID = 76561198053826525, Kills = 15, Deaths = 22, Rounds = 30, OneKill = 5, TwoKill = 2, ThreeKill = 2, FourKill = 0, FiveKill = 0, Id = Guid.NewGuid().ToString() },
            new MatchStatistics { SteamID = 76561198053826525, Kills = 11, Deaths = 24, Rounds = 27, OneKill = 9, TwoKill = 1, ThreeKill = 0, FourKill = 0, FiveKill = 0, Id = Guid.NewGuid().ToString() },
            new MatchStatistics { SteamID = 76561198053826525, Kills = 14, Deaths = 24, Rounds = 30, OneKill = 10, TwoKill = 2, ThreeKill = 0, FourKill = 0, FiveKill = 0, Id = Guid.NewGuid().ToString() },
            new MatchStatistics { SteamID = 76561198053826525, Kills = 19, Deaths = 24, Rounds = 30, OneKill = 9, TwoKill = 2, ThreeKill = 2, FourKill = 0, FiveKill = 0, Id = Guid.NewGuid().ToString() },
            new MatchStatistics { SteamID = 76561198053826525, Kills = 8, Deaths = 17, Rounds = 20, OneKill = 4, TwoKill = 2, ThreeKill = 0, FourKill = 0, FiveKill = 0, Id = Guid.NewGuid().ToString() },
            new MatchStatistics { SteamID = 76561198053826525, Kills = 24, Deaths = 20, Rounds = 29, OneKill = 14, TwoKill = 5, ThreeKill = 0, FourKill = 0, FiveKill = 0, Id = Guid.NewGuid().ToString() },
            new MatchStatistics { SteamID = 76561198053826525, Kills = 21, Deaths = 20, Rounds = 26, OneKill = 9, TwoKill = 4, ThreeKill = 0, FourKill = 1, FiveKill = 0, Id = Guid.NewGuid().ToString() },
            new MatchStatistics { SteamID = 76561198053826525, Kills = 18, Deaths = 23, Rounds = 30, OneKill = 12, TwoKill = 3, ThreeKill = 0, FourKill = 0, FiveKill = 0, Id = Guid.NewGuid().ToString() },
            new MatchStatistics { SteamID = 76561198053826525, Kills = 22, Deaths = 25, Rounds = 29, OneKill = 10, TwoKill = 3, ThreeKill = 2, FourKill = 0, FiveKill = 0, Id = Guid.NewGuid().ToString() },
            new MatchStatistics { SteamID = 76561198053826525, Kills = 14, Deaths = 15, Rounds = 21, OneKill = 5, TwoKill = 3, ThreeKill = 1, FourKill = 0, FiveKill = 0, Id = Guid.NewGuid().ToString() },
            new MatchStatistics { SteamID = 76561198053826525, Kills = 19, Deaths = 21, Rounds = 27, OneKill = 6, TwoKill = 2, ThreeKill = 3, FourKill = 0, FiveKill = 0, Id = Guid.NewGuid().ToString() },
            new MatchStatistics { SteamID = 76561198053826525, Kills = 5, Deaths = 21, Rounds = 22, OneKill = 3, TwoKill = 1, ThreeKill = 0, FourKill = 0, FiveKill = 0, Id = Guid.NewGuid().ToString() },
            new MatchStatistics { SteamID = 76561198053826525, Kills = 16, Deaths = 24, Rounds = 30, OneKill = 10, TwoKill = 3, ThreeKill = 0, FourKill = 0, FiveKill = 0, Id = Guid.NewGuid().ToString() },
            new MatchStatistics { SteamID = 76561198053826525, Kills = 9, Deaths = 23, Rounds = 23, OneKill = 7, TwoKill = 1, ThreeKill = 0, FourKill = 0, FiveKill = 0, Id = Guid.NewGuid().ToString() },
            new MatchStatistics { SteamID = 76561198053826525, Kills = 16, Deaths = 24, Rounds = 28, OneKill = 10, TwoKill = 3, ThreeKill = 0, FourKill = 0, FiveKill = 0, Id = Guid.NewGuid().ToString() },
            new MatchStatistics { SteamID = 76561198053826525, Kills = 12, Deaths = 18, Rounds = 21, OneKill = 6, TwoKill = 3, ThreeKill = 0, FourKill = 0, FiveKill = 0, Id = Guid.NewGuid().ToString() },
            new MatchStatistics { SteamID = 76561198053826525, Kills = 16, Deaths = 19, Rounds = 29, OneKill = 9, TwoKill = 2, ThreeKill = 1, FourKill = 0, FiveKill = 0, Id = Guid.NewGuid().ToString() },
            new MatchStatistics { SteamID = 76561198053826525, Kills = 15, Deaths = 19, Rounds = 23, OneKill = 7, TwoKill = 1, ThreeKill = 2, FourKill = 0, FiveKill = 0, Id = Guid.NewGuid().ToString() },
            new MatchStatistics { SteamID = 76561198053826525, Kills = 14, Deaths = 25, Rounds = 30, OneKill = 8, TwoKill = 3, ThreeKill = 0, FourKill = 0, FiveKill = 0, Id = Guid.NewGuid().ToString() },
            new MatchStatistics { SteamID = 76561198053826525, Kills = 21, Deaths = 25, Rounds = 30, OneKill = 14, TwoKill = 2, ThreeKill = 1, FourKill = 0, FiveKill = 0, Id = Guid.NewGuid().ToString() },
            new MatchStatistics { SteamID = 76561198053826525, Kills = 19, Deaths = 22, Rounds = 30, OneKill = 9, TwoKill = 5, ThreeKill = 0, FourKill = 0, FiveKill = 0, Id = Guid.NewGuid().ToString() }
        };

        public HLTVRatingTests()
        {
            var options = new DbContextOptionsBuilder<MatchRepository>()
                .UseInMemoryDatabase(databaseName: "MatchRepository")
                .Options;

            myMatchRepository = new MatchRepository(options);
        }

        [Fact]
        [Trait("Category", "unit")]
        public void Constructor_RealWorldUweStatistics_CorrectHLTVScore()
        {
            var matches = CreateMultipleMatchesWithStatistics(myRealUweStatistics);
            myMatchRepository.AddRange(matches);
            myMatchRepository.SaveChanges();

            var hltvRating = new HLTVRating(76561198053826525, myMatchRepository);

            Assert.Equal(0.724, hltvRating.Score, 3);
        }

        [Fact]
        [Trait("Category", "unit")]
        public void Constructor_LastMatchHigherScore_UpwardsTrend()
        {
            var matches = CreateMultipleMatchesWithStatistics(myRealUweStatistics);

            for (var i = 0; i < matches.Count; i++)
            {
                matches[i].Date = new DateTime(i);
            }

            var highScoreStatistic = new MatchStatistics { SteamID = 76561198053826525, Id = Guid.NewGuid().ToString(), Kills = 200, Deaths = 1, Rounds = 30, OneKill = 100, TwoKill = 5, FiveKill = 20 };
            var fakeLatestMatchWithHighScore = new Match { Date = new DateTime(matches.Count + 1), Id = Guid.NewGuid().ToString() };
            fakeLatestMatchWithHighScore.PlayerResults.Add(highScoreStatistic);
            matches.Add(fakeLatestMatchWithHighScore);

            myMatchRepository.AddRange(matches);
            myMatchRepository.SaveChanges();

            var hltvRating = new HLTVRating(76561198053826525, myMatchRepository);

            Assert.Equal(Trend.Upwards, hltvRating.Trend);
        }

        [Fact]
        [Trait("Category", "unit")]
        public void Constructor_LastMatchLowerScore_DownwardTrend()
        {
            var matches = CreateMultipleMatchesWithStatistics(myRealUweStatistics);

            for (var i = 0; i < matches.Count; i++)
            {
                matches[i].Date = new DateTime(i);
            }

            var highScoreStatistic = new MatchStatistics { SteamID = 76561198053826525, Id = Guid.NewGuid().ToString(), Kills = 0, Deaths = 30, Rounds = 30 };
            var fakeLatestMatchWithLowScore = new Match { Date = new DateTime(matches.Count + 1), Id = Guid.NewGuid().ToString() };
            fakeLatestMatchWithLowScore.PlayerResults.Add(highScoreStatistic);
            matches.Add(fakeLatestMatchWithLowScore);

            myMatchRepository.AddRange(matches);
            myMatchRepository.SaveChanges();

            var hltvRating = new HLTVRating(76561198053826525, myMatchRepository);

            Assert.Equal(Trend.Downwards, hltvRating.Trend);
        }

        [Fact]
        [Trait("Category", "unit")]
        public void Constructor_LastMatchSimilarScore_PlateauTrend()
        {
            var statistics = new List<MatchStatistics>
            {
                new MatchStatistics {SteamID = 76561198053826525, Id = Guid.NewGuid().ToString(), Kills = 10, Deaths = 5, Rounds = 20 },
                new MatchStatistics {SteamID = 76561198053826525, Id = Guid.NewGuid().ToString(), Kills = 11, Deaths = 6, Rounds = 20 }
            };

            var matches = CreateMultipleMatchesWithStatistics(statistics);

            for (var i = 0; i < matches.Count; i++)
            {
                matches[i].Date = new DateTime(i);
            }

            myMatchRepository.AddRange(matches);
            myMatchRepository.SaveChanges();

            var hltvRating = new HLTVRating(76561198053826525, myMatchRepository);

            Assert.Equal(Trend.Plateau, hltvRating.Trend);
        }

        private static List<Match> CreateMultipleMatchesWithStatistics(List<MatchStatistics> statistics)
        {
            var matches = new List<Match>();
            foreach (var statistic in statistics)
            {
                matches.Add(CreateMatchWithStatistics(statistic));
            }

            return matches;
        }

        private static Match CreateMatchWithStatistics(MatchStatistics statistics)
        {
            var match = new Match
            {
                Id = Guid.NewGuid().ToString()
            };

            match.PlayerResults.Add(statistics);
            return match;
        }

        public void Dispose()
        {
            myMatchRepository.Database.EnsureDeleted();
            myMatchRepository.Dispose();
        }
    }
}
