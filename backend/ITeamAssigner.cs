using System.Collections.Generic;

namespace backend
{
    public interface ITeamAssigner
    {
        (Team terrorists, Team counterTerrorists) GetAssignedPlayers(IEnumerable<Player> players);
    }
}