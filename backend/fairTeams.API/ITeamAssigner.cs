using System.Collections.Generic;
using System.Threading.Tasks;

namespace fairTeams.API
{
    public interface ITeamAssigner
    {
        Task<(Team terrorists, Team counterTerrorists)> GetAssignedPlayers(IEnumerable<Player> players);
        Task<(Team terrorists, Team counterTerrorists)> GetAssignedPlayers(IEnumerable<Player> players, bool includeBot);
    }
}