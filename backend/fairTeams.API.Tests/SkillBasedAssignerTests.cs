using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using fairTeams.Core;
using fairTeams.Steamworks;
using Microsoft.EntityFrameworkCore;
using Xunit;

using Match = fairTeams.Core.Match;

namespace fairTeams.API.Tests
{
    public class SkillBasedAssignerTests : IDisposable
    {
        private readonly MatchRepository myMatchRepository;

        public SkillBasedAssignerTests()
        {
            var options = new DbContextOptionsBuilder<MatchRepository>()
                .UseInMemoryDatabase(databaseName: "MatchRepository")
                .Options;

            myMatchRepository = new MatchRepository(options);
        }

        [Fact]
        public async Task GetAssignedPlayers_TwoPlayers_OnePlayerAssignedToEachTeam()
        {
            var match = new Match { Id = "M1" };
            match.PlayerResults.Add(new MatchStatistics { SteamID = 1, Id = "1", Kills = 1, Deaths = 1 });
            match.PlayerResults.Add(new MatchStatistics { SteamID = 2, Id = "2", Kills = 2, Deaths = 2 });
            myMatchRepository.Add(match);
            myMatchRepository.SaveChanges();
            var skillBasedAssigner = new SkillBasedAssigner(myMatchRepository, new SteamworksApi());

            (var t, var ct) = await skillBasedAssigner.GetAssignedPlayers(new List<Player> { new Player { SteamID = "1" }, new Player { SteamID = "2" } });

            Assert.Single(t.Players);
            Assert.Single(ct.Players);
        }

        [Fact]
        public async Task GetAssignedPlayers_ThreePlayers_TwoPlayersVersusOnePlayerPlusBot()
        {
            var match = new Match { Id = "M1" };
            match.PlayerResults.Add(new MatchStatistics { SteamID = 1, Id = "1", Kills = 1, Deaths = 1 });
            match.PlayerResults.Add(new MatchStatistics { SteamID = 2, Id = "2", Kills = 2, Deaths = 2 });
            match.PlayerResults.Add(new MatchStatistics { SteamID = 3, Id = "3", Kills = 3, Deaths = 3 });
            myMatchRepository.Add(match);
            myMatchRepository.SaveChanges();
            var skillBasedAssigner = new SkillBasedAssigner(myMatchRepository, new SteamworksApi());

            (var t, var ct) = await skillBasedAssigner.GetAssignedPlayers(new List<Player> { 
                new Player { SteamID = "1", Name = "Emma"}, 
                new Player { SteamID = "2", Name = "Kathy"}, 
                new Player { SteamID = "3", Name = "Baltasar" } });

            Assert.Equal(2, t.Players.Count);
            Assert.Equal(2, ct.Players.Count);
            Assert.True(t.Players.Any(x => x.Name.Equals("BOT")) || ct.Players.Any(x => x.Name.Equals("BOT")));
        }

        [Fact]
        public async Task GetAssignedPlayers_TwoStrongPlayersAndTwoMediumPlayers_OneStrongOneMediumEach()
        {
            var match = new Match { Id = "M1" };
            var strongPlayer1 = new Player { SteamID = "1" };
            match.PlayerResults.Add(new MatchStatistics { SteamID = 1, Id = Guid.NewGuid().ToString(), Kills = 10, Deaths = 2, Rounds = 5 });
            var strongPlayer2 = new Player { SteamID = "2" };
            match.PlayerResults.Add(new MatchStatistics { SteamID = 2, Id = Guid.NewGuid().ToString(), Kills = 11, Deaths = 3, Rounds = 5 });
            var mediumPlayer1 = new Player { SteamID = "3" };
            match.PlayerResults.Add(new MatchStatistics { SteamID = 3, Id = Guid.NewGuid().ToString(), Kills = 2, Deaths = 2, Rounds = 5 });
            var mediumPlayer2 = new Player { SteamID = "4" };
            match.PlayerResults.Add(new MatchStatistics { SteamID = 4, Id = Guid.NewGuid().ToString(), Kills = 2, Deaths = 3, Rounds = 5 });
            myMatchRepository.Add(match);
            myMatchRepository.SaveChanges();
            var skillBasedAssigner = new SkillBasedAssigner(myMatchRepository, new SteamworksApi());

            (var t, var ct) = await skillBasedAssigner.GetAssignedPlayers(new List<Player> { strongPlayer1, strongPlayer2, mediumPlayer1, mediumPlayer2 });

            Assert.Equal(2, t.Players.Count);
            Assert.Equal(2, ct.Players.Count);
            Assert.False(t.Players.Contains(strongPlayer1) && t.Players.Contains(strongPlayer2));
            Assert.False(ct.Players.Contains(strongPlayer1) && ct.Players.Contains(strongPlayer2));
        }

        public void Dispose()
        {
            myMatchRepository.Database.EnsureDeleted();
            myMatchRepository.Dispose();
        }
    }
}
