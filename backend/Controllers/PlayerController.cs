using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace backend.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [EnableCors]
    public class PlayerController : ControllerBase
    {
        private readonly ITeamAssigner myAssigner;
        public PlayerController()
        {
            myAssigner = new SkillBasedAssigner();
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