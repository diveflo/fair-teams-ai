using System.Threading.Tasks;
using Xunit;

namespace fairTeams.DemoHandling.Tests
{
    public class GameCoordinatorTests
    {
        [Fact]
        public async Task GetDownloadURLForMatch_ValidInput_ReturnsCorrectURL()
        {
            var gameCoordinatorClient = new GameCoordinatorClient();
            var gameRequest = new GameRequest { MatchId = 3455336489968206603, OutcomeId = 3455341431328080096, Token = 2615 };
            //var secondGameRequest = new GameRequest { MatchId = 3455712677563728700, OutcomeId = 3455718735615098899, Token = 48028 };

            var downloadUrl = await gameCoordinatorClient.GetDownloadURLForMatch(gameRequest);
            //var secondDownloadUrl = await gameCoordinatorClient.GetDownloadURLForMatch(secondGameRequest);

            Assert.Equal(@"http://replay137.valve.net/730/003455341431328080096_0558631479.dem.bz2", downloadUrl);
            //Assert.Equal(@"http://replay134.valve.net/730/003455718735615098899_1941420956.dem.bz2", secondDownloadUrl);
        }
    }
}
