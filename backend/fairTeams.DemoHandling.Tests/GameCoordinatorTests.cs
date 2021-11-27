using fairTeams.Core;
using System.Collections.Generic;
using Xunit;

namespace fairTeams.DemoHandling.Tests
{
    public class GameCoordinatorTests
    {
        [Fact]
        public void GetMatchInfo_ShareCodeInput_ReturnsCorrectDateAndDownloadURL()
        {
            var gameCoordinatorClient = new GameCoordinatorClient();
            gameCoordinatorClient.ConnectAndLogin();
            var gameRequest = ShareCodeDecoder.Decode("CSGO-j6hrT-hvqmd-pNMXY-TuTrq-aXnMC");
            var demo = new Demo { GameRequest = gameRequest };

            var match = gameCoordinatorClient.GetMatchInfo(demo);

            var expectedMatchDate = new System.DateTime(637451358060000000);
            var expectedDownloadURL = @"http://replay191.valve.net/730/003456465718474703287_0558788749.dem.bz2";

            Assert.Equal(expectedMatchDate, match.Date);
            Assert.Equal(expectedDownloadURL, match.Demo.DownloadURL);
            Assert.Equal(13, match.TScore);
            Assert.Equal(16, match.CTScore);
            Assert.Equal(29, match.Rounds);
        }

        [Fact(Skip = "Obviously this isn't stable...")]
        public void GetRank_GoldNovaI_GoldNovaI()
        {
            var gameCoordinatorClient = new GameCoordinatorClient();
            var steamId = 76561197973591119;

            var rank = gameCoordinatorClient.GetRank(steamId);

            Assert.Equal(Rank.GoldNovaI, rank);
        }
    }
}
