using System;

namespace fairTeams.DemoHandling
{
    public class ShareCode : IEquatable<ShareCode>
    {
        private readonly TimeSpan myCooldown;

        public string Code { get; set; }

        public int DownloadAttempt { get; private set; }

        public DateTime EarliestRetry { get; set; }

        public ShareCode(string code) : this(code, TimeSpan.FromMinutes(30)) { }

        public ShareCode(string code, TimeSpan cooldown)
        {
            Code = code;
            DownloadAttempt = 0;
            EarliestRetry = DateTime.UtcNow;
            myCooldown = cooldown;
        }

        public void IncrementDownloadAttempts()
        {
            DownloadAttempt++;
            EarliestRetry = DateTime.UtcNow + myCooldown;
        }

        public bool Equals(ShareCode other)
        {
            return other.Code.Equals(Code);
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as ShareCode);
        }

        public override int GetHashCode()
        {
            return Code.GetHashCode();
        }
    }
}
