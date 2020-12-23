using System.Collections.Generic;

namespace fairTeams.API
{
    public class Team
    {
        public string Name { get; set; }

        public IList<Player> Players { get; set; }

        public Team(string name)
        {
            Name = name;
            Players = new List<Player>();
        }
    }
}