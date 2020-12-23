using System;

namespace fairTeams.API.SteamworksApi
{
    public class PlayerNotFoundException : Exception
    {
        public PlayerNotFoundException(string message) : base(message)
        {
        }
    }
}
