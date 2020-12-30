using System.IO;

namespace fairTeams.Core
{
    public static class Settings
    {
        public static string SteamUsername => GetEnvironmentVariableMachineAndProcess("STEAM_USERNAME");
        public static string SteamPassword => GetEnvironmentVariableMachineAndProcess("STEAM_PASSWORD");
        public static string SteamWebAPIKey => GetEnvironmentVariableMachineAndProcess("STEAM_WEBAPI_KEY");
        public static string ApplicationFolder => Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData), "fairteamsai");

        private static string GetEnvironmentVariableMachineAndProcess(string environmentVariable)
        {
            var machine = System.Environment.GetEnvironmentVariable(environmentVariable, System.EnvironmentVariableTarget.Machine);
            var process = System.Environment.GetEnvironmentVariable(environmentVariable, System.EnvironmentVariableTarget.Process);
            if (string.IsNullOrEmpty(machine) && string.IsNullOrEmpty(process))
            {
                throw new System.Exception($"The environment variable {environmentVariable} is set neither on Machine or Process level");
            }

            return !string.IsNullOrEmpty(machine) ? machine : process;
        }
    }
}
