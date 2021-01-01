using fairTeams.Core;
using Xunit;

namespace fairTeams.DemoHandling.Tests
{
    public class GameCoordinatorTests
    {
        [Fact]
        public void GetMatchInfo_ValidInput_ReturnsCorrectURL()
        {
            var gameCoordinatorClient = new GameCoordinatorClient();
            var gameRequest = ShareCode.Decode("CSGO-inhFT-GJ96p-bkn6N-duydj-a5UrC");
            var demo = new Demo { GameRequest = gameRequest };

            var match = gameCoordinatorClient.GetMatchInfo(demo);

            Assert.Equal(@"http://replay137.valve.net/730/003455341431328080096_0558631479.dem.bz2", match.Demo.DownloadURL);
        }
    }
}
