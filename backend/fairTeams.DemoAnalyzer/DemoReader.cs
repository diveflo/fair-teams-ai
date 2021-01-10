using DemoInfo;
using fairTeams.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace fairTeams.DemoAnalyzer
{
    public class DemoReader : IDisposable
    {
        private readonly Demo myDemo;
        private FileStream myDemoFileStream;
        private DemoParser myDemoParser;
        private int myMinimumRounds;
        private int myMinimumPlayers;
        private readonly Dictionary<Player, int> myKillsThisRound;
        private bool myHasMatchStarted;

        public Match Match { get; }

        public DemoReader(Match match) : this(match, 15, 5) { }

        public DemoReader(Match match, int minimumRounds, int minimumPlayers)
        {
            Match = match;
            myDemo = match.Demo;
            myMinimumRounds = minimumRounds;
            myMinimumPlayers = minimumPlayers;

            myHasMatchStarted = false;
            myKillsThisRound = new Dictionary<Player, int>(new SteamIdBasedPlayerEqualityComparer());
        }

        public void ReadHeader()
        {
            myDemoFileStream = File.OpenRead(myDemo.FilePath);
            myDemoParser = new DemoParser(myDemoFileStream);

            myDemoParser.ParseHeader();
            Match.Map = myDemoParser.Map;
            Match.Id = myDemoParser.Header.MapName.Replace("/", "") + "_" + myDemoParser.Header.SignonLength + myDemoParser.Header.PlaybackTicks + myDemoParser.Header.PlaybackFrames;
            Match.Demo.Id = Match.Id;
        }

        public void Read()
        {
            myDemoParser.MatchStarted += HandleMatchStarted;
            myDemoParser.RoundAnnounceMatchStarted += HandleRoundAnnounceMatchStarted;
            myDemoParser.RoundStart += HandleRoundStarted;
            myDemoParser.PlayerKilled += HandlePlayerKilled;
            myDemoParser.RoundOfficiallyEnd += HandleRoundOfficiallyEnd;

            try
            {
                myDemoParser.ParseToEnd();
            }
            catch (Exception e)
            {
                throw new DemoReaderException($"Unexpected exception thrown during demo analysis: {e.Message}");
            }

            ParseFinalTeamScores();
            ProcessMissingLastRound();

            AssertMinimumRoundsAndPlayers();

            CheckResultConsistency();
        }

        // we clear the kill counts etc. additionally here because MatchStarted is only thrown once somehow
        // and doesn't correctly handle the case that the game is restarted (e.g. on our server)
        private void HandleRoundAnnounceMatchStarted(object sender, RoundAnnounceMatchStartedEventArgs e)
        {
            myKillsThisRound.Clear();
            Match.PlayerResults.Clear();
            Match.Rounds = 0;
        }

        private void HandleMatchStarted(object sender, MatchStartedEventArgs e)
        {
            myKillsThisRound.Clear();
            Match.PlayerResults.Clear();
            Match.Rounds = 0;

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

            // Race condition in DemoInfo library:
            // Sometimes PlayerKilledEventArgs.Killer is defined while adding +1 to the player's kills, but null when setting the multiple kill stats
            // --> Copy killer and victim here to ensure we don't run into inconsistencies
            if (e.Killer == null || e.Victim == null)
            {
                return;
            }
            var killer = e.Killer.Copy();
            var victim = e.Victim.Copy();

            if (!killer.IsBot())
            {
                EnsurePlayerRegistered(killer.SteamID);

                var killerWithStats = Match.PlayerResults.Single(x => x.SteamID == killer.SteamID);

                if (IsSuicide(killer, victim))
                {
                    killerWithStats.Kills -= 1;
                    killerWithStats.Deaths += 1;
                    return;
                }

                if (victim.IsBot())
                {
                    killerWithStats.Kills += 1;
                    AddKillToMultipleKillTracking(killer);
                    return;
                }

                EnsurePlayerRegistered(victim.SteamID);
                var victimWithStats = Match.PlayerResults.Single(x => x.SteamID == victim.SteamID);

                if (IsTeamkill(killer, victim))
                {
                    killerWithStats.Kills -= 1;
                    victimWithStats.Deaths += 1;
                    return;
                }

                killerWithStats.Kills += 1;
                victimWithStats.Deaths += 1;
                AddKillToMultipleKillTracking(killer);
            }
        }

        private static bool IsSuicide(Player killer, Player victim)
        {
            return killer.SteamID == victim.SteamID;
        }

        private static bool IsTeamkill(Player killer, Player victim)
        {
            return killer.Team == victim.Team;
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
            UpdateRounds();
        }

        private void UpdateKillCounts()
        {
            foreach (var kills in myKillsThisRound)
            {
                var player = kills.Key;
                var numberOfKills = kills.Value;

                if (player.IsBot())
                {
                    continue;
                }

                EnsurePlayerRegistered(player.SteamID);

                var playerMatchStatistics = Match.PlayerResults.Single(x => x.SteamID == player.SteamID);

                switch (numberOfKills)
                {
                    case 1:
                        playerMatchStatistics.OneKill += 1;
                        break;
                    case 2:
                        playerMatchStatistics.TwoKill += 1;
                        break;
                    case 3:
                        playerMatchStatistics.ThreeKill += 1;
                        break;
                    case 4:
                        playerMatchStatistics.FourKill += 1;
                        break;
                    case 5:
                        playerMatchStatistics.FiveKill += 1;
                        break;
                }
            }
        }

        private void UpdateRounds()
        {
            if (!myHasMatchStarted)
            {
                return;
            }

            Match.Rounds += 1;

            foreach (var playerResult in Match.PlayerResults)
            {
                playerResult.Rounds += 1;
            }
        }

        private void ParseFinalTeamScores()
        {
            // At the end of the game, the initial CT team is T side and vice versa
            if (Match.Rounds > 15)
            {
                Match.TScore = myDemoParser.CTScore;
                Match.CTScore = myDemoParser.TScore;
                return;
            }

            Match.CTScore = myDemoParser.CTScore;
            Match.TScore = myDemoParser.TScore;

        }

        private void ProcessNewPlayers()
        {
            var playingParticipantsWithoutBots = myDemoParser.PlayingParticipants.Where(x => !(x.SteamID == 0));
            var newPlayers = playingParticipantsWithoutBots.Where(x => !Match.PlayerResults.Select(x => x.SteamID).Contains(x.SteamID)).ToList();
            foreach (var newPlayer in newPlayers)
            {
                Match.PlayerResults.Add(new MatchStatistics { SteamID = newPlayer.SteamID, Id = $"{newPlayer.SteamID}_{Match.Id}" });
            }
        }

        private void ProcessMissingLastRound()
        {
            // RoundOfficiallyEnded event is more stable than RoundEnd but isn't fired for the last round of a match.
            // Hence, we process one more round if have a mismatch between our counted rounds and the sum of both teams scores.
            if (Match.Rounds != Match.CTScore + Match.TScore)
            {
                UpdateKillCounts();
                UpdateRounds();
            }
        }

        private void EnsurePlayerRegistered(long steamid)
        {
            if (!IsPlayerRegistered(steamid))
            {
                ProcessNewPlayers();
                if (!IsPlayerRegistered(steamid))
                {
                    throw new PlayerNotYetRegisteredException(steamid, Match.PlayerResults.Select(x => x.SteamID));
                }
            }
        }

        private bool IsPlayerRegistered(long steamid)
        {
            return Match.PlayerResults.Any(x => x.SteamID == steamid);
        }

        private void CheckResultConsistency()
        {
            CheckNumberOfRoundsConsistency();
            CheckNumberOfKillsConsistency();
        }

        private void CheckNumberOfRoundsConsistency()
        {
            foreach (var player in Match.PlayerResults)
            {
                if (player.Rounds != Match.Rounds)
                {
                    throw new InconsistentStatisticsException($"Number of rounds differs between Match ({Match.Rounds}) and player ({player.Rounds}) with steam id {player.SteamID}");
                }
            }
        }

        // The overall number of kills can be lower than the sum of the multiple kill statistic (b.c. of teamkills, suicide) but not the other way round
        private void CheckNumberOfKillsConsistency()
        {
            foreach (var player in Match.PlayerResults)
            {
                var sumOfKills = player.OneKill + 2 * player.TwoKill + 3 * player.ThreeKill + 4 * player.FourKill + 5 * player.FiveKill;
                if (player.Kills > sumOfKills)
                {
                    throw new InconsistentStatisticsException($"The sum of the multiple kill statistic ({sumOfKills}) must be larger-than or equal" +
                        $"to the overall number of kills ({player.Kills}) for player with steamid: {player.SteamID}");
                }
            }
        }

        private void AssertMinimumRoundsAndPlayers()
        {
            if (Match.Rounds < myMinimumRounds)
            {
                throw new TooFewRoundsException(myMinimumRounds, Match.Rounds);
            }

            if (Match.PlayerResults.Count < myMinimumPlayers)
            {
                throw new TooFewPlayersException(myMinimumPlayers, Match.PlayerResults.Count);
            }
        }

        public void Dispose()
        {
            myDemoParser.MatchStarted -= HandleMatchStarted;
            myDemoParser.RoundStart -= HandleRoundStarted;
            myDemoParser.PlayerKilled -= HandlePlayerKilled;
            myDemoParser.RoundOfficiallyEnd -= HandleRoundOfficiallyEnd;
            myDemoParser.Dispose();

            myDemoFileStream.Dispose();

            myDemoParser = null;
        }
    }
}
