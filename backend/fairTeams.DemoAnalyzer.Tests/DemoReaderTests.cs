using Azure.Storage.Blobs;
using fairTeams.Core;
using fairTeams.DemoAnalyzer;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Xunit;

namespace fairTeams.DemoParser.Tests
{
    public class AzureStorageFixture
    {
        public AzureStorageFixture()
        {
            var connectionString = Environment.GetEnvironmentVariable("AZURE_STORAGE_CONNECTION_STRING");
            var blobServiceClient = new BlobServiceClient(connectionString);
            var containerClient = blobServiceClient.GetBlobContainerClient("demos");

            foreach (var blobItem in containerClient.GetBlobs())
            {
                var localFilePath = Path.GetFullPath(Path.Combine("TestData", blobItem.Name));
                if (File.Exists(localFilePath))
                {
                    var localFileSizeInBytes = GetFileSizeInBytes(localFilePath);
                    var blobFileSizeInBytes = blobItem.Properties.ContentLength;
                    if (localFileSizeInBytes.Equals(blobFileSizeInBytes))
                    {
                        continue;
                    }
                }
                var blobClient = containerClient.GetBlobClient(blobItem.Name);
                blobClient.DownloadTo(Path.Combine("TestData", blobItem.Name));
            }
        }

        public static long GetFileSizeInBytes(string filename)
        {
            var fileInfo = new FileInfo(filename);
            return fileInfo.Length;
        }
    }

    public class DemoReaderTests : IClassFixture<AzureStorageFixture>
    {
        [Fact]
        [Trait("Category", "unit")]
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

        [Theory]
        [Trait("Category", "unit")]
        [InlineData(@"auto0-20210103-190414-139014994-de_dust2-honigbiene_vs_waldfrosch.dem", 0, 3)]
        [InlineData(@"auto0-20210403-200450-1500889020-de_mirage-honigbiene_vs_waldfrosch.dem", 3, 16)]
        [InlineData(@"auto0-20201222-213144-349508145-de_inferno-honigbiene_vs_waldfrosch.dem", 2, 2)]
        [InlineData(@"auto0-20210102-204730-1247575572-de_mirage-honigbiene_vs_waldfrosch.dem", 15, 15)]
        public void Read_GameOnOurServer_ReturnsCorrectScores(string demoFileName, int CTScore, int TScore)
        {
            var demo = new Demo { FilePath = Path.Combine("TestData", demoFileName) };

            var demoReader = new DemoReader(new Match { Demo = demo }, 0, 0);

            var match = demoReader.Parse();

            Assert.Equal(CTScore, match.CTScore);
            Assert.Equal(TScore, match.TScore);
        }

        [Theory]
        [Trait("Category", "unit")]
        [InlineData(@"de_inferno_56500117958089672.dem", 16, 9)]
        [InlineData(@"de_mirage_5631969914449548.dem", 9, 7)]
        public void Read_MatchMakingGame_ReturnsCorrectScores(string demoFileName, int CTScore, int TScore)
        {
            var demo = new Demo { FilePath = Path.Combine("TestData", demoFileName) };

            var demoReader = new DemoReader(new Match { Demo = demo }, 0, 0);

            var match = demoReader.Parse();

            Assert.Equal(CTScore, match.CTScore);
            Assert.Equal(TScore, match.TScore);
        }

        [Theory]
        [Trait("Category", "unit")]
        [InlineData(@"de_inferno_56500117958089672.dem", "76561198996567053", "76561198021024163")]
        [InlineData(@"de_mirage_5631969914449548.dem", "76561198055707754", "76561197995643389")]
        public void Read_MatchMakingGame_ReturnsCorrectPlayerHighestAndLowestHLTV(string demoFileName, string highestHLTVPlayerSteamId, string lowestHLTVPlayerSteamId)
        {
            var demo = new Demo { FilePath = Path.Combine("TestData", demoFileName) };

            var demoReader = new DemoReader(new Match { Demo = demo }, 0, 0);

            var match = demoReader.Parse();

            var orderedByDescendingHLTV = match.PlayerResults.OrderByDescending(x => x.HLTVScore);

            Assert.Equal(long.Parse(highestHLTVPlayerSteamId), orderedByDescendingHLTV.First().SteamID);
            Assert.Equal(long.Parse(lowestHLTVPlayerSteamId), orderedByDescendingHLTV.Last().SteamID);
        }

