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
        [JsonPropertyName("players")]
        public Player[] Players { get; set; }
    }

    public class Player
    {
        [JsonPropertyName("steamid")]
        public string SteamID { get; set; }
        public int communityvisibilitystate { get; set; }
        public int profilestate { get; set; }
        [JsonPropertyName("personaname")]
        public string PersonaName { get; set; }
        public string profileurl { get; set; }
        public string avatar { get; set; }
        public string avatarmedium { get; set; }
        public string avatarfull { get; set; }
        public string avatarhash { get; set; }
        public int lastlogoff { get; set; }
        public int personastate { get; set; }
        public string primaryclanid { get; set; }
        public int timecreated { get; set; }
        public int personastateflags { get; set; }
    }
}
