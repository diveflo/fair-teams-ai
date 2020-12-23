using fairTeams.API.SteamworksApi;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using System.Threading.Tasks;

namespace fairTeams.API.Tests
{
    public class SteamworksApiTests
    {
        [Fact]
        public void ParseSteamUsernames_FloSteamID_ReturnsAntifasupersoldier()
        {
            var expectedSteamUsername = "antifa super soldier";
            var floSteamID = "76561197973591119";
            var flo = new Player { Name = "Flo", SteamID = floSteamID };

            flo = SteamworksApi.SteamworksApi.ParseSteamUsernames(new List<Player> { flo }).Result.First();

            Assert.Equal(expectedSteamUsername, flo.SteamName);
        }

        [Fact]
        public async Task ParseSteamUsernames_InvalidSteamID_ThrowsPlayerNotFoundException()
        {            
            var invalidSteamID = "0";
            var nonExistingUser = new Player { Name = "ImAGhostYo", SteamID = invalidSteamID };

            var parseUsernameTask = SteamworksApi.SteamworksApi.ParseSteamUsernames(new List<Player> { nonExistingUser });

            await Assert.ThrowsAsync<PlayerNotFoundException>(() => parseUsernameTask);
        }
    }
}
