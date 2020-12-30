using fairTeams.Core;
using fairTeams.DemoAnalyzer;
using System.Collections.Generic;
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

            var statisticsAndi = demoReader.Match.PlayerResults.Single(x => x.SteamID == 76561199045573415);
            Assert.Equal(22, statisticsAndi.Kills);
            Assert.Equal(7, statisticsAndi.Deaths);
            Assert.Equal(21, statisticsAndi.Rounds);
            Assert.Equal(12, statisticsAndi.OneKill);
            Assert.Equal(2, statisticsAndi.TwoKill);
            Assert.Equal(2, statisticsAndi.ThreeKill);
            Assert.Equal(1.641, statisticsAndi.HLTVScore);

        }

        [Fact (Skip = "data only locally avaialble for now")]
        public void Read_CompetitiveMatch_ReturnsCorrectStatistics()
        {
            var demo = new Demo { FilePath = @"C:\Program Files (x86)\Steam\steamapps\common\Counter-Strike Global Offensive\csgo\replays\match730_003455341431328080096_0558631479_137.dem" };
            var demoReader = new DemoReader(demo);

            demoReader.Read();

            Assert.Equal(11, demoReader.Match.CTScore);
            Assert.Equal(16, demoReader.Match.TScore);

            var expectedStatisticsForSteamId = new List<MatchStatistics>
            {
                new MatchStatistics { SteamID = 76561199051736738, Kills = 33, Deaths = 20, Rounds = 27, OneKill = 8, TwoKill = 4, ThreeKill = 3, FourKill = 2 },
                new MatchStatistics { SteamID = 76561198011486180, Kills = 15, Deaths = 22, Rounds = 27, OneKill = 8, TwoKill = 2, ThreeKill = 1 },
                new MatchStatistics { SteamID = 76561198449086269, Kills = 15, Deaths = 22, Rounds = 27, OneKill = 9, TwoKill = 3 },
                new MatchStatistics { SteamID = 76561198308728638, Kills = 24, Deaths = 21, Rounds = 27, OneKill = 10, TwoKill = 4, ThreeKill = 2 },
                new MatchStatistics { SteamID = 76561198322776705, Kills = 14, Deaths = 21, Rounds = 27, OneKill = 9, TwoKill = 1, ThreeKill = 1 },
                new MatchStatistics { SteamID = 76561198449214703, Kills = 37, Deaths = 20, Rounds = 27, OneKill = 3, TwoKill = 6, ThreeKill = 6, FourKill = 1 },
                new MatchStatistics { SteamID = 76561197984050254, Kills = 12, Deaths = 22, Rounds = 27, OneKill = 8, TwoKill = 2 },
                new MatchStatistics { SteamID = 76561197973591119, Kills = 29, Deaths = 18, Rounds = 27, OneKill = 6, TwoKill = 4, ThreeKill = 2, FourKill = 1, FiveKill = 1 },
                new MatchStatistics { SteamID = 76561198115219464, Kills = 24, Deaths = 19, Rounds = 27, OneKill = 5, TwoKill = 8, ThreeKill = 1 },
                new MatchStatistics { SteamID = 76561198021024163, Kills = 3, Deaths = 23, Rounds = 27, OneKill = 4 }
            };

            foreach(var playerResult in demoReader.Match.PlayerResults)
            {
                var expected = expectedStatisticsForSteamId.Single(x => x.SteamID == playerResult.SteamID);
                Assert.Equal(expected.Kills, playerResult.Kills);
                Assert.Equal(expected.Deaths, playerResult.Deaths);
                Assert.Equal(expected.Rounds, playerResult.Rounds);
                Assert.Equal(expected.OneKill, playerResult.OneKill);
                Assert.Equal(expected.TwoKill, playerResult.TwoKill);
                Assert.Equal(expected.ThreeKill, playerResult.ThreeKill);
                Assert.Equal(expected.FourKill, playerResult.FourKill);
                Assert.Equal(expected.FiveKill, playerResult.FiveKill);
            }
        }

        [Fact (Skip = "data only locally avaialble for now")]
        public void Read_MatchWithTeamkills_CountedAsNegativeKill()
        {
            var teamkillerSteamID = 76561198021024163;
            var demo = new Demo() { FilePath = @"C:\Program Files (x86)\Steam\steamapps\common\Counter-Strike Global Offensive\csgo\replays\match730_003455336292399710316_1349054772_183.dem" };
            var demoReader = new DemoReader(demo);

            demoReader.Read();

            Assert.Equal(-1, demoReader.Match.PlayerResults.Single(x => x.SteamID == teamkillerSteamID).Kills);
        }
    }
}
