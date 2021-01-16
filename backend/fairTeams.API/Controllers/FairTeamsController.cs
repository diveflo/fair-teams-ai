using fairTeams.DemoHandling;
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
        private readonly SteamUserRepository myUserRepository;
        private readonly ILogger myLogger;

        public FairTeamsController(ITeamAssigner teamAssigner, SteamworksApi steamworksApi, SteamUserRepository userRepository, ILogger<FairTeamsController> logger)
        {
            myAssigner = teamAssigner;
            mySteamworksApi = steamworksApi;
            myUserRepository = userRepository;
            myLogger = logger;
        }

        [HttpPost]
        public async Task<Assignment> GetAssignedTeams(IEnumerable<RequestPlayer> players, bool includeBot)
        {
            var extendedPlayers = await GetSteamUsernames(players);
            extendedPlayers = GetRanks(extendedPlayers);

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

        private IEnumerable<Player> GetRanks(IEnumerable<Player> players)
        {
            foreach (var player in players)
            {
                var storedPlayer = myUserRepository.SteamUsers.Find(long.Parse(player.SteamID));
                if (storedPlayer == null)
                {
                    player.Skill.Rank = Core.Rank.NotRanked;
                    continue;
                }

                player.Skill.Rank = storedPlayer.Rank;
            }

            return players;
        }
    }
}