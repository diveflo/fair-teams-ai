namespace backend.Rating
{
    public interface IRating
    {
        string Name { get; }
        double Score { get; set; }

        void ScrapeForPlayer(string steamID);
    }
}