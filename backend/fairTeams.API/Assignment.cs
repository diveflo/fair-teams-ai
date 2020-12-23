namespace fairTeams.API
{
    public class Assignment
    {
        public Team Terrorists { get; }

        public Team CounterTerrorists { get; }

        public Assignment(Team terrorists, Team counterTerrorists)
        {
            Terrorists = terrorists;
            CounterTerrorists = counterTerrorists;
        }
    }
}