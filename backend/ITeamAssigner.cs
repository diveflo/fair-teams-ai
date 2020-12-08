using System.Collections.Generic;

namespace backend
{
    public interface ITeamAssigner
    {
        public IList<Player> GetAssignedPlayers(IEnumerable<Player> players);
    }
}