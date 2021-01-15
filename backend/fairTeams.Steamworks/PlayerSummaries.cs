using System.Text.Json.Serialization;

namespace fairTeams.Steamworks
{
    public class PlayerSummariesResponse
    {
        [JsonPropertyName("response")]
        public Response Response { get; set; }
    }

    public class Response
    {
        public Player[] Players { get; set; }
    }

    public class Player
    {
        public string SteamID { get; set; }
        public int Communityvisibilitystate { get; set; }
        public int Profilestate { get; set; }
        public string PersonaName { get; set; }
        public string Profileurl { get; set; }
        public string Avatar { get; set; }
        public string AvatarMedium { get; set; }
        public string AvatarFull { get; set; }
        public string AvatarHash { get; set; }
        public int LastLogOff { get; set; }
        public int PersonaState { get; set; }
        public string PrimaryClanId { get; set; }
        public int TimeCreated { get; set; }
        public int PersonaStateFlags { get; set; }
    }
}
