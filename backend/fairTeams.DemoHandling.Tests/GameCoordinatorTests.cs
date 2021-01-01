using fairTeams.Core;
using Xunit;

namespace fairTeams.DemoHandling.Tests
{
    public class GameCoordinatorTests
    {
        /// <summary>
        /// This will fail pretty soon once Valve expires the demo...we should probably instead somehow get a current/recent share-code automatically.
        /// Though this kind of breaks the whole "unit"-test approach :/
        /// </summary>
        [Fact]
        public void GetMatchInfo_ShareCodeInput_ReturnsCorrectDateAndDownloadURL()
        {
            var gameCoordinatorClient = new GameCoordinatorClient();
            var gameRequest = ShareCode.Decode("CSGO-inhFT-GJ96p-bkn6N-duydj-a5UrC");
            var demo = new Demo { GameRequest = gameRequest };

            var match = gameCoordinatorClient.GetMatchInfo(demo);

            var expectedMatchDate = new System.DateTime(637438359890000000);
            var expectedDownloadURL = @"http://replay191.valve.net/730/003453673016922210752_2051865056.dem.bz2";

            Assert.Equal(expectedMatchDate, match.Date);
            Assert.Equal(expectedDownloadURL, match.Demo.DownloadURL);
        }
    }
}
