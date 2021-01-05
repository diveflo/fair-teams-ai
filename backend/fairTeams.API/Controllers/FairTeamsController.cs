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
            players = await SetSteamUsernames(players);

            (Team terrorists, Team counterTerrorists) = await myAssigner.GetAssignedPlayers(Convert(players));

            return new Assignment(terrorists, counterTerrorists);
        }

        private async Task<IEnumerable<RequestPlayer>> SetSteamUsernames(IEnumerable<RequestPlayer> players)
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

        private static IEnumerable<Player> Convert(IEnumerable<RequestPlayer> requestPlayers)
        {
            foreach (var requestPlayer in requestPlayers)
            {
                yield return new Player(requestPlayer);
            }
        }
    }
}