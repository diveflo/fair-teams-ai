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

    public static class PlayerKilledEventArgsExtensions
    {
        public static bool IsSuicide(this PlayerKilledEventArgs eventArgs)
        {
            return eventArgs.Killer.SteamID == eventArgs.Victim.SteamID;
        }

        public static bool IsTeamkill(this PlayerKilledEventArgs eventArgs)
        {
            return eventArgs.Killer.Team == eventArgs.Victim.Team;
        }
    }
}
