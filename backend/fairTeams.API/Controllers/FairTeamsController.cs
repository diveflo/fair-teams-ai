﻿using fairTeams.Steamworks;
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
        private readonly ILogger myLogger;

        public FairTeamsController(ITeamAssigner teamAssigner, ILogger<FairTeamsController> logger)
        {
            myAssigner = teamAssigner;
            myLogger = logger;
        }

        [HttpPost]
        public async Task<Assignment> GetAssignedTeams(IEnumerable<RequestPlayer> players)
        {
            var extendedPlayers = await GetSteamUsernames(players);

            (Team terrorists, Team counterTerrorists) = await myAssigner.GetAssignedPlayers(extendedPlayers);

            return new Assignment(terrorists, counterTerrorists);
        }

        private async Task<IEnumerable<Player>> GetSteamUsernames(IEnumerable<RequestPlayer> players)
        {
            var steamIDsWithUsernames = await SteamworksApi.ParseSteamUsernames(players.Select(x => x.SteamID).ToList());
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