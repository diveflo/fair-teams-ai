using backend.Rating;
using Combinatorics.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace backend
{
    public enum SolverOptions
    {
        Greedy,
        Optimal
    }

    public class SkillBasedAssigner : ITeamAssigner
    {
        public async Task<(Team terrorists, Team counterTerrorists)> GetAssignedPlayers(IEnumerable<Player> players)
        {
            return await GetAssignedPlayers(players, SolverOptions.Optimal);
        }

        private static async Task<(Team terrorists, Team counterTerrorists)> GetAssignedPlayers(IEnumerable<Player> players, SolverOptions option)
        {
            var playersList = players.ToList();

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
                bot.Skill.AddRating(new DummyRating { Score = averageScoreHumanPlayers / 2.0 });

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
                    (terrorists, counterTerrorists) = OptimalAssigner(sortedByScore, 0.1);
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

        private static (Team terrorists, Team counterTerrorists) OptimalAssigner(IEnumerable<Player> players, double skillDifferenceCutoff)
        {
            var playersList = players.ToList();

            if (playersList.Count % 2 != 0)
            {
                throw new ArgumentException("The number of players has to be even!");
            }

            var playersPerTeam = playersList.Count / 2;

            var firstTeamCombinations = new Combinations<Player>(playersList, playersPerTeam);

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

            var assignmentsSortedBySkillDifference = assignmentAndCost.OrderBy(x => x.Value).TakeWhile(x => x.Value < skillDifferenceCutoff).ToList();
            var indexOfRandomlySelectedAssignment = new Random().Next(0, assignmentsSortedBySkillDifference.Count);

            return assignmentsSortedBySkillDifference.ElementAt(indexOfRandomlySelectedAssignment).Key;
        }

        private static double GetSkillDifference(Team terrorists, Team counterTerrorists)
        {
            var averageSkillTerrorists = terrorists.Players.Sum(x => x.Skill.SkillScore);
            var averageSkillCounterTerrorists = counterTerrorists.Players.Sum(x => x.Skill.SkillScore);

            return Math.Abs(averageSkillTerrorists - averageSkillCounterTerrorists);
        }

        private static async Task<Player> GetSkillLevel(Player player)
        {
            try
            {
                var playerStatistics = await SteamworksApi.SteamworksApi.ParsePlayerStatistics(player.SteamID);
                var kdRating = new KDRating(playerStatistics);
                player.Skill.AddRating(kdRating);
                player.ProfilePublic = true;
            }
            catch (ProfileNotPublicException)
            {
                player.Skill.AddRating(new DummyRating { Score = new Random().NextDouble() + 0.3 });
                player.ProfilePublic = false;
            }

            return player;
        }
    }
}