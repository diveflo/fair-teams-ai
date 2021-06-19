using Xunit;

namespace fairTeams.DemoHandling.Tests
{
    public class ShareCodeDecoderTests
    {
        [Theory]
        [Trait("Category", "unit")]
        [InlineData("CSGO-c8yAf-mYUVd-EYMDm-YZkqQ-66r4N", 3456660989162815693u, 3456667023591866681u, 30789u)]
        [InlineData("CSGO-JyGRk-R6F42-BNsVL-UaMAV-QmeqA", 3455336489968206603u, 3455341431328080096u, 2615u)]
        [InlineData("CSGO-U6UbF-rpnN5-yhxh5-xpYju-YDERE", 3455505353049899588u, 3455511353119211701u, 62578u)]
        [InlineData("CSGO-fCLxU-j7kLH-XFfxT-y47pa-FkBAA", 3455526464961643008u, 3455531870177985027u, 9615u)]
        [InlineData("CSGO-XPBWY-U43tj-DpmEA-jsZRk-34OJM", 3455498807519740337u, 3455504648675263354u, 21931u)]
        [InlineData("CSGO-ndsnw-9jkUc-six5k-y2hcE-kosSJ", 3455512396796264580u, 3455518115545219333u, 8129u)]
        public void Decode_ValidShareCode_ReturnsValidMatchId(string shareCode, ulong matchId, ulong outcomeId, uint token)
        {
            var gameRequest = ShareCodeDecoder.Decode(shareCode);

            Assert.Equal(matchId, gameRequest.MatchId);
            Assert.Equal(outcomeId, gameRequest.OutcomeId);
            Assert.Equal(token, gameRequest.Token);
        }
    }
}
