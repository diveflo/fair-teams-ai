namespace fairTeams.API.Rating
{
    public interface IRating
    {
        string Name { get; }
        double Score { get; }
        Trend Trend { get; }
    }
}