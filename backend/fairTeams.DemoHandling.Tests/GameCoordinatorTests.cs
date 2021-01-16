using fairTeams.Core;
using System.Collections.Generic;
using Xunit;

namespace fairTeams.DemoHandling.Tests
{
    public class GameCoordinatorTests
    {
        [Fact (Skip = "The match/demo link will expire at some point")]
        public void GetMatchInfo_ShareCodeInput_ReturnsCorrectDateAndDownloadURL()
        {
            var gameCoordinatorClient = new GameCoordinatorClient();
            var gameRequest = ShareCodeDecoder.Decode("CSGO-j6hrT-hvqmd-pNMXY-TuTrq-aXnMC");
            var demo = new Demo { GameRequest = gameRequest };

            var match = gameCoordinatorClient.GetMatchInfo(demo);

            var expectedMatchDate = new System.DateTime(637451358060000000);
            var expectedDownloadURL = @"http://replay191.valve.net/730/003456465718474703287_0558788749.dem.bz2";

            Assert.Equal(expectedMatchDate, match.Date);
            Assert.Equal(expectedDownloadURL, match.Demo.DownloadURL);
            Assert.Equal(13, match.TScore);
            Assert.Equal(16, match.CTScore);
            Assert.Equal(29, match.Rounds);
        }

        [Fact (Skip = "The match/demo links will expire at some point")]
        public void GetMatchInfo_MultipleCallsSameObject_ReturnsCorrectDateAndDownloadURL()
        {
            var gameCoordinatorClient = new GameCoordinatorClient();
            var shareCodes = new List<string> {
                "CSGO-6FfJt-Fkyyu-8VFij-h5TDt-osMHA",
                "CSGO-FmnKB-V88mu-kidjO-bCxAj-c49oF",
                "CSGO-V6a3d-CjTH6-mqZGt-6mVw8-DKoJN",
                "CSGO-6ZQsb-hVSm7-qL7KM-mS9FQ-MwYDJ",
                "CSGO-6xbsY-7zSsm-ewtnA-hTxns-bCyDK"
            };

            var matches = new List<Match>();

            foreach (var shareCode in shareCodes)
            {
                var request = ShareCodeDecoder.Decode(shareCode);

                var demo = new Demo { GameRequest = request };
                try
                {
                    var match = gameCoordinatorClient.GetMatchInfo(demo);
                    matches.Add(match);
                }
                catch (GameCoordinatorException e)
                {
                    System.Console.WriteLine($"{e.Message}");
                    continue;
                }
            }

            Assert.Equal(2, matches.Count);
        }

        [Fact (Skip = "Obviously this isn't stable...")]
        public void GetRank_GoldNovaI_GoldNovaI()
        {
            var gameCoordinatorClient = new GameCoordinatorClient();
            var steamId = 76561197973591119;

            var rank = gameCoordinatorClient.GetRank(steamId);

            Assert.Equal(Rank.GoldNovaI, rank);
        }
    }
}
