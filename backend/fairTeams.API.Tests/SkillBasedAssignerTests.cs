using fairTeams.API.Rating;
using fairTeams.Core;
using fairTeams.Steamworks;
using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
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
        public async Task GetAssignedPlayers_ThreePlayersIncludeBotParameterFalse_NoBotAddedToBalanceTeamSizes()
        {
            var match = new Match { Id = "M1" };
            match.PlayerResults.Add(new MatchStatistics { SteamID = 1, Id = "1", Kills = 1, Deaths = 1 });
            match.PlayerResults.Add(new MatchStatistics { SteamID = 2, Id = "2", Kills = 2, Deaths = 2 });
            match.PlayerResults.Add(new MatchStatistics { SteamID = 3, Id = "3", Kills = 3, Deaths = 3 });
            myMatchRepository.Add(match);
            myMatchRepository.SaveChanges();
            var skillBasedAssigner = new SkillBasedAssigner(myMatchRepository, new SteamworksApi());

            (var t, var ct) = await skillBasedAssigner.GetAssignedPlayers(
                new List<Player> {
                    new Player { SteamID = "1", Name = "Emma"},
                    new Player { SteamID = "2", Name = "Kathy"},
                    new Player { SteamID = "3", Name = "Baltasar" } },
                false);

            var numberOfAssignedPlayers = t.Players.Count + ct.Players.Count;
            Assert.Equal(3, numberOfAssignedPlayers);
        }

        [Fact]
        public async Task GetAssignedPlayers_TwoStrongPlayersAndFourMediumPlayers_OneStrongTwoMediumEach()
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
            var mediumPlayer3 = new Player { SteamID = "5" };
            match.PlayerResults.Add(new MatchStatistics { SteamID = 5, Id = Guid.NewGuid().ToString(), Kills = 2, Deaths = 3, Rounds = 5 });
            var mediumPlayer4 = new Player { SteamID = "6" };
            match.PlayerResults.Add(new MatchStatistics { SteamID = 6, Id = Guid.NewGuid().ToString(), Kills = 2, Deaths = 3, Rounds = 5 });
            myMatchRepository.Add(match);
            myMatchRepository.SaveChanges();
            var skillBasedAssigner = new SkillBasedAssigner(myMatchRepository, new SteamworksApi());

            (var t, var ct) = await skillBasedAssigner.GetAssignedPlayers(new List<Player> { strongPlayer1, strongPlayer2, mediumPlayer1, mediumPlayer2, mediumPlayer3, mediumPlayer4 });

            Assert.Equal(3, t.Players.Count);
            Assert.Equal(3, ct.Players.Count);
            Assert.False(t.Players.Contains(strongPlayer1) && t.Players.Contains(strongPlayer2));
            Assert.False(ct.Players.Contains(strongPlayer1) && ct.Players.Contains(strongPlayer2));
        }

        [Fact]
        public async Task GetAssignedPlayers_NoMatchesForPlayer_UsesKDRating()
        {
            var steamworksApiMock = new Mock<SteamworksApi>();
            steamworksApiMock
                .Setup(x => x.ParsePlayerStatistics(It.IsAny<string>()))
                .Returns(Task.FromResult((IList<Statistic>)new List<Statistic> {
                    new Statistic {Name = "total_kills", Value = 5 },
                    new Statistic {Name = "total_deaths", Value = 1 },
                    new Statistic {Name = "last_match_kills", Value = 5 },
                    new Statistic {Name = "last_match_deaths", Value = 5 }
                 }));

            var skillBasedAssigner = new SkillBasedAssigner(myMatchRepository, steamworksApiMock.Object);
            var steamIdOfPlayerWithoutMatches = "111";

            (var t, var ct) = await skillBasedAssigner.GetAssignedPlayers(new List<Player> { new Player { SteamID = steamIdOfPlayerWithoutMatches } });

            steamworksApiMock.Verify(x => x.ParsePlayerStatistics(steamIdOfPlayerWithoutMatches));
            var playersOfBothTeams = t.Players.Concat(ct.Players);
            Assert.Equal(5, playersOfBothTeams.Single(x => x.SteamID.Equals(steamIdOfPlayerWithoutMatches)).Skill.SkillScore);
        }

        [Fact]
        public async Task GetAssignedPlayers_NoMatchesForPlayerProfileNotPublic_UsesDummyRating()
        {
            var steamworksApiMock = new Mock<SteamworksApi>();
            steamworksApiMock.Setup(x => x.ParsePlayerStatistics(It.IsAny<string>())).Throws(new ProfileNotPublicException());

            var skillBasedAssigner = new SkillBasedAssigner(myMatchRepository, steamworksApiMock.Object);
            var steamIdOfPlayerWithoutMatches = "111";

            (var t, var ct) = await skillBasedAssigner.GetAssignedPlayers(new List<Player> { new Player { SteamID = steamIdOfPlayerWithoutMatches } });

            var player = t.Players.Concat(ct.Players).Single(x => x.SteamID.Equals(steamIdOfPlayerWithoutMatches));
            Assert.InRange(player.Skill.SkillScore, 0.3, 1.3);
        }

        [Fact]
        public void ScrambledEquals_Test()
        {
            var player1 = new Player { Name = "Player 1", SteamID = "1" };
            player1.Skill.SetRating(new DummyRating { Score = 1.0 });
            var player2 = new Player { Name = "Player 2", SteamID = "2" };
            player2.Skill.SetRating(new DummyRating { Score = 2.0 });
            var player3 = new Player { Name = "Player 3", SteamID = "3" };
            player3.Skill.SetRating(new DummyRating { Score = 3.0 });
            var player4 = new Player { Name = "Player 4", SteamID = "4" };
            player4.Skill.SetRating(new DummyRating { Score = 4.0 });

            var team1 = new Team("1")
            {
                Players = new List<Player> { player1, player2 }
            };
            var team1copy = new Team("1")
            {
                Players = new List<Player> { player1, player2 }
            };
            var team2 = new Team("2")
            {
                Players = new List<Player> { player3, player4 }
            };
            var team2copy = new Team("2")
            {
                Players = new List<Player> { player3, player4 }
            };

            Assert.True(SkillBasedAssigner.ScrambledEquals(team1.Players, team1copy.Players));
            Assert.True(SkillBasedAssigner.ScrambledEquals(team2.Players, team2copy.Players));
            Assert.False(SkillBasedAssigner.ScrambledEquals(team1.Players, team2.Players));
        }

        public void Dispose()
        {
            myMatchRepository.Database.EnsureDeleted();
            myMatchRepository.Dispose();
        }
    }
}
