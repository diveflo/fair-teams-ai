namespace backend.Rating
{
    public class DummyRating : IRating
    {
        public string Name => "HLTV";
        public double Score { get; set; }
    }
}
