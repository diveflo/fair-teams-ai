using Xunit;

namespace fairTeams.Core.Tests
{
    public class SteamIdDecoderTests
    {
        [Theory]
        [InlineData(76561197973591119, 13325391u)]
        [InlineData(76561198258023370, 297757642u)]
        [InlineData(76561198011775117, 51509389u)]
        [InlineData(76561198011654217, 51388489u)]
        [InlineData(76561197984050254, 23784526u)]
        [InlineData(76561199045573415, 1085307687u)]
        [InlineData(76561197978519504, 18253776u)]
        [InlineData(76561198031200891, 70935163u)]
        [InlineData(76561197995643389, 35377661u)]
        public void ToAccountId_ValidSteamId_ReturnsCorrectAccountId(long steamId, uint expectedAccountId)
        {
            var accountId = SteamIdDecoder.ToAccountId(steamId);

            Assert.Equal(expectedAccountId, accountId);
        }
    }
}
