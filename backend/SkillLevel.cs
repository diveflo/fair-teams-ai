using backend.Rating;
using System;
using System.Collections.Generic;
using System.Linq;

namespace backend
{
    public class SkillLevel : IComparable
    {
        private readonly IList<IRating> myRatings;

        public double SkillScore => myRatings.Average(x => x.Score);
        public SkillLevel()
        {
            myRatings = new List<IRating>();
        }

        public void AddRating(IRating rating)
        {
            if (!myRatings.Contains(rating))
            {
                myRatings.Add(rating);
            }
        }

        public int CompareTo(object obj)
        {
            var score = (obj as SkillLevel).SkillScore;

            return score < SkillScore ? -1 : score == SkillScore ? 0 : 1;
        }
    }
}