        [Theory]
        [Trait("Category", "unit")]
        [InlineData(@"auto0-20210103-190414-139014994-de_dust2-honigbiene_vs_waldfrosch.dem", CompetitiveMatchType.Short)]
        [InlineData(@"auto0-20210403-200450-1500889020-de_mirage-honigbiene_vs_waldfrosch.dem", CompetitiveMatchType.Standard)]
        [InlineData(@"auto0-20201222-213144-349508145-de_inferno-honigbiene_vs_waldfrosch.dem", CompetitiveMatchType.Short)]
        [InlineData(@"auto0-20210102-204730-1247575572-de_mirage-honigbiene_vs_waldfrosch.dem", CompetitiveMatchType.Standard)]
        public void Read_GameOnOurServer_ReturnsCorrectMatchType(string demoFileName, CompetitiveMatchType expectedMatchType)
        {
            var demo = new Demo { FilePath = Path.Combine("TestData", demoFileName) };

            var demoReader = new DemoReader(new Match { Demo = demo }, 0, 0);

            var match = demoReader.Parse();

            foreach(var matchStatistics in match.PlayerResults)
            {
                Assert.Equal(expectedMatchType, matchStatistics.MatchType);
            }
        }

        [Theory]
        [Trait("Category", "unit")]
        [InlineData(@"de_inferno_56500117958089672.dem", CompetitiveMatchType.Standard)]
        [InlineData(@"de_mirage_5631969914449548.dem", CompetitiveMatchType.Short)]
        public void Read_MatchMakingGame_ReturnsCorrectMatchType(string demoFileName, CompetitiveMatchType expectedMatchType)
        {
            var demo = new Demo { FilePath = Path.Combine("TestData", demoFileName) };

            var demoReader = new DemoReader(new Match { Demo = demo }, 0, 0);

            var match = demoReader.Parse();

            foreach (var matchStatistics in match.PlayerResults)
            {
                Assert.Equal(expectedMatchType, matchStatistics.MatchType);
            }
        }

        [Fact]
        [Trait("Category", "unit")]
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
        [Trait("Category", "unit")]
        public void Parse_MatchHasDifferentNumberOfRoundsPreset_ThrowsInconsistentStatistics()
        {
            var demo = new Demo { FilePath = Path.Combine("TestData", @"auto0-20210103-190414-139014994-de_dust2-honigbiene_vs_waldfrosch.dem") };
            var match = new Match { Demo = demo, Rounds = 6 };

            //Setting minimum number of rounds & players to 0 (different than during runtime) to allow reading this smaller demo file
            var demoReader = new DemoReader(match, 0, 0);

            Assert.Throws<InconsistentStatisticsException>(() => demoReader.Parse());
        }

        [Fact]
        [Trait("Category", "unit")]
        public void Read_MatchHasDifferentScorePreset_ThrowsInconsistentStatistics()
        {
            var demo = new Demo { FilePath = Path.Combine("TestData", @"auto0-20210103-190414-139014994-de_dust2-honigbiene_vs_waldfrosch.dem") };
            var match = new Match { Demo = demo, CTScore = 3, TScore = 0 };

            //Setting minimum number of rounds & players to 0 (different than during runtime) to allow reading this smaller demo file
            var demoReader = new DemoReader(match, 0, 0);
            demoReader.ReadHeader();

            Assert.Throws<InconsistentStatisticsException>(() => demoReader.Read());
        }

        [Fact]
        [Trait("Category", "unit")]
        public void Read_GameIsRestartedAfterMatchStarted_DoesNotCountEventsBeforeRealMatchStart()
        {
            var demo = new Demo { FilePath = Path.Combine("TestData", @"auto0-20210102-204730-1247575572-de_mirage-honigbiene_vs_waldfrosch.dem") };
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

        [Fact]
        [Trait("Category", "unit")]
        public void Read_OnlyFourRounds_CorrectStatistics()
        {
            var demo = new Demo { FilePath = Path.Combine("TestData", @"auto0-20201222-213144-349508145-de_inferno-honigbiene_vs_waldfrosch.dem") };
            var demoReader = new DemoReader(new Match { Demo = demo }, 0, 0);

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

        [Fact]
        [Trait("Category", "unit")]
        public void Read_GameIsRestartedAfterMatchStarted_DoesNotCountRoundsBeforeRealMatchStart()
        {
            var demo = new Demo { FilePath = Path.Combine("TestData", @"auto0-20210102-225615-1235223714-de_dust2-honigbiene_vs_waldfrosch.dem") };
            var demoReader = new DemoReader(new Match { Demo = demo });

            demoReader.ReadHeader();
            demoReader.Read();

            Assert.Equal(30, demoReader.Match.Rounds);
            foreach (var player in demoReader.Match.PlayerResults)
            {
                Assert.Equal(30, player.Rounds);
            }
        }
    }
}
