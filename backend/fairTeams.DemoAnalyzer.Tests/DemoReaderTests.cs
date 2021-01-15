using fairTeams.Core;
using fairTeams.DemoAnalyzer;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Xunit;

namespace fairTeams.DemoParser.Tests
{
    public class DemoReaderTests
    {
        [Fact]
        public void Read_MatchOnOurServer_ReturnsCorrectStatistics()
        {
            var demo = new Demo { FilePath = Path.Combine("TestData", @"auto0-20210103-190414-139014994-de_dust2-honigbiene_vs_waldfrosch.dem") };

            //Setting minimum number of rounds & players to 0 (different than during runtime) to allow reading this smaller demo file
            var demoReader = new DemoReader(new Match { Demo = demo }, 0, 0);

            demoReader.ReadHeader();
            demoReader.Read();

            Assert.Equal(0, demoReader.Match.CTScore);
            Assert.Equal(3, demoReader.Match.TScore);

            var expectedStatisticsAlex = demoReader.Match.PlayerResults.Single(x => x.SteamID == 76561198011775117);
            Assert.Equal(1, expectedStatisticsAlex.Kills);
            Assert.Equal(0, expectedStatisticsAlex.Deaths);
            Assert.Equal(3, expectedStatisticsAlex.Rounds);
            Assert.Equal(1, expectedStatisticsAlex.OneKill);
            Assert.Equal(1.096, expectedStatisticsAlex.HLTVScore);

            var expectedStatisticsFlo = demoReader.Match.PlayerResults.Single(x => x.SteamID == 76561197973591119);
            Assert.Equal(-1, expectedStatisticsFlo.Kills);
            Assert.Equal(2, expectedStatisticsFlo.Deaths);
            Assert.Equal(3, expectedStatisticsFlo.Rounds);
            Assert.Equal(0.091, expectedStatisticsFlo.HLTVScore);
        }

        [Fact]
        public void Read_MatchHasDifferentNumberOfRoundsPreset_ThrowsInconsistentStatistics()
        {
            var demo = new Demo { FilePath = Path.Combine("TestData", @"auto0-20210103-190414-139014994-de_dust2-honigbiene_vs_waldfrosch.dem") };
            var match = new Match { Demo = demo, Rounds = 6 };

            //Setting minimum number of rounds & players to 0 (different than during runtime) to allow reading this smaller demo file
            var demoReader = new DemoReader(match, 0, 0);
            demoReader.ReadHeader();

            Assert.Throws<InconsistentStatisticsException>(() => demoReader.Read());
        }

        [Fact]
        public void Read_MatchHasDifferentScorePreset_ThrowsInconsistentStatistics()
        {
            var demo = new Demo { FilePath = Path.Combine("TestData", @"auto0-20210103-190414-139014994-de_dust2-honigbiene_vs_waldfrosch.dem") };
            var match = new Match { Demo = demo, CTScore = 3, TScore = 0 };

            //Setting minimum number of rounds & players to 0 (different than during runtime) to allow reading this smaller demo file
            var demoReader = new DemoReader(match, 0, 0);
            demoReader.ReadHeader();

            Assert.Throws<InconsistentStatisticsException>(() => demoReader.Read());
        }

        [Fact(Skip = "Data only locally available")]
        public void Read_GameIsRestartedAfterMatchStarted_DoesNotCountEventsBeforeRealMatchStart()
        {
            var demo = new Demo { FilePath = @"C:\Users\Flo\projects\csgo-demo-server\auto0-20210102-204730-1247575572-de_mirage-honigbiene_vs_waldfrosch.dem" };
            var demoReader = new DemoReader(new Match { Demo = demo });

            demoReader.ReadHeader();
            demoReader.Read();

            Assert.Equal(15, demoReader.Match.CTScore);
            Assert.Equal(15, demoReader.Match.TScore);

            var expectedStatisticsForSteamId = new List<MatchStatistics>
            {
                new MatchStatistics { SteamID = 76561197984050254, Kills = 26, Deaths = 24, Rounds = 30 },
                new MatchStatistics { SteamID = 76561197973591119, Kills = 24, Deaths = 27, Rounds = 30 },
                new MatchStatistics { SteamID = 76561197978519504, Kills = 21, Deaths = 22, Rounds = 30 },
                new MatchStatistics { SteamID = 76561198011775117, Kills = 21, Deaths = 18, Rounds = 30 },
                new MatchStatistics { SteamID = 76561198011654217, Kills = 15, Deaths = 23, Rounds = 30 },
                new MatchStatistics { SteamID = 76561199045573415, Kills = 37, Deaths = 19, Rounds = 30 },
                new MatchStatistics { SteamID = 76561198258023370, Kills = 25, Deaths = 19, Rounds = 30 },
                new MatchStatistics { SteamID = 76561197995643389, Kills = 21, Deaths = 23, Rounds = 30 },
                new MatchStatistics { SteamID = 76561198031200891, Kills = 17, Deaths = 21, Rounds = 30 },
                new MatchStatistics { SteamID = 76561198053826525, Kills = 14, Deaths = 25, Rounds = 30 }
            };

            foreach (var playerResult in demoReader.Match.PlayerResults)
            {
                var expected = expectedStatisticsForSteamId.Single(x => x.SteamID == playerResult.SteamID);
                Assert.Equal(expected.Kills, playerResult.Kills);
                Assert.Equal(expected.Deaths, playerResult.Deaths);
                Assert.Equal(expected.Rounds, playerResult.Rounds);
            }
        }

