using backend.Rating;
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

                bot.Skill.AddRating(new DummyRating { Score = 0.0d });

                sortedByScore.Add(bot);
                var bestPlayer = sortedByScore[0];
                var secondBestPlayer = sortedByScore[1];
                sortedByScore[0] = secondBestPlayer;
                sortedByScore[1] = bestPlayer;
            }

            return GreedyAssigner(sortedByScore);
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
                player.Skill.AddRating(new DummyRating { Score = Double.MaxValue });
                player.ProfilePublic = false;
            }

            return player;
        }
    }
}