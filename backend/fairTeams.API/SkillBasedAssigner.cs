using Combinatorics.Collections;
using fairTeams.API.Rating;
using fairTeams.Core;
using fairTeams.Steamworks;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace fairTeams.API
{
    public class SkillBasedAssigner : ITeamAssigner
    {
        private readonly MatchRepository myMatchRepository;
        private readonly SteamworksApi mySteamworksApi;
        private readonly ILogger myLogger;
        private readonly Random myRandom;

        public SkillBasedAssigner(MatchRepository matchRepository, SteamworksApi steamworksApi, ILogger<SkillBasedAssigner> logger)
        {
            myMatchRepository = matchRepository;
            mySteamworksApi = steamworksApi;
            myLogger = logger;
            myRandom = new Random();
        }

        public SkillBasedAssigner(MatchRepository matchRepository, SteamworksApi steamworksApi) : this(matchRepository, steamworksApi, UnitTestLoggerCreator.CreateUnitTestLogger<SkillBasedAssigner>()) { }

        public Task<(Team terrorists, Team counterTerrorists)> GetAssignedPlayers(IEnumerable<Player> players)
        {
            return GetAssignedPlayers(players, true);
        }

        public async Task<(Team terrorists, Team counterTerrorists)> GetAssignedPlayers(IEnumerable<Player> players, bool includeBot)
        {
            var playersList = players.ToList();
            myLogger.LogInformation($"Computing optimal assignment for players: {string.Join(", ", playersList.Select(x => x.SteamName))}");

            for (var i = 0; i < playersList.Count; i++)
            {
                playersList[i] = await GetSkillLevel(playersList[i]);
            }

            if (includeBot)
            {
                playersList = BalanceTeamSizesWithBot(playersList);
            }

            (var terrorists, var counterTerrorists) = OptimalAssigner(playersList);
            terrorists.Players = terrorists.Players.OrderBy(x => x.Name).ToList();
            counterTerrorists.Players = counterTerrorists.Players.OrderBy(x => x.Name).ToList();

            return (terrorists, counterTerrorists);
        }

        private (Team terrorists, Team counterTerrorists) OptimalAssigner(IEnumerable<Player> players)
        {
            var playersList = players.ToList();
            var playersPerTeam = (int)Math.Ceiling(playersList.Count / 2.0d);

            var firstTeamCombinations = new Combinations<Player>(playersList, playersPerTeam);
            myLogger.LogInformation($"{firstTeamCombinations.Count} possible combinations of teams. Computing their skill differences.");

            var assignmentAndCost = new Dictionary<(Team, Team), double>((int)firstTeamCombinations.Count);
            foreach (var combination in firstTeamCombinations)
            {
                var terrorists = new Team("Terrorists")
                {
                    Players = combination
                };
                var counterTerrorists = new Team("CounterTerrorists")
                {
                    Players = playersList.Except(combination).ToList()
                };

                var skillDifference = GetSkillDifference(terrorists, counterTerrorists);

                if (!assignmentAndCost.Any(x => ScrambledEquals(x.Key.Item1.Players, counterTerrorists.Players)))
                {
                    assignmentAndCost.Add((terrorists, counterTerrorists), skillDifference);
                }
            }

            return GetRandomSelectionOfBestAssignments(assignmentAndCost);
        }

        private List<Player> BalanceTeamSizesWithBot(List<Player> players)
        {
            if (players.Count % 2 != 0)
            {
                var bot = new Player
                {
                    Name = "BOT",
                    SteamName = "BOT",
                    SteamID = "0",
                    Skill = new SkillLevel()
                };

                var averageScoreHumanPlayers = players.Average(x => x.Skill.SkillScore);
                var minimumScoreHumanPlayers = players.Min(x => x.Skill.SkillScore);
                var botScore = averageScoreHumanPlayers * 0.75;

                if (minimumScoreHumanPlayers <= botScore)
                {
                    botScore = 0.9 * minimumScoreHumanPlayers;
                }

                bot.Skill.SetRating(new DummyRating { Score = botScore });
                myLogger.LogInformation($"Balancing team sizes by adding bot with a score of {botScore}");

                players.Add(bot);
            }

            return players;
        }

        private static double GetSkillDifference(Team terrorists, Team counterTerrorists)
        {
            var averageSkillTerrorists = terrorists.Players.Sum(x => x.Skill.SkillScore);
            var averageSkillCounterTerrorists = counterTerrorists.Players.Sum(x => x.Skill.SkillScore);

            return Math.Abs(averageSkillTerrorists - averageSkillCounterTerrorists);
        }

        private async Task<Player> GetSkillLevel(Player player)
        {
            try
            {
                var hltvRating = Task.Run(() => new HLTVRating(long.Parse(player.SteamID), myMatchRepository));
                player.Skill.SetRating(await hltvRating);
                return player;
            }
            catch (NoMatchstatisticsFoundException)
            {
                myLogger.LogWarning($"Didn't find any matches for {player.Name} (Steam ID: {player.SteamID}). Trying overall K/D rating from Steam.");
            }

            try
            {
                var steamapiStatistics = await mySteamworksApi.ParsePlayerStatistics(player.SteamID);
                var kdRating = new KDRating(steamapiStatistics);
                player.Skill.SetRating(kdRating);
                return player;
            }
            catch (ProfileNotPublicException)
            {
                myLogger.LogWarning($"{player.Name}'s profile (Steam ID: {player.SteamID}) seems not to be public. Using dummy score!");
                player.Skill.SetRating(new DummyRating { Score = new Random().NextDouble() + 0.3 });
            }

            return player;
        }

        private (Team, Team) GetRandomSelectionOfBestAssignments(Dictionary<(Team, Team), double> assignmentsAndCosts)
        {
            var assignmentsWithAcceptableSkillDifference = assignmentsAndCosts.Where(x => x.Value <= 0.075);
            var numberOfAssignments = assignmentsWithAcceptableSkillDifference.Count();
            const int minimumNumberOfAssignments = 3;

            IEnumerable<KeyValuePair<(Team, Team), double>> selectedSubset;

            if (numberOfAssignments >= minimumNumberOfAssignments)
            {
                selectedSubset = assignmentsAndCosts.OrderBy(x => x.Value).Take(minimumNumberOfAssignments);

                myLogger.LogInformation($"Using subset of best {selectedSubset.Count()} assignments (skill difference max. 0.075)." +
                    $"Their skill-difference is {string.Join(",", selectedSubset.Select(x => x.Value))} respectively");
            }
            else
            {
                selectedSubset = assignmentsWithAcceptableSkillDifference;
                myLogger.LogInformation($"Using all possible assignments as there are so few.");
            }

            var indexOfAssignment = myRandom.Next(0, selectedSubset.Count());
            myLogger.LogTrace($"Generated random index for assignment selection: {indexOfAssignment}");

            var selectedAssignment = selectedSubset.ElementAt(indexOfAssignment);
            myLogger.LogInformation($"The selected teams have a skill difference of {selectedAssignment.Value}");

            return selectedAssignment.Key;
        }

        public static bool ScrambledEquals(IEnumerable<Player> first, IEnumerable<Player> second)
        {
            return Enumerable.SequenceEqual(first.OrderBy(x => x.Skill), second.OrderBy(x => x.Skill));
        }
    }
}