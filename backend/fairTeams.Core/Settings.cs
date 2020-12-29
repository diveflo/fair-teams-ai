namespace fairTeams.Core
{
    public static class Settings
    {
#pragma warning disable CA2211 // Non-constant fields should not be visible
        public static string SteamUsername = System.Environment.GetEnvironmentVariable("STEAM_USER");
        public static string SteamPassword = System.Environment.GetEnvironmentVariable("STEAM_PASSWORD");
#pragma warning restore CA2211 // Non-constant fields should not be visible
    }
}
