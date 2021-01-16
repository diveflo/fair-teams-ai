using fairTeams.API.Rating;
using fairTeams.Core;
using System;

namespace fairTeams.API
{
    public class SkillLevel : IComparable
    {
        private IRating myRating;

        public double SkillScore => myRating.Score;
        public Trend SkillTrend => myRating.Trend;
        public Rank Rank { get; set; }

        public SkillLevel()
        {
            Rank = Rank.NotRanked;
        }

        public void SetRating(IRating rating)
        {
            if (myRating == null)
            {
                myRating = rating;
            }
        }

        public int CompareTo(object obj)
        {
            var score = (obj as SkillLevel).SkillScore;

            return score < SkillScore ? -1 : score == SkillScore ? 0 : 1;
        }
    }
}