using Newtonsoft.Json;
using System;
using System.IO;
using System.Net;

namespace backend
{
    public class Player
    {
        private const string steamAPIKey = "B0E3E0ED2572C01223E0ED7043E9678C";
        public string Name { get; set; }
        public string SteamID { get; set; }
        public SkillLevel Skill { get; set; }

        public void ScrapeName()
        {
            if (string.IsNullOrEmpty(SteamID))
            {
                throw new InvalidOperationException("The SteamID has to be set to scrape the player's name.");
            }

            var webRequest = WebRequest.Create($"https://api.steampowered.com/ISteamUser/GetPlayerSummaries/v0002/?key={steamAPIKey}&steamids={SteamID}");

            webRequest.ContentType = "application/json";

            using var responseStream = webRequest.GetResponse().GetResponseStream();
            using var responseStreamReader = new StreamReader(responseStream);
            dynamic responseJson = JsonConvert.DeserializeObject(responseStreamReader.ReadToEnd());

            if (responseJson.response.players.Count != 1)
            {
                throw new ArgumentException($"The SteamID ({SteamID}) you provided could not be found. Please check the player's Steam community profile URL.");
            }

            string profileName = responseJson.response.players[0].personaname;
            Name = profileName.Replace("'", "");
        }
    }
}