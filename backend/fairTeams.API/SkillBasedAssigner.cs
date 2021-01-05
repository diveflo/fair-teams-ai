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
    public enum SolverOptions
    {
        Greedy,
        Optimal
    }

    public class SkillBasedAssigner : ITeamAssigner
    {
        private readonly MatchRepository myMatchRepository;
        private readonly ILogger myLogger;

        public SkillBasedAssigner(MatchRepository matchRepository, ILogger<SkillBasedAssigner> logger)
        {
            myMatchRepository = matchRepository;
            myLogger = logger;
        }

        public SkillBasedAssigner(MatchRepository matchRepository) : this(matchRepository, UnitTestLoggerCreator.CreateUnitTestLogger<SkillBasedAssigner>()) { }

        public async Task<(Team terrorists, Team counterTerrorists)> GetAssignedPlayers(IEnumerable<Player> players)
        {
            return await GetAssignedPlayers(players, SolverOptions.Optimal);
        }

        private async Task<(Team terrorists, Team counterTerrorists)> GetAssignedPlayers(IEnumerable<Player> players, SolverOptions option)
        {
            var playersList = players.ToList();
            myLogger.LogInformation($"Computing optimal assignment for players: {string.Join(", ", playersList.Select(x => x.SteamName))}");

            for (var i = 0; i < playersList.Count; i++)
            {
                playersList[i] = await GetSkillLevel(playersList[i]);
            }

            var sortedByScore = playersList.OrderByDescending(x => x.Skill.SkillScore).ToList();

            if (sortedByScore.Count % 2 != 0)
            {
                var bot = new Player
                {
                    Name = "BOT",
                    SteamName = "BOT",
                    ProfilePublic = true,
                    SteamID = "1",
                    Skill = new SkillLevel()
                };

                var averageScoreHumanPlayers = sortedByScore.Average(x => x.Skill.SkillScore);
                var minimumScoreHumanPlayers = sortedByScore.Min(x => x.Skill.SkillScore);
                var botScore = averageScoreHumanPlayers / 2.0;

                if (minimumScoreHumanPlayers <= botScore)
                {
                    botScore = 0.9 * minimumScoreHumanPlayers;
                }

                bot.Skill.AddRating(new DummyRating { Score = botScore });
                myLogger.LogInformation($"Balancing team sizes by adding bot with a score of {botScore}");

                sortedByScore.Add(bot);
                var bestPlayer = sortedByScore[0];
                var secondBestPlayer = sortedByScore[1];
                sortedByScore[0] = secondBestPlayer;
                sortedByScore[1] = bestPlayer;
            }

            var terrorists = new Team("Terrorists");
            var counterTerrorists = new Team("CounterTerrorists");

            switch (option)
            {
                case SolverOptions.Optimal:
                    (terrorists, counterTerrorists) = OptimalAssigner(sortedByScore);
                    break;
                case SolverOptions.Greedy:
                    (terrorists, counterTerrorists) = GreedyAssigner(sortedByScore);
                    break;
            }

            return (terrorists, counterTerrorists);
        }

        private static (Team terrorists, Team counterTerrorists) GreedyAssigner(IEnumerable<Player> playersSortedByScore)
        {
            var sortedByScore = playersSortedByScore.ToList();

            var bestPlayerIsTerrorist = new Random().NextDouble() < 0.5;

            var terrorists = new Team("Terrorists");
            var counterTerrorists = new Team("CounterTerrorists");

            for (var i = 0; i < sortedByScore.Count; i++)
            {
                var currentPlayer = sortedByScore[i];

                if (i % 2 == 0)
                {
                    if (bestPlayerIsTerrorist)
                    {
                        terrorists.Players.Add(currentPlayer);
                    }
                    else
                    {
                        counterTerrorists.Players.Add(currentPlayer);
                    }
                }
                else
                {
                    if (bestPlayerIsTerrorist)
                    {
                        counterTerrorists.Players.Add(currentPlayer);
                    }
                    else
                    {
                        terrorists.Players.Add(currentPlayer);
                    }
                }
            }

            return (terrorists, counterTerrorists);
        }

        private (Team terrorists, Team counterTerrorists) OptimalAssigner(IEnumerable<Player> players)
        {
            var playersList = players.ToList();

            if (playersList.Count % 2 != 0)
            {
                throw new ArgumentException("The number of players has to be even!");
            }

            var playersPerTeam = playersList.Count / 2;

            var firstTeamCombinations = new Combinations<Player>(playersList, playersPerTeam);
            myLogger.LogInformation($"{firstTeamCombinations.Count} possible combinations of teams. Computing their skill differences.");

            var assignmentAndCost = new Dictionary<(Team, Team), double>((int)firstTeamCombinations.Count);
            Parallel.ForEach(firstTeamCombinations, combination =>
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

                assignmentAndCost.Add((terrorists, counterTerrorists), skillDifference);
            });

            var smallSubsetOfOptimalAssinments = GetSmallSubsetOfBestAssignments(assignmentAndCost);
            return GetRandomlySelectedAssignment(smallSubsetOfOptimalAssinments);
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
                player.Skill.AddRating(await hltvRating);
                player.ProfilePublic = true;
            }
            catch (ProfileNotPublicException)
            {
                myLogger.LogWarning($"{player.Name}'s profile (Steam ID: {player.SteamID}) seems not to be public. Using dummy score!");

                player.Skill.AddRating(new DummyRating { Score = new Random().NextDouble() + 0.3 });
                player.ProfilePublic = false;
            }

            return player;
        }

        private List<(Team, Team)> GetSmallSubsetOfBestAssignments(Dictionary<(Team, Team), double> assignmentsAndCosts)
        {
            var orderedByCosts = assignmentsAndCosts.OrderBy(x => x.Value);
            var numberOfAssignments = orderedByCosts.Count();
            const int minimumNumberOfAssignments = 3;

            if (numberOfAssignments >= minimumNumberOfAssignments)
            {
                myLogger.LogInformation($"Using subset of best {minimumNumberOfAssignments} (hard-coded value!) assignments.");
                return orderedByCosts.Take(minimumNumberOfAssignments).Select(x => x.Key).ToList();
            }

            myLogger.LogInformation($"Using all possible assignments as there are so few.");

            return orderedByCosts.Take(numberOfAssignments).Select(x => x.Key).ToList();
        }

        private (Team, Team) GetRandomlySelectedAssignment(List<(Team, Team)> smallSubsetOfOptimalAssignments)
        {
            myLogger.LogInformation("Returning randomly selected assignment.");
            var indexOfAssignment = new Random().Next(0, smallSubsetOfOptimalAssignments.Count);
            return smallSubsetOfOptimalAssignments.ElementAt(indexOfAssignment);
        }
    }
}