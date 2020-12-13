using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;

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
        public Assignment GetAssignedTeams(IEnumerable<Player> players)
        {
            var playersWithSteamNames = SteamworksApi.SteamworksApi.ParseSteamUsernames(players.ToList());
            (Team terrorists, Team counterTerrorists) = myAssigner.GetAssignedPlayers(playersWithSteamNames);
            return new Assignment(terrorists, counterTerrorists);
        }
    }
}