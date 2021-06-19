using fairTeams.API.Rating;
using System;
using Xunit;

namespace fairTeams.API.Tests
{
    public class SkillLevelTests
    {
        [Fact]
        [Trait("Category", "unit")]
        public void SkillScore_NoRatingsAdded_ThrowsNullRef()
        {
            var skillLevel = new SkillLevel();

            Assert.Throws<NullReferenceException>(() => skillLevel.SkillScore);
        }

        [Fact]
        [Trait("Category", "unit")]
        public void SkillScore_DummyRatingAdded_ReturnsRatingValue()
        {
            var expectedScore = 1.0d;
            var skillLevel = new SkillLevel();
            skillLevel.SetRating(new DummyRating() { Score = expectedScore });

            var skillScore = skillLevel.SkillScore;

            Assert.Equal(expectedScore, skillScore);
        }

        [Fact]
        [Trait("Category", "unit")]
        public void CompareTo_FirstBetterThanSecond_ReturnsNegativeValue()
        {
            var firstSkillLevel = new SkillLevel();
            firstSkillLevel.SetRating(new DummyRating() { Score = 1.0d });
            var secondSkillLevel = new SkillLevel();
            secondSkillLevel.SetRating(new DummyRating() { Score = 0.5d });

            var comparisonResult = firstSkillLevel.CompareTo(secondSkillLevel);

            Assert.True(comparisonResult < 0.0d);
        }

        [Fact]
        [Trait("Category", "unit")]
        public void CompareTo_SameScore_ReturnsZero()
        {
            var firstSkillLevel = new SkillLevel();
            firstSkillLevel.SetRating(new DummyRating() { Score = 1.0d });
            var secondSkillLevel = new SkillLevel();
            secondSkillLevel.SetRating(new DummyRating() { Score = 1.0d });

            var comparisonResult = firstSkillLevel.CompareTo(secondSkillLevel);

            Assert.Equal(0.0d, comparisonResult);
        }

        [Fact]
        [Trait("Category", "unit")]
        public void CompareTo_SecondBetterThanFirst_ReturnsPositiveValue()
        {
            var firstSkillLevel = new SkillLevel();
            firstSkillLevel.SetRating(new DummyRating() { Score = 0.5d });
            var secondSkillLevel = new SkillLevel();
            secondSkillLevel.SetRating(new DummyRating() { Score = 1.0d });

            var comparisonResult = firstSkillLevel.CompareTo(secondSkillLevel);

            Assert.True(comparisonResult > 0.0d);
        }
    }
}
