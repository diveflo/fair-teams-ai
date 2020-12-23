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
    public class PlayerController : ControllerBase
    {
        private readonly ITeamAssigner myAssigner;
        private readonly ILogger myLogger;

        public PlayerController(ILogger<PlayerController> logger)
        {
            myAssigner = new SkillBasedAssigner();
            myLogger = logger;
        }

        [HttpPost]
        public async Task<Assignment> GetAssignedTeams(IEnumerable<Player> players)
        {
            var playersWithSteamNames = await SteamworksApi.SteamworksApi.ParseSteamUsernames(players.ToList());
            (Team terrorists, Team counterTerrorists) = await myAssigner.GetAssignedPlayers(playersWithSteamNames);
            return new Assignment(terrorists, counterTerrorists);
        }
    }
}