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
            var hltvRating = new HLTVRating();
            hltvRating.ScrapeForPlayer(player.SteamID);
            player.Skill.AddRating(hltvRating);
        }
    }
}