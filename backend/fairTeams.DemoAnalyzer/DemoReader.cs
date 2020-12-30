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
        private readonly Dictionary<Player, int> myKillsThisRound;
        private bool myHasMatchStarted;

        public Match Match { get; }

        public DemoReader(Demo demo)
        {
            myDemo = demo;
            myHasMatchStarted = false;
            myKillsThisRound = new Dictionary<Player, int>();

            Match = new Match { Demo = myDemo };
        }

        public void Read()
        {
            using var fileStream = File.OpenRead(myDemo.FilePath);
            myDemoParser = new DemoInfo.DemoParser(fileStream);

            myDemoParser.ParseHeader();
            Match.Map = myDemoParser.Map;
            Match.Id = myDemoParser.Header.MapName.Replace("/", "") + "_" + myDemoParser.Header.SignonLength + myDemoParser.Header.PlaybackTicks + myDemoParser.Header.PlaybackFrames;
            Match.Demo.Id = Match.Id;

            myDemoParser.MatchStarted += HandleMatchStarted;
            myDemoParser.RoundStart += HandleRoundStarted;
            myDemoParser.PlayerKilled += HandlePlayerKilled;
            myDemoParser.RoundOfficiallyEnd += HandleRoundOfficiallyEnd;

            myDemoParser.ParseToEnd();

            ParseFinalTeamScores();
            ProcessMissingLastRound();
        }

        private void HandleMatchStarted(object sender, MatchStartedEventArgs e)
        {
            myKillsThisRound.Clear();
            Match.PlayerResults.Clear();

            myHasMatchStarted = true;

            ProcessNewPlayers();
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
            if (!myHasMatchStarted)
            {
                return;
            }

            if (e.Killer != null && !e.Killer.IsBot())
            {
                var killerWithStats = Match.PlayerResults.Single(x => x.SteamID == e.Killer.SteamID);

                if (e.IsSuicide())
                {
                    killerWithStats.Kills -= 1;
                    killerWithStats.Deaths += 1;
                    return;
                }

                if (e.Victim.IsBot())
                {
                    killerWithStats.Kills += 1;
                    AddKillToMultipleKillTracking(e.Killer);
                    return;
                }

                var victimWithStats = Match.PlayerResults.Single(x => x.SteamID == e.Victim.SteamID);

                if (e.IsTeamkill())
                {
                    killerWithStats.Kills -= 1;
                    victimWithStats.Deaths += 1;
                    return;
                }

                killerWithStats.Kills += 1;
                victimWithStats.Deaths += 1;
                AddKillToMultipleKillTracking(e.Killer);
            }
        }

        private void AddKillToMultipleKillTracking(Player killer)
        {
            if (!myKillsThisRound.ContainsKey(killer))
            {
                myKillsThisRound[killer] = 0;
            }

            myKillsThisRound[killer]++;
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

                var correspondingMatchPlayer = Match.PlayerResults.Single(x => x.SteamID == player.SteamID);

                switch (numberOfKills)
                {
                    case 1:
                        correspondingMatchPlayer.OneKill += 1;
                        break;
                    case 2:
                        correspondingMatchPlayer.TwoKill += 1;
                        break;
                    case 3:
                        correspondingMatchPlayer.ThreeKill += 1;
                        break;
                    case 4:
                        correspondingMatchPlayer.FourKill += 1;
                        break;
                    case 5:
                        correspondingMatchPlayer.FiveKill += 1;
                        break;
                }
            }

            foreach (var playerResult in Match.PlayerResults)
            {
                playerResult.Rounds += 1;
            }
        }

        private void ParseFinalTeamScores()
        {
            // At the end of the game, the initial CT team is T side and vice versa
            Match.TScore = myDemoParser.CTScore;
            Match.CTScore = myDemoParser.TScore;
        }

        private void ProcessNewPlayers()
        {
            var playingParticipantsWithoutBots = myDemoParser.PlayingParticipants.Where(x => !(x.SteamID == 0));
            var newPlayers = playingParticipantsWithoutBots.Where(x => !Match.PlayerResults.Select(x => x.SteamID).Contains(x.SteamID)).ToList();
            foreach (var newPlayer in newPlayers)
            {
                Match.PlayerResults.Add(new MatchStatistics { SteamID = newPlayer.SteamID, Id = $"{newPlayer.SteamID}_{Match.Id}"});
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
