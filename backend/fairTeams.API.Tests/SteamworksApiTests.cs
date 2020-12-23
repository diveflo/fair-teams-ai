using fairTeams.API.SteamworksApi;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using System.Threading.Tasks;

namespace fairTeams.API.Tests
{
    public class SteamworksApiTests
    {
        private const string myPrivateProfilePlayerSteamID = "76561199120831930";
        private const string myPrivateProfilePlayerSteamUsername = "fairteamsai";
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
    }
}
