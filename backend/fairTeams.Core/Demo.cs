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
        public string ShareCode { get; set; }
        public string FilePath { get; set; }
        public DemoState State { get; set; }
    }
}
