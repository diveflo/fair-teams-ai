namespace fairTeams.Core
{
    public static class SteamIdDecoder
    {
        public static uint ToAccountId(long steamId)
        {
            return (uint)(steamId & 0xFFffFFff);
        }

        public static long ToSteamId(uint accountId)
        {
            return accountId + 76561197960265728L;
        }
    }
}