        [Fact(Skip = "Data only locally available")]
        public void Read_OnlyFourRounds_CorrectStatistics()
        {
            var demo = new Demo { FilePath = @"C:\Users\Flo\projects\csgo-demo-server\auto0-20201222-213144-349508145-de_inferno-honigbiene_vs_waldfrosch.dem" };
            var demoReader = new DemoReader(new Match { Demo = demo });

            demoReader.ReadHeader();
            demoReader.Read();

            Assert.Equal(2, demoReader.Match.CTScore);
            Assert.Equal(2, demoReader.Match.TScore);
            Assert.Equal(4, demoReader.Match.Rounds);

            var expectedStatisticsForSteamId = new List<MatchStatistics>
            {
                new MatchStatistics { SteamID = 76561198011775117, Kills = 1, Deaths = 3, Rounds = 4, OneKill = 0, TwoKill = 1 },
                new MatchStatistics { SteamID = 76561197984050254, Kills = 1, Deaths = 4, Rounds = 4, OneKill = 1 },
                new MatchStatistics { SteamID = 76561198053826525, Kills = -1, Deaths = 4, Rounds = 4 },
                new MatchStatistics { SteamID = 76561199045573415, Kills = 8, Deaths = 1, Rounds = 4, OneKill = 2, TwoKill = 0, ThreeKill = 2 },
                new MatchStatistics { SteamID = 76561197973591119, Kills = 0, Deaths = 2, Rounds = 4 },
                new MatchStatistics { SteamID = 76561198258023370, Kills = -1, Deaths = 2, Rounds = 4, OneKill = 1 }
            };

            foreach (var playerResult in demoReader.Match.PlayerResults)
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

        [Fact(Skip = "Data only locally available")]
        public void Read_WarumupIsRestarted_DoesNotCountEventsFromAnyWarmup()
        {
            var demo = new Demo { FilePath = @"C:\Users\Flo\projects\csgo-demo-server\auto0-20210102-215342-2066393818-de_inferno-honigbiene_vs_waldfrosch.dem" };
            var demoReader = new DemoReader(new Match { Demo = demo });

            demoReader.ReadHeader();
            demoReader.Read();

            var expectedStatisticsForSteamId = new List<MatchStatistics>
            {
                new MatchStatistics { SteamID = 76561197984050254, Kills = 20, Deaths = 25, Rounds = 30 },
                new MatchStatistics { SteamID = 76561197973591119, Kills = 20, Deaths = 22, Rounds = 30 },
                new MatchStatistics { SteamID = 76561197978519504, Kills = 21, Deaths = 24, Rounds = 30 },
                new MatchStatistics { SteamID = 76561198011775117, Kills = 32, Deaths = 23, Rounds = 30 },
                new MatchStatistics { SteamID = 76561198011654217, Kills = 14, Deaths = 26, Rounds = 30 },
                new MatchStatistics { SteamID = 76561199045573415, Kills = 35, Deaths = 19, Rounds = 30 },
                new MatchStatistics { SteamID = 76561198258023370, Kills = 36, Deaths = 20, Rounds = 30 },
                new MatchStatistics { SteamID = 76561197995643389, Kills = 17, Deaths = 20, Rounds = 30 },
                new MatchStatistics { SteamID = 76561198031200891, Kills = 11, Deaths = 23, Rounds = 30 },
                new MatchStatistics { SteamID = 76561198053826525, Kills = 21, Deaths = 25, Rounds = 30 }
            };

            foreach (var playerResult in demoReader.Match.PlayerResults)
            {
                var expected = expectedStatisticsForSteamId.Single(x => x.SteamID == playerResult.SteamID);
                Assert.Equal(expected.Kills, playerResult.Kills);
                Assert.Equal(expected.Deaths, playerResult.Deaths);
                Assert.Equal(expected.Rounds, playerResult.Rounds);
            }
        }

        [Fact(Skip = "Data only locally available")]
        public void Read_GameIsRestartedAfterMatchStarted_DoesNotCountRoundsBeforeRealMatchStart()
        {
            var demo = new Demo { FilePath = @"C:\Users\Flo\projects\csgo-demo-server\auto0-20210102-225615-1235223714-de_dust2-honigbiene_vs_waldfrosch.dem" };
            var demoReader = new DemoReader(new Match { Demo = demo });

            demoReader.ReadHeader();
            demoReader.Read();

            Assert.Equal(30, demoReader.Match.Rounds);
            foreach (var player in demoReader.Match.PlayerResults)
            {
                Assert.Equal(30, player.Rounds);
            }
        }

        [Fact(Skip = "Data only locally available")]
        public void Read_CompetitiveMatch_ReturnsCorrectStatistics()
        {
            var demo = new Demo { FilePath = @"C:\Program Files (x86)\Steam\steamapps\common\Counter-Strike Global Offensive\csgo\replays\match730_003455341431328080096_0558631479_137.dem" };
            var demoReader = new DemoReader(new Match { Demo = demo });

            demoReader.ReadHeader();
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

            foreach (var playerResult in demoReader.Match.PlayerResults)
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

        [Fact(Skip = "Data only locally available")]
        public void Read_MatchWithTeamkills_CountedAsNegativeKill()
        {
            var teamkillerSteamID = 76561198021024163;
            var demo = new Demo() { FilePath = @"C:\Program Files (x86)\Steam\steamapps\common\Counter-Strike Global Offensive\csgo\replays\match730_003455336292399710316_1349054772_183.dem" };
            var demoReader = new DemoReader(new Match { Demo = demo });

            demoReader.ReadHeader();
            demoReader.Read();

            Assert.Equal(-1, demoReader.Match.PlayerResults.Single(x => x.SteamID == teamkillerSteamID).Kills);
        }
    }
}
