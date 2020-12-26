using fairTeams.API.SteamworksApi;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace fairTeams.API.Tests
{
    public class SteamworksApiTests
    {
        private const string myPrivateProfilePlayerSteamID = "76561199120831930";
        private const string myPrivateProfilePlayerSteamUsername = "fairteamsai";
        private const string myPublicProfilePlayerSteamID = "76561197973591119";

        [Fact]
        public void ParseSteamUsernames_ValidSteamID_ReturnsFairTeamsAIUsername()
        {
            var user = new Player { SteamID = myPrivateProfilePlayerSteamID };

            user = SteamworksApi.SteamworksApi.ParseSteamUsernames(new List<Player> { user }).Result.First();

            Assert.Equal(myPrivateProfilePlayerSteamUsername, user.SteamName);
        }

        [Fact]
        public async Task ParseSteamUsernames_InvalidSteamID_ThrowsPlayerNotFoundException()
        {
            var invalidSteamID = "0";
            var nonExistingUser = new Player { Name = "ImAGhostYo", SteamID = invalidSteamID };

            var parseUsernameTask = SteamworksApi.SteamworksApi.ParseSteamUsernames(new List<Player> { nonExistingUser });

            await Assert.ThrowsAsync<PlayerNotFoundException>(() => parseUsernameTask);
        }

        [Fact]
        public async Task ParsePlayerStatistics_ProfileNotPublic_ThrowsProfileNotPublicException()
        {
            var parseStatisticsTask = SteamworksApi.SteamworksApi.ParsePlayerStatistics(myPrivateProfilePlayerSteamID);

            await Assert.ThrowsAsync<ProfileNotPublicException>(() => parseStatisticsTask);
        }

        [Fact]
        public void ParsePlayerStatistics_ProfilePublic_ReturnsExpectedStatistics()
        {
            var expectedStatistics = new List<string>
            {
                "total_kills",
                "total_deaths",
                "total_damage_done",
                "total_kills_bizon"
            };

            var statistics = SteamworksApi.SteamworksApi.ParsePlayerStatistics(myPublicProfilePlayerSteamID).Result;

            Assert.True(statistics.Any());

            foreach (var expectedStatistic in expectedStatistics)
            {
                Assert.True(statistics.Select(x => x.Name == expectedStatistic).Any());
            }
        }
    }
}
