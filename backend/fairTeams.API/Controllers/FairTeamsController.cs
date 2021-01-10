using fairTeams.Steamworks;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace fairTeams.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [EnableCors]
    public class FairTeamsController : ControllerBase
    {
        private readonly ITeamAssigner myAssigner;
        private readonly SteamworksApi mySteamworksApi;
        private readonly ILogger myLogger;

        public FairTeamsController(ITeamAssigner teamAssigner, SteamworksApi steamworksApi, ILogger<FairTeamsController> logger)
        {
            myAssigner = teamAssigner;
            mySteamworksApi = steamworksApi;
            myLogger = logger;
        }

        [HttpPost]
        public async Task<Assignment> GetAssignedTeams(IEnumerable<RequestPlayer> players, bool includeBot)
        {
            var extendedPlayers = await GetSteamUsernames(players);

            (Team terrorists, Team counterTerrorists) = await myAssigner.GetAssignedPlayers(extendedPlayers, includeBot);

            return new Assignment(terrorists, counterTerrorists);
        }

        private async Task<IEnumerable<Player>> GetSteamUsernames(IEnumerable<RequestPlayer> players)
        {
            var steamIDsWithUsernames = await mySteamworksApi.ParseSteamUsernames(players.Select(x => x.SteamID).ToList());
            var extendedPlayers = new List<Player>();

            foreach (var player in players)
            {
                var steamUsername = steamIDsWithUsernames.SingleOrDefault(x => x.Key == player.SteamID).Value;
                extendedPlayers.Add(new Player(player) { SteamName = steamUsername });
            }

            foreach (var notFoundPlayer in extendedPlayers.Where(x => string.IsNullOrEmpty(x.SteamName)))
            {
                myLogger.LogWarning($"Player's {notFoundPlayer.Name} Steam ID ({notFoundPlayer.SteamID}) seems to be invalid.");
            }

            return extendedPlayers;
        }
    }
}