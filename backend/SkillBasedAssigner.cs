using System;
using System.Collections.Generic;
using System.Linq;
using backend.Rating;

namespace backend
{
    public class SkillBasedAssigner : ITeamAssigner
    {
        IList<Player> ITeamAssigner.GetAssignedPlayers(IEnumerable<Player> players)
        {
            foreach (var player in players)
            {
                GetSkillLevel(player);
            }

            var sortedByScore = players.OrderByDescending(x => x.Skill.SkillScore).ToList();
            var bestPlayerIsTerrorist = new Random().NextDouble() < 0.5;

            for (var i = 0; i < sortedByScore.Count; i++)
            {
                var currentPlayer = sortedByScore[i];

                if (i % 2 == 0)
                {
                    currentPlayer.Team = bestPlayerIsTerrorist ? Team.Terrorists : Team.CounterTerrorists;
                }
                else
                {
                    currentPlayer.Team = bestPlayerIsTerrorist ? Team.CounterTerrorists : Team.Terrorists;
                }
            }

            return sortedByScore;
        }

        private static void GetSkillLevel(Player player)
        {
            var hltvRating = new HLTVRating();
            hltvRating.ScrapeForPlayer(player.SteamID);
            player.Skill.AddRating(hltvRating);
        }
    }
}