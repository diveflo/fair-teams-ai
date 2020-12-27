using System;

namespace fairTeams.Steamworks
{
    public class PlayerNotFoundException : Exception
    {
        public PlayerNotFoundException(string message) : base(message)
        {
        }
    }
}
