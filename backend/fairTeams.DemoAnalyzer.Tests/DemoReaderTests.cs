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

            var expectedStatisticsAndi = new MatchStatistics { Kills = 22, Deaths = 7, Rounds = 21, MultipleKills = new MultipleKills { OneKill = 12, TwoKill = 2, ThreeKill = 2 } };
            Assert.Equal(expectedStatisticsAndi, demoReader.Match.PlayerResults.Single(x => x.Key.SteamID == 76561199045573415).Value);
            Assert.Equal(1.641, demoReader.Match.PlayerResults.Single(x => x.Key.SteamID == 76561199045573415).Value.HLTVScore);

            var expectedStatisticsPhilip = new MatchStatistics { Kills = 19, Deaths = 15, Rounds = 21, MultipleKills = new MultipleKills { OneKill = 8, TwoKill = 4, ThreeKill = 1 } };
            Assert.Equal(expectedStatisticsPhilip, demoReader.Match.PlayerResults.Single(x => x.Key.SteamID == 76561198258023370).Value);
            Assert.Equal(1.183, demoReader.Match.PlayerResults.Single(x => x.Key.SteamID == 76561198258023370).Value.HLTVScore);

            var expectedStatisticsMarkus = new MatchStatistics { Kills = 12, Deaths = 16, Rounds = 21, MultipleKills = new MultipleKills { OneKill = 4, TwoKill = 4 } };
            Assert.Equal(expectedStatisticsMarkus, demoReader.Match.PlayerResults.Single(x => x.Key.SteamID == 76561197984050254).Value);
            Assert.Equal(0.783, demoReader.Match.PlayerResults.Single(x => x.Key.SteamID == 76561197984050254).Value.HLTVScore);
        }

        [Fact (Skip = "data only locally avaialble for now")]
        public void Read_CompetitiveMatch_ReturnsCorrectStatistics()
        {
            var demo = new Demo { FilePath = @"C:\Program Files (x86)\Steam\steamapps\common\Counter-Strike Global Offensive\csgo\replays\match730_003455341431328080096_0558631479_137.dem" };
            var demoReader = new DemoReader(demo);

            demoReader.Read();

            Assert.Equal(11, demoReader.Match.CTScore);
            Assert.Equal(16, demoReader.Match.TScore);

            var expectedStatisticsForSteamId = new Dictionary<long, MatchStatistics>
            {
                { 76561199051736738, new MatchStatistics { Kills = 33, Deaths = 20, Rounds = 27, MultipleKills = new MultipleKills { OneKill = 8, TwoKill = 4, ThreeKill = 3, FourKill = 2 } } },
                { 76561198011486180, new MatchStatistics { Kills = 15, Deaths = 22, Rounds = 27, MultipleKills = new MultipleKills { OneKill = 8, TwoKill = 2, ThreeKill = 1 } } },
                { 76561198449086269, new MatchStatistics { Kills = 15, Deaths = 22, Rounds = 27, MultipleKills = new MultipleKills { OneKill = 9, TwoKill = 3 } } },
                { 76561198308728638, new MatchStatistics { Kills = 24, Deaths = 21, Rounds = 27, MultipleKills = new MultipleKills { OneKill = 10, TwoKill = 4, ThreeKill = 2 } } },
                { 76561198322776705, new MatchStatistics { Kills = 14, Deaths = 21, Rounds = 27, MultipleKills = new MultipleKills { OneKill = 9, TwoKill = 1, ThreeKill = 1 } } },
                { 76561198449214703, new MatchStatistics { Kills = 37, Deaths = 20, Rounds = 27, MultipleKills = new MultipleKills { OneKill = 3, TwoKill = 6, ThreeKill = 6, FourKill = 1 } } },
                { 76561197984050254, new MatchStatistics { Kills = 12, Deaths = 22, Rounds = 27, MultipleKills = new MultipleKills { OneKill = 8, TwoKill = 2 } } },
                { 76561197973591119, new MatchStatistics { Kills = 29, Deaths = 18, Rounds = 27, MultipleKills = new MultipleKills { OneKill = 6, TwoKill = 4, ThreeKill = 2, FourKill = 1, FiveKill = 1 } } },
                { 76561198115219464, new MatchStatistics { Kills = 24, Deaths = 19, Rounds = 27, MultipleKills = new MultipleKills { OneKill = 5, TwoKill = 8, ThreeKill = 1 } } },
                { 76561198021024163, new MatchStatistics { Kills = 3, Deaths = 23, Rounds = 27, MultipleKills = new MultipleKills { OneKill = 4 } } }
            };

            foreach (var expected in expectedStatisticsForSteamId)
            {
                Assert.Equal(expected.Value, demoReader.Match.PlayerResults.Single(x => x.Key.SteamID == expected.Key).Value);
            }
        }

        [Fact (Skip = "data only locally avaialble for now")]
        public void Read_MatchWithTeamkills_CountedAsNegativeKill()
        {
            var teamkillerSteamID = 76561198021024163;
            var expectedStatistics = new MatchStatistics { Kills = -1, Deaths = 17, Rounds = 17 };

            var demo = new Demo() { FilePath = @"C:\Program Files (x86)\Steam\steamapps\common\Counter-Strike Global Offensive\csgo\replays\match730_003455336292399710316_1349054772_183.dem" };
            var demoReader = new DemoReader(demo);

            demoReader.Read();

            Assert.Equal(expectedStatistics, demoReader.Match.PlayerResults.Single(x => x.Key.SteamID == teamkillerSteamID).Value);
        }
    }
}
