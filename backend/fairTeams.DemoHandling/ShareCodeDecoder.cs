using fairTeams.Core;
using System;
using System.Linq;
using System.Numerics;
using System.Text.RegularExpressions;

namespace fairTeams.DemoHandling
{
    public class ShareCodeDecoder
    {
        const string DICTIONARY = "ABCDEFGHJKLMNOPQRSTUVWXYZabcdefhijkmnopqrstuvwxyz23456789";
        const string SHARECODE_PATTERN = "^CSGO(-?[\\w]{5}){5}$";

        public static GameRequest Decode(string shareCode)
        {
            var r = new Regex(SHARECODE_PATTERN);
            if (!r.IsMatch(shareCode))
                throw new ShareCodePatternException();

            string code = shareCode.Remove(0, 4).Replace("-", "");

            BigInteger big = BigInteger.Zero;
            foreach (char c in code.ToCharArray().Reverse())
                big = BigInteger.Multiply(big, DICTIONARY.Length) + DICTIONARY.IndexOf(c);

            big = SwapEndianness(big);
            var bitmask64 = BigInteger.Pow(2, 64) - 1;

            var matchId = big & bitmask64;
            var outcomdeId = big >> 64 & bitmask64;
            var token = big >> 128;// & 0xFFF;

            return new GameRequest { MatchId = (ulong)matchId, OutcomeId = (ulong)outcomdeId, Token = (uint)token };
        }

        private static BigInteger SwapEndianness(BigInteger number)
        {
            BigInteger result = 0;

            for (var i = 0; i < 144; i += 8)
            {
                result = (result << 8) + ((number >> i) & 0xFF);
            }

            return result;
        }

        public class ShareCodePatternException : Exception
        {
            public ShareCodePatternException() : base("Invalid share code")
            {
            }
        }
    }
}