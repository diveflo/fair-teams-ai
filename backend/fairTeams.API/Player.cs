using System;

namespace fairTeams.API
{
    public class Player
    {
        public string Name { get; set; }
        public string SteamName { get; set; }
        [Obsolete("Please do not use anymore.")]
        public bool ProfilePublic { get; set; }
        public string SteamID { get; set; }
        public SkillLevel Skill { get; set; }

        public Player() { }

        public Player(RequestPlayer obj)
        {
            Name = obj.Name;
            SteamName = obj.SteamName;
            SteamID = obj.SteamID;
        }
    }

    public class RequestPlayer
    {
        public string Name { get; set; }
        public string SteamName { get; set; }
        public string SteamID { get; set; }
    }
}