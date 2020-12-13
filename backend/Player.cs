using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;
using System.Net;

namespace backend
{
    public class Player
    {
        public string Name { get; set; }
        public string SteamName { get; set; }
        public bool ProfilePublic { get; set; }
        public string SteamID { get; set; }
        public SkillLevel Skill { get; set; }
    }
}