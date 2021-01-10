namespace fairTeams.API.Rating
{
    public enum Trend
    {
        Plateau,
        Upwards,
        Downwards
    }

    public static class TrendHelper
    {
        public static Trend GetTrend(double overallScore, double latestScore)
        {
            const double plateauDelta = 0.05d;

            if ((overallScore + plateauDelta) < latestScore)
            {
                return Trend.Upwards;
            }

            if ((overallScore - plateauDelta) > latestScore)
            {
                return Trend.Downwards;
            }

            return Trend.Plateau;
        }
    }
}
