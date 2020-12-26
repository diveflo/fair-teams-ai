using fairTeams.Core;
using fairTeams.DemoAnalyzer;
using System.Linq;
using Xunit;

namespace fairTeams.DemoParser.Tests
{
    public class DemoReaderTests
    {
        [Fact (Skip = "data only locally avaialble for now")]
        public void Read_MatchOnOurServer_ReturnsCorrectStatistics()
        {
            var demo = new Demo { FilePath = @"C:\Users\Flo\projects\csgo-demo-server\auto0-20201222-205911-1208050719-de_vertigo-honigbiene_vs_waldfrosch.dem" };
            var demoReader = new DemoReader(demo);

            demoReader.Read();

            Assert.Equal(16, demoReader.Match.CTScore);
            Assert.Equal(5, demoReader.Match.TScore);
            var expectedStatisticsPhilip = new MatchStatistics { Kills = 19, Deaths = 15, Rounds = 21, MultipleKills = new MultipleKills { OneKill = 8, TwoKill = 4, ThreeKill = 1 } };
            Assert.Equal(expectedStatisticsPhilip, demoReader.Match.PlayerResults.Single(x => x.Key.SteamID == 76561198258023370).Value);
        }
    }
}
