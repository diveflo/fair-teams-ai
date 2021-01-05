using DemoInfo;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace fairTeams.DemoAnalyzer
{
    public static class PlayerExtensions
    {
        public static bool IsBot(this Player player)
        {
            return player.SteamID == 0;
        }
    }

    public class SteamIdBasedPlayerEqualityComparer : IEqualityComparer<Player>
    {
        public bool Equals(Player x, Player y)
        {
            return x.SteamID == y.SteamID;
        }

        public int GetHashCode([DisallowNull] Player obj)
        {
            return obj.SteamID.GetHashCode();
        }
    }
}
