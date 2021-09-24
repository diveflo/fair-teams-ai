using DemoInfo;
using fairTeams.Core;
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

    public static class DemoInfoExtensions
    {
        public static Side ToSide(this Team team)
        {
            switch (team)
            {
                case Team.CounterTerrorist:
                    return Side.CounterTerrorists;
                case Team.Terrorist:
                    return Side.Terrorists;
                case Team.Spectate:
                default:
                    throw new DemoReaderException($"Could not convert DemoInfo team: {team} to Side");
            }
        }
    }
}
