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

    }
}
