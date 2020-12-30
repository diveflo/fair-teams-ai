using Microsoft.EntityFrameworkCore;
using System.IO;
using System.Linq;
using Xunit;

namespace fairTeams.Core.Tests
{
    public class MatchRepositoryTests
    {
        [Fact]
        public void MatchesAdd_RepositoryEmptyBeforehand_RepositoryHasOneMatch()
        {
            using var repository = CreateEmptyRepository();
            var match = new Match { Id = "Matches_AddNewMatch_IsAdded", Map = "de_dust2" };

            repository.Matches.Add(match);
            repository.SaveChanges();

            Assert.Single(repository.Matches);
        }

        [Theory]
        [InlineData(76561197973591119, 1.1974)]
        [InlineData(76561197984050254, 0.9542)]
        [InlineData(76561197995643389, 1.056)]
        [InlineData(76561198011775117, 1.0368)]
        public void Matches_ReadExisitingRepository_CorrectAverageHLTVScore(long steamID, double expectedHLTVScore)
        {
            using var repository = ReadTestRepository("TestData" + Path.DirectorySeparatorChar + "matchRepository_0.db");

            var actualAverageHLTVScore = repository.Matches.SelectMany(x => x.PlayerResults).Where(y => y.SteamID == steamID).ToList().Average(z => z.HLTVScore);

            Assert.Equal(expectedHLTVScore, actualAverageHLTVScore, 4);
        }

        private static MatchRepository CreateEmptyRepository()
        {
            var databasePath = Path.GetTempFileName().Replace(".tmp", ".db");
            var databaseOptions = new DbContextOptionsBuilder<MatchRepository>()
                .UseSqlite($"Data Source={databasePath}")
                .Options;

            var repository = new MatchRepository(databaseOptions);
            repository.Database.EnsureDeleted();
            repository.Database.EnsureCreated();

            return repository;
        }

        private static MatchRepository ReadTestRepository(string fileName)
        {
            var databaseOptions = new DbContextOptionsBuilder<MatchRepository>()
                .UseSqlite($"Data Source={fileName}")
                .Options;

            return new MatchRepository(databaseOptions);
        }
    }
}
