using System;
using System.Collections.Generic;

namespace fairTeams.DemoAnalyzer
{
    public class DemoReaderException : Exception
    {
        public DemoReaderException(string message) : base(message) { }
    }

    public class PlayerNotYetRegisteredException : DemoReaderException
    {
        public PlayerNotYetRegisteredException(long playerSteamID, IEnumerable<long> registeredPlayersSteamIDs)
            : base($"Player with steam id: {playerSteamID} not yet registered in PlayerResults. Registered players steam ids: {string.Join(", ", registeredPlayersSteamIDs)}") { }
    }

    public class InconsistentStatisticsException : DemoReaderException
    {
        public InconsistentStatisticsException(string message) : base(message) { }
    }
}
