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

    public class TooFewRoundsException : DemoReaderException
    {
        public TooFewRoundsException(int minimumNumberOfRounds, int parsedNumberOfRounds)
            : base($"The parsed demo file only contained {parsedNumberOfRounds} rounds but we expected a minimum of {minimumNumberOfRounds}. Ignoring demo file.") { }
    }

    public class TooFewPlayersException : DemoReaderException
    {
        public TooFewPlayersException(int minimumNumberOfPlayers, int parsedNumberOfPlayers)
            : base($"The parsed demo file only contained {parsedNumberOfPlayers} players but we expected a minimum of {minimumNumberOfPlayers}. Ignoring demo file.") { }
    }
}
