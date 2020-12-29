using Xunit;

namespace fairTeams.DemoHandling.Tests
{
    public class ShareCodeTests
    {
        [Fact]
        public void Decode_ValidShareCode_ReturnsValidMatchId1()
        {
            var shareCode = "CSGO-JyGRk-R6F42-BNsVL-UaMAV-QmeqA";

            var gameRequest = ShareCode.Decode(shareCode);

            Assert.Equal(3455336489968206603u, gameRequest.MatchId);
            Assert.Equal(3455341431328080096u, gameRequest.OutcomeId);
            Assert.Equal(2615u, gameRequest.Token);
        }
    }
}
