using fairTeams.Steamworks;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Threading.Tasks;

namespace fairTeams.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [EnableCors]
    public class PlayerController : ControllerBase
    {
        private readonly ITeamAssigner myAssigner;
        private readonly ILogger myLogger;

        public PlayerController(ITeamAssigner teamAssigner, ILogger<PlayerController> logger)
        {
            myAssigner = teamAssigner;
            myLogger = logger;
        }

        [HttpPost]
        public async Task<Assignment> GetAssignedTeams(IEnumerable<Player> players)
        {
            players = await SetSteamUsernames(players);
            (Team terrorists, Team counterTerrorists) = await myAssigner.GetAssignedPlayers(players);

            return new Assignment(terrorists, counterTerrorists);
        }

        private async Task<IEnumerable<Player>> SetSteamUsernames(IEnumerable<Player> players)
        {
            var steamIDsWithUsernames = await SteamworksApi.ParseSteamUsernames(players.Select(x => x.SteamID).ToList());

            foreach (var player in players)
            {
                player.SteamName = steamIDsWithUsernames.SingleOrDefault(x => x.Key == player.SteamID).Value;
            }

            foreach (var notFoundPlayer in players.Where(x => string.IsNullOrEmpty(x.SteamName)))
            {
                myLogger.LogWarning($"Player's {notFoundPlayer.Name} Steam ID ({notFoundPlayer.SteamID}) seems to be invalid.");
            }

            return players;
        }
    }
}