namespace backend.Rating
{
    public class DummyRating : IRating
    {
        public string Name => "Dummy";
        public double Score { get; set; }
    }
}