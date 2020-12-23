namespace fairTeams.API.Rating
{
    public class HLTVRating : IRating
    {
        public string Name => "HLTV";
        public double Score { get; }
    }
}