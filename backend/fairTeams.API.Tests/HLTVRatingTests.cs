using fairTeams.API.Rating;
using fairTeams.Core;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using Xunit;

using Match = fairTeams.Core.Match;

namespace fairTeams.API.Tests
{
    public class HLTVRatingTests : IDisposable
    {
        private DbConnection myInMemoryDatabaseConnection;

        [Fact]
        public void Constructor_RealMatchStatistics_CorrectScore()
        {
            var statistics = new List<MatchStatistics>
            {
                new MatchStatistics { Id = "00", SteamID = 76561198053826525, Kills = 25, Deaths = 21, Rounds = 29, OneKill = 12, TwoKill = 5, ThreeKill = 1, FourKill = 0, FiveKill = 0 },
                new MatchStatistics { Id = "01", SteamID = 76561198053826525, Kills = 12, Deaths = 21, Rounds = 23, OneKill = 8, TwoKill = 2, ThreeKill = 0, FourKill = 0, FiveKill = 0 },
                new MatchStatistics { Id = "02", SteamID = 76561198053826525, Kills = 14, Deaths = 26, Rounds = 30, OneKill = 6, TwoKill = 4, ThreeKill = 0, FourKill = 0, FiveKill = 0 },
                new MatchStatistics { Id = "03", SteamID = 76561198053826525, Kills = 13, Deaths = 22, Rounds = 27, OneKill = 13, TwoKill = 0, ThreeKill = 0, FourKill = 0, FiveKill = 0 },
                new MatchStatistics { Id = "04", SteamID = 76561198053826525, Kills = 11, Deaths = 22, Rounds = 24, OneKill = 7, TwoKill = 2, ThreeKill = 0, FourKill = 0, FiveKill = 0 },
                new MatchStatistics { Id = "05", SteamID = 76561198053826525, Kills = 15, Deaths = 22, Rounds = 30, OneKill = 5, TwoKill = 2, ThreeKill = 2, FourKill = 0, FiveKill = 0 },
                new MatchStatistics { Id = "06", SteamID = 76561198053826525, Kills = 11, Deaths = 24, Rounds = 27, OneKill = 9, TwoKill = 1, ThreeKill = 0, FourKill = 0, FiveKill = 0 },
                new MatchStatistics { Id = "07", SteamID = 76561198053826525, Kills = 14, Deaths = 24, Rounds = 30, OneKill = 10, TwoKill = 2, ThreeKill = 0, FourKill = 0, FiveKill = 0 },
                new MatchStatistics { Id = "08", SteamID = 76561198053826525, Kills = 19, Deaths = 24, Rounds = 30, OneKill = 9, TwoKill = 2, ThreeKill = 2, FourKill = 0, FiveKill = 0 },
                new MatchStatistics { Id = "09", SteamID = 76561198053826525, Kills = 8, Deaths = 17, Rounds = 20, OneKill = 4, TwoKill = 2, ThreeKill = 0, FourKill = 0, FiveKill = 0 },
                new MatchStatistics { Id = "10", SteamID = 76561198053826525, Kills = 24, Deaths = 20, Rounds = 29, OneKill = 14, TwoKill = 5, ThreeKill = 0, FourKill = 0, FiveKill = 0 },
                new MatchStatistics { Id = "11", SteamID = 76561198053826525, Kills = 21, Deaths = 20, Rounds = 26, OneKill = 9, TwoKill = 4, ThreeKill = 0, FourKill = 1, FiveKill = 0 },
                new MatchStatistics { Id = "12", SteamID = 76561198053826525, Kills = 18, Deaths = 23, Rounds = 30, OneKill = 12, TwoKill = 3, ThreeKill = 0, FourKill = 0, FiveKill = 0 },
                new MatchStatistics { Id = "13", SteamID = 76561198053826525, Kills = 22, Deaths = 25, Rounds = 29, OneKill = 10, TwoKill = 3, ThreeKill = 2, FourKill = 0, FiveKill = 0 },
                new MatchStatistics { Id = "14", SteamID = 76561198053826525, Kills = 14, Deaths = 15, Rounds = 21, OneKill = 5, TwoKill = 3, ThreeKill = 1, FourKill = 0, FiveKill = 0 },
                new MatchStatistics { Id = "13", SteamID = 76561198053826525, Kills = 16, Deaths = 19, Rounds = 29, OneKill = 9, TwoKill = 2, ThreeKill = 1, FourKill = 0, FiveKill = 0 },
                new MatchStatistics { Id = "13", SteamID = 76561198053826525, Kills = 15, Deaths = 19, Rounds = 23, OneKill = 7, TwoKill = 1, ThreeKill = 2, FourKill = 0, FiveKill = 0 },
                new MatchStatistics { Id = "13", SteamID = 76561198053826525, Kills = 16, Deaths = 24, Rounds = 29, OneKill = 11, TwoKill = 3, ThreeKill = 0, FourKill = 0, FiveKill = 0 },
                new MatchStatistics { Id = "13", SteamID = 76561198053826525, Kills = 27, Deaths = 25, Rounds = 29, OneKill = 10, TwoKill = 3, ThreeKill = 2, FourKill = 0, FiveKill = 0 },
            };

            var matches = CreateMultipleMatchesWithStatistics(statistics).AsQueryable();

            var matchRepositorySetMock = new Mock<DbSet<Match>>();
            matchRepositorySetMock.As<IQueryable<Match>>().Setup(m => m.Provider).Returns(matches.Provider);
            matchRepositorySetMock.As<IQueryable<Match>>().Setup(m => m.Expression).Returns(matches.Expression);
            matchRepositorySetMock.As<IQueryable<Match>>().Setup(m => m.ElementType).Returns(matches.ElementType);
            matchRepositorySetMock.As<IQueryable<Match>>().Setup(m => m.GetEnumerator()).Returns(matches.GetEnumerator());

            var matchRepositoryMock = CreateMockMatchRepository();
            matchRepositoryMock.Setup(r => r.Matches).Returns(matchRepositorySetMock.Object);

            var hltvRating = new HLTVRating(76561198053826525, matchRepositoryMock.Object);

            Assert.Equal(0.674, hltvRating.Score);
        }

        private Mock<MatchRepository> CreateMockMatchRepository()
        {
            myInMemoryDatabaseConnection = CreateInMemoryDatabase();

            var contextOptions = new DbContextOptionsBuilder<MatchRepository>()
                .UseSqlite(myInMemoryDatabaseConnection)
                .Options;

            return new Mock<MatchRepository>(contextOptions, UnitTestLoggerCreator.CreateUnitTestLogger<MatchRepository>());
        }

        private static DbConnection CreateInMemoryDatabase()
        {
            var connection = new SqliteConnection("Filename=:memory:");
            connection.Open();
            return connection;
        }

        private static List<Match> CreateMultipleMatchesWithStatistics(List<MatchStatistics> statistics)
        {
            var matches = new List<Match>();
            foreach(var statistic in statistics)
            {
                matches.Add(CreateMatchWithStatistics(statistic));
            }

            return matches;
        }

        private static Match CreateMatchWithStatistics(MatchStatistics statistics)
        {
            var match = new Match
            {
                Id = new Guid().ToString()
            };
            match.PlayerResults.Add(statistics);
            return match;
        }

        public void Dispose()
        {
            myInMemoryDatabaseConnection.Close();
            myInMemoryDatabaseConnection.Dispose();
        }
    }
}
