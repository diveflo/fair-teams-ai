using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

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
            foreach (var player in players)
            {
                player.ScrapeName();
            }

            (Team terrorists, Team counterTerrorists) = myAssigner.GetAssignedPlayers(players);
            return new Assignment(terrorists, counterTerrorists);
        }
    }
}