namespace fairTeams.Core
{
    public static class SteamIdDecoder
    {
        public static uint ToAccountId(long steamId)
        {
            return (uint)(steamId & 0xFFffFFff);
        }
    }
}
