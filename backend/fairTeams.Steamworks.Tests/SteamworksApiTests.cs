using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace fairTeams.Steamworks.Tests
{
    public class SteamworksApiTests
    {
        private const string myPrivateProfilePlayerSteamID = "76561199120831930";
        private const string myPrivateProfilePlayerSteamUsername = "fairteamsai";
        private const string myPublicProfilePlayerSteamID = "76561197973591119";

        [Fact]
        public void ParseSteamUsernames_ValidSteamID_ReturnsFairTeamsAIUsername()
        {
            var steamworksApi = new SteamworksApi();

            var steamUsername = steamworksApi.ParseSteamUsernames(new List<string> { myPrivateProfilePlayerSteamID }).Result.First().Value;

            Assert.Equal(myPrivateProfilePlayerSteamUsername, steamUsername);
        }

        [Fact]
        public void ParseSteamUsernames_InvalidSteamID_ReturnsNothing()
        {
            var invalidSteamID = "0";
            var steamworksApi = new SteamworksApi();

            var steamIDsWithUsername = steamworksApi.ParseSteamUsernames(new List<string> { invalidSteamID }).Result;

            Assert.Empty(steamIDsWithUsername);
        }

        [Fact]
        public void ParseSteamUsernames_OneValidOneInvalidSteamID_ReturnsUsernameForValidSteamID()
        {
            var invalidSteamID = "0";
            var oneValidOneInvalidSteamID = new List<string> { myPrivateProfilePlayerSteamID, invalidSteamID };
            var steamworksApi = new SteamworksApi();

            var steamIDsWithUsername = steamworksApi.ParseSteamUsernames(oneValidOneInvalidSteamID).Result;

            Assert.Single(steamIDsWithUsername);
            Assert.Equal(myPrivateProfilePlayerSteamUsername, steamIDsWithUsername.First().Value);
        }

        [Fact]
        public async Task ParsePlayerStatistics_ProfileNotPublic_ThrowsProfileNotPublicException()
        {
            var steamworksApi = new SteamworksApi();

            var parseStatisticsTask = steamworksApi.ParsePlayerStatistics(myPrivateProfilePlayerSteamID);

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
            var steamworksApi = new SteamworksApi();

            var statistics = steamworksApi.ParsePlayerStatistics(myPublicProfilePlayerSteamID).Result;

            Assert.True(statistics.Any());

            foreach (var expectedStatistic in expectedStatistics)
            {
                Assert.True(statistics.Select(x => x.Name == expectedStatistic).Any());
            }
        }

        [Fact]
        public async Task GetNextMatchSharingCode_PreviousCodeIsNotNewest_ReturnsNextSharingCode()
        {
            var previousSharingCode = "CSGO-ndsnw-9jkUc-six5k-y2hcE-kosSJ";
            var authenticationToken = "7TDM-B27HW-THBQ";

            var steamworksApi = new SteamworksApi();

            var nextSharingCode = await steamworksApi.GetNextMatchSharingCode(myPublicProfilePlayerSteamID, authenticationToken, previousSharingCode);

            Assert.Equal("CSGO-k2TXT-9rmts-XE8G2-yqGVu-FhnEM", nextSharingCode);
        }
    }
}
