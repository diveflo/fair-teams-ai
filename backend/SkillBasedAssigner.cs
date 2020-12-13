using backend.Rating;
using System;
using System.Collections.Generic;
using System.Linq;

namespace backend
{
    public class SkillBasedAssigner : ITeamAssigner
    {
        (Team terrorists, Team counterTerrorists) ITeamAssigner.GetAssignedPlayers(IEnumerable<Player> players)
        {
            foreach (var player in players)
            {
                GetSkillLevel(player);
            }

            var sortedByScore = players.OrderByDescending(x => x.Skill.SkillScore).ToList();

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

        private static void GetSkillLevel(Player player)
        {
            try
            {
                var kdRating = new KDRating(player.SteamID);
                player.Skill.AddRating(kdRating);
                player.ProfilePublic = true;
            }
            catch (ProfileNotPublicException)
            {
                player.Skill.AddRating(new DummyRating { Score = Double.MaxValue });
                player.ProfilePublic = false;
            }
        }
    }
}