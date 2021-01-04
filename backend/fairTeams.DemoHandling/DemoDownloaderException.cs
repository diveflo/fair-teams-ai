using System;

namespace fairTeams.DemoHandling
{
    public class DemoDownloaderException : Exception
    {
        public DemoDownloaderException() : base() { }
        public DemoDownloaderException(string message) : base(message) { }
    }

    public class DemoNotAvailableException : DemoDownloaderException { }
}
