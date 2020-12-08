using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PlayerController : ControllerBase
    {
        private readonly ITeamAssigner myAssigner;
        public PlayerController()
        {
            myAssigner = new SkillBasedAssigner();
        }

        [HttpPost]
        public IList<Player> GetAssignedTeams(IEnumerable<Player> players)
        {
            foreach (var player in players)
            {
                player.ScrapeName();
            }

            return myAssigner.GetAssignedPlayers(players);
        }
    }
}