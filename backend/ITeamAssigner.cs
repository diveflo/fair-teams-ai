using System.Collections.Generic;
using System.Threading.Tasks;

namespace backend
{
    public interface ITeamAssigner
    {
        Task<(Team terrorists, Team counterTerrorists)> GetAssignedPlayers(IEnumerable<Player> players);
    }
}