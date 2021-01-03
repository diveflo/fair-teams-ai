using fairTeams.Core;
using System;
using System.Linq;
using System.Numerics;
using System.Text.RegularExpressions;

namespace fairTeams.DemoHandling
{
    public class ShareCode
    {
        const string DICTIONARY = "ABCDEFGHJKLMNOPQRSTUVWXYZabcdefhijkmnopqrstuvwxyz23456789";
        const string SHARECODE_PATTERN = "^CSGO(-?[\\w]{5}){5}$";

        public static string Encode(ulong matchId, ulong reservationId, uint tvPort)
        {
            byte[] matchIdBytes = BitConverter.GetBytes(matchId);
            byte[] reservationBytes = BitConverter.GetBytes(reservationId);
            // only the UInt16 low bits from the TV port are used
            ushort tvPort16 = (ushort)(tvPort & ((1 << 16) - 1));
            byte[] tvBytes = BitConverter.GetBytes(tvPort16);

            byte[] bytes = new byte[matchIdBytes.Length + reservationBytes.Length + tvBytes.Length + 1];

            Buffer.BlockCopy(new byte[] { 0 }, 0, bytes, 0, 1);
            Buffer.BlockCopy(matchIdBytes, 0, bytes, 1, matchIdBytes.Length);
            Buffer.BlockCopy(reservationBytes, 0, bytes, 1 + matchIdBytes.Length, reservationBytes.Length);
            Buffer.BlockCopy(tvBytes, 0, bytes, 1 + matchIdBytes.Length + reservationBytes.Length, tvBytes.Length);

            var big = new BigInteger(bytes.Reverse().ToArray());

            var dictionaryAsCharArray = DICTIONARY.ToCharArray();
            string c = "";

            for (var i = 0; i < 25; i++)
            {
                BigInteger.DivRem(big, dictionaryAsCharArray.Length, out BigInteger rem);
                c += dictionaryAsCharArray[(int)rem];
                big = BigInteger.Divide(big, dictionaryAsCharArray.Length);
            }

            return $"CSGO-{c.Substring(0, 5)}-{c.Substring(5, 5)}-{c.Substring(10, 5)}-{c.Substring(15, 5)}-{c.Substring(20, 5)}";
        }

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