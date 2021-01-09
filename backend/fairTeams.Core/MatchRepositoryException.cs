using System;

namespace fairTeams.Core
{
    public class MatchRepositoryException : Exception
    {
        public MatchRepositoryException(string message) : base(message) { }
    }

    public class NoMatchstatisticsFoundException : MatchRepositoryException
    {
        public NoMatchstatisticsFoundException(long steamId) : base($"No match statistics found for steam ID: {steamId}") { }
    }
}
