namespace fairTeams.Core
{
    /// <summary>
    /// Request object for RequestGame
    /// </summary>
    public class GameRequest
    {
        /// <summary>
        /// UNKNOWN
        /// </summary>
        public uint Token { get; set; }
        /// <summary>
        /// ID of match
        /// </summary>
        public ulong MatchId { get; set; }
        /// <summary>
        /// ID of outcome of match
        /// </summary>
        public ulong OutcomeId { get; set; }
    }
}