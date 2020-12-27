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
                var killerWithStats = Match.PlayerResults.Single(x => x.Key.SteamID == e.Killer.SteamID);

                if (e.IsSuicide())
                {
                    killerWithStats.Value.Kills -= 1;
                    killerWithStats.Value.Deaths += 1;
                    return;
                }

                if (e.Victim.IsBot())
                {
                    killerWithStats.Value.Kills += 1;
                    AddKillToMultipleKillTracking(e.Killer);
                    return;
                }

                var victimWithStats = Match.PlayerResults.Single(x => x.Key.SteamID == e.Victim.SteamID);

                if (e.IsTeamkill())
                {
                    killerWithStats.Value.Kills -= 1;
                    victimWithStats.Value.Deaths += 1;
                    return;
                }

                killerWithStats.Value.Kills += 1;
                victimWithStats.Value.Deaths += 1;
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

        private void ParseFinalTeamScores()
        {
            // At the end of the game, the initial CT team is T side and vice versa
            Match.TScore = myDemoParser.CTScore;
            Match.CTScore = myDemoParser.TScore;
        }

        private void ProcessNewPlayers()
        {
            var playingParticipantsWithoutBots = myDemoParser.PlayingParticipants.Where(x => !(x.SteamID == 0));
            var newPlayers = playingParticipantsWithoutBots.Where(x => !Match.PlayerResults.Keys.Select(x => x.SteamID).Contains(x.SteamID)).ToList();
            foreach (var newPlayer in newPlayers)
            {
                var matchPlayer = new MatchPlayer { SteamID = newPlayer.SteamID, Name = newPlayer.Name };
                Match.PlayerResults.Add(matchPlayer, new MatchStatistics());
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
