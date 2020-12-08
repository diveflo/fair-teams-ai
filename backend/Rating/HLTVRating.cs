using System;

namespace backend.Rating
{
    public class HLTVRating : IRating
    {
        public string Name => "HLTV";
        public double Score { get; set; }

        public void ScrapeForPlayer(string steamID)
        {
            var random = new Random();
            Score = random.NextDouble();
        }
    }
}