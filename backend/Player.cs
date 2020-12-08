using System;
using System.IO;
using System.Net;
using Newtonsoft.Json;

namespace backend
{
    public class Player {
        private const string steamAPIKey = "B0E3E0ED2572C01223E0ED7043E9678C";
        public string Name { get; set; }

        public string SteamID { get; set; }

        public SkillLevel Skill { get; set; }

        public Team Team { get; set; }

        public void ScrapeName()
        {
            if (string.IsNullOrEmpty(SteamID))
            {
                throw new InvalidOperationException("The SteamID has to be set to scrape the player's name.");
            }

            var webRequest = WebRequest.Create($"https://api.steampowered.com/ISteamUser/GetPlayerSummaries/v0002/?key={steamAPIKey}&steamids={SteamID}");
            if (webRequest == null)
            {
                return;
            }

            webRequest.ContentType = "application/json";

            using (var s = webRequest.GetResponse().GetResponseStream())
            {
                using (var sr = new StreamReader(s))
                {
                    dynamic json = JsonConvert.DeserializeObject(sr.ReadToEnd());
                    string profileName = json.response.players[0].personaname;
                    Name = profileName.Replace("'", "");
                }
            }
        }
    }
}