using System;

namespace fairTeams.Core
{
    public class MatchPlayer : IEquatable<MatchPlayer>
    {
        public long SteamID { get; set; }
        public string Name { get; set; }

        public bool Equals(MatchPlayer other)
        {
            return SteamID == other.SteamID;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as MatchPlayer);
        }

        public override int GetHashCode()
        {
            return SteamID.GetHashCode();
        }
    }
}
