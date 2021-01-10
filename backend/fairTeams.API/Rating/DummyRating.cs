namespace fairTeams.API.Rating
{
    public class DummyRating : IRating
    {
        public string Name => "Dummy";
        public double Score { get; set; }
        public Trend Trend => Trend.Plateau;
    }
}
