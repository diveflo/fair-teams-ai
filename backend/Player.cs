using Newtonsoft.Json;
using System;
using System.IO;
using System.Net;

namespace backend
{
    public class Player
    {
        public string SteamID { get; set; }

        public SkillLevel Skill { get; set; }
    }
}