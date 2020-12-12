using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;

namespace backend.Rating
{
    public class KDRating : IRating
    {
        private const string steamAPIKey = "B0E3E0ED2572C01223E0ED7043E9678C";
        private readonly string mySteamID;
        public string Name => "KD";
        public double Score { get; }

        public KDRating(string steamID)
        {
            mySteamID = steamID;
            Score = ScrapeForPlayer();
        }

        public double ScrapeForPlayer()
        {
            var webRequest = WebRequest.Create($"https://api.steampowered.com/ISteamUserStats/GetUserStatsForGame/v2/?appid=730&key={steamAPIKey}&steamid={mySteamID}");

            webRequest.ContentType = "application/json";

            try
            {
                using var responseStream = webRequest.GetResponse().GetResponseStream();
                using var responseStreamReader = new StreamReader(responseStream);

                dynamic responseJson = JsonConvert.DeserializeObject(responseStreamReader.ReadToEnd());

                IEnumerable<dynamic> stats = responseJson.playerstats.stats;
                double totalKills = stats.Where(x => x.name == "total_kills").Single().value;
                double totalDeaths = stats.Where(x => x.name == "total_deaths").Single().value;

                return totalKills / totalDeaths;
            }
            catch (WebException)
            {
                throw new ProfileNotPublicException();
            }
        }
    }
}