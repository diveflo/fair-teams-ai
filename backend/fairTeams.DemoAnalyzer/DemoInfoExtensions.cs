using DemoInfo;

namespace fairTeams.DemoAnalyzer
{
    public static class PlayerExtensions
    {
        public static bool IsBot(this Player player)
        {
            return player.SteamID == 0;
        }
    }
}
