namespace fairTeams.DemoHandling
{
    /// <summary>
    /// Request object for RequestGame
    /// </summary>
    public class GameRequest
    {
        /// <summary>
        /// UNKNOWN
        /// </summary>
        public uint Token;
        /// <summary>
        /// ID of match
        /// </summary>
        public ulong MatchId;
        /// <summary>
        /// ID of outcome of match
        /// </summary>
        public ulong OutcomeId;
    }
}