using DemoInfo;
using fairTeams.Core;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace fairTeams.DemoAnalyzer
{
    public class DemoReader
    {
        private readonly Demo myDemo;
        private DemoParser myDemoParser;
        private readonly IList<Player> myIngamePlayers;
        private readonly Dictionary<Player, int> myKillsThisRound;
        private bool myHasMatchStarted;

        public Match Match { get; }

        public DemoReader(Demo demo)
        {
            myDemo = demo;
            myHasMatchStarted = false;
            myKillsThisRound = new Dictionary<Player, int>();
            myIngamePlayers = new List<Player>();

            Match = new Match();
        }

        public void Read()
        {
            using var fileStream = File.OpenRead(myDemo.FilePath);
            myDemoParser = new DemoInfo.DemoParser(fileStream);

            myDemoParser.ParseHeader();
            Match.Map = myDemoParser.Map;

            myDemoParser.MatchStarted += HandleMatchStarted;
            myDemoParser.RoundStart += HandleRoundStarted;
            myDemoParser.PlayerKilled += HandlePlayerKilled;
            myDemoParser.RoundOfficiallyEnd += HandleRoundOfficiallyEnd;

            myDemoParser.ParseToEnd();

            ParseOverallStatistics();
            ProcessMissingLastRound();
        }

        private void HandleMatchStarted(object sender, MatchStartedEventArgs e)
        {
            myKillsThisRound.Clear();
            Match.PlayerResults.Clear();
            myIngamePlayers.Clear();

            myHasMatchStarted = true;

            foreach (var playingPlayer in myDemoParser.PlayingParticipants)
            {
                var matchPlayer = new MatchPlayer { SteamID = playingPlayer.SteamID, Name = playingPlayer.Name };
                Match.PlayerResults.Add(matchPlayer, new MatchStatistics());
                myIngamePlayers.Add(playingPlayer);
            }
        }

        private void HandleRoundStarted(object sender, RoundStartedEventArgs e)
        {
            if (!myHasMatchStarted)
            {
                return;
            }

            ProcessNewPlayers();

            myKillsThisRound.Clear();
        }

        private void HandlePlayerKilled(object sender, PlayerKilledEventArgs e)
        {
            if (e.Killer != null)
            {
                if (!myKillsThisRound.ContainsKey(e.Killer))
                {
                    myKillsThisRound[e.Killer] = 0;
                }

                myKillsThisRound[e.Killer]++;
            }
        }

        private void HandleRoundOfficiallyEnd(object sender, RoundOfficiallyEndedEventArgs e)
        {
            if (!myHasMatchStarted)
            {
                return;
            }

            ProcessNewPlayers();

            UpdateKillCounts();
            Match.Rounds += 1;
        }

        private void UpdateKillCounts()
        {
            foreach (var kills in myKillsThisRound)
            {
                var player = kills.Key;
                var numberOfKills = kills.Value;

                var correspondingMatchPlayer = Match.PlayerResults.Single(x => x.Key.SteamID == player.SteamID);

                switch (numberOfKills)
                {
                    case 1:
                        Match.PlayerResults[correspondingMatchPlayer.Key].MultipleKills.OneKill += 1;
                        break;
                    case 2:
                        Match.PlayerResults[correspondingMatchPlayer.Key].MultipleKills.TwoKill += 1;
                        break;
                    case 3:
                        Match.PlayerResults[correspondingMatchPlayer.Key].MultipleKills.ThreeKill += 1;
                        break;
                    case 4:
                        Match.PlayerResults[correspondingMatchPlayer.Key].MultipleKills.FourKill += 1;
                        break;
                    case 5:
                        Match.PlayerResults[correspondingMatchPlayer.Key].MultipleKills.FiveKill += 1;
                        break;
                }
            }

            foreach (var playerResult in Match.PlayerResults)
            {
                playerResult.Value.Rounds += 1;
            }
        }

        private void ParseOverallStatistics()
        {
            // At the end of the game, the initial CT team is T side and vice versa
            Match.TScore = myDemoParser.CTScore;
            Match.CTScore = myDemoParser.TScore;

            foreach (var player in myIngamePlayers)
            {
                var correspondingMatchPlayer = Match.PlayerResults.Single(x => x.Key.SteamID == player.SteamID);

                Match.PlayerResults[correspondingMatchPlayer.Key].Kills = player.AdditionaInformations.Kills;
                Match.PlayerResults[correspondingMatchPlayer.Key].Deaths = player.AdditionaInformations.Deaths;
            }
        }

        private void ProcessNewPlayers()
        {
            var playingParticipantsWithoutBots = myDemoParser.PlayingParticipants.Where(x => !(x.SteamID == 0));
            var newPlayers = playingParticipantsWithoutBots.Where(x => !Match.PlayerResults.Keys.Select(x => x.SteamID).Contains(x.SteamID)).ToList();
            foreach (var newPlayer in newPlayers)
            {
                var matchPlayer = new MatchPlayer { SteamID = newPlayer.SteamID, Name = newPlayer.Name };
                Match.PlayerResults.Add(matchPlayer, new MatchStatistics());
                myIngamePlayers.Add(newPlayer);
            }
        }

        private void ProcessMissingLastRound()
        {
            if (Match.Rounds != Match.CTScore + Match.TScore)
            {
                UpdateKillCounts();
                Match.Rounds += 1;
            }
        }
    }
}
