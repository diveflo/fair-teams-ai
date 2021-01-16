using fairTeams.Core;

namespace fairTeams.DemoHandling
{
    public class MatchMakingSteamUser
    {
        public long SteamID { get; set; }
        public Rank Rank { get; set; }
        public string AuthenticationCode { get; set; }
        public string LastSharingCode { get; set; }
    }
}
