namespace fairTeams.Core
{
    public enum DemoState
    {
        New,
        Downloaded,
        Processed
    }

    public class Demo
    {
        public string Id { get; set; }
        public string ShareCode { get; set; }
        public GameRequest GameRequest { get; set; }
        public string FilePath { get; set; }
        public string DownloadURL { get; set; }
        public DemoState State { get; set; }
    }
}
