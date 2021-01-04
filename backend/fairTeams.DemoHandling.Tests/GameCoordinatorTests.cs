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
        [Fact(Skip = "See comment above. This will fail soon...")]
        public void GetMatchInfo_ShareCodeInput_ReturnsCorrectDateAndDownloadURL()
        {
            var gameCoordinatorClient = new GameCoordinatorClient();
            var gameRequest = ShareCode.Decode("CSGO-j6hrT-hvqmd-pNMXY-TuTrq-aXnMC");
            var demo = new Demo { GameRequest = gameRequest };

            var match = gameCoordinatorClient.GetMatchInfo(demo);

            var expectedMatchDate = new System.DateTime(637451358060000000);
            var expectedDownloadURL = @"http://replay191.valve.net/730/003456465718474703287_0558788749.dem.bz2";

            Assert.Equal(expectedMatchDate, match.Date);
            Assert.Equal(expectedDownloadURL, match.Demo.DownloadURL);
        }
    }
}
