using fairTeams.API.Rating;
using System;
using Xunit;

namespace fairTeams.API.Tests
{
    public class SkillLevelTests
    {
        [Fact]
        public void SkillScore_NoRatingsAdded_ThrowsInvalidOperation()
        {
            var skillLevel = new SkillLevel();

            Assert.Throws<InvalidOperationException>(() => skillLevel.SkillScore);
        }

        [Fact]
        public void SkillScore_DummyRatingAdded_ReturnsRatingValue()
        {
            var expectedScore = 1.0d;
            var skillLevel = new SkillLevel();
            skillLevel.AddRating(new DummyRating() { Score = expectedScore });

            var skillScore = skillLevel.SkillScore;

            Assert.Equal(expectedScore, skillScore);
        }

        [Fact]
        public void SkillScore_TwoRatingsAdded_ReturnsAverage()
        {
            var expectedScore = 2.0d;
            var skillLevel = new SkillLevel();
            skillLevel.AddRating(new DummyRating() { Score = 1.0d });
            skillLevel.AddRating(new DummyRating() { Score = 3.0d });

            var skillScore = skillLevel.SkillScore;

            Assert.Equal(expectedScore, skillScore);
        }

        [Fact]
        public void CompareTo_FirstBetterThanSecond_ReturnsNegativeValue()
        {
            var firstSkillLevel = new SkillLevel();
            firstSkillLevel.AddRating(new DummyRating() { Score = 1.0d });
            var secondSkillLevel = new SkillLevel();
            secondSkillLevel.AddRating(new DummyRating() { Score = 0.5d });

            var comparisonResult = firstSkillLevel.CompareTo(secondSkillLevel);

            Assert.True(comparisonResult < 0.0d);
        }
    }
}
