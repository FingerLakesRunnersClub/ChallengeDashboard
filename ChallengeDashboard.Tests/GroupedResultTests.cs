using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace FLRC.ChallengeDashboard.Tests
{
    public class GroupedResultTests
    {
        [Fact]
        public void CanGetAverageDuration()
        {
            var athlete = new Athlete();

            //arrange
            var results = new List<Result>
            {
                new Result { Athlete = athlete, Duration = new Time(TimeSpan.Parse("1:00")) },
                new Result { Athlete = athlete, Duration = new Time(TimeSpan.Parse("1:10")) },
                new Result { Athlete = athlete, Duration = new Time(TimeSpan.Parse("1:20")) },
                new Result { Athlete = athlete, Duration = new Time(TimeSpan.Parse("1:30")) }
            };

            var groupedResult = new GroupedResult(results.GroupBy(r => r.Athlete).First());

            //act
            var avg = groupedResult.Average();

            //assert
            Assert.Equal(TimeSpan.Parse("1:15"), avg.Duration.Value);
        }

        [Fact]
        public void CanGetAverageFromTopAttempts()
        {
            //arrange
            var athlete = new Athlete();

            var results = new List<Result>
            {
                new Result { Athlete = athlete, Duration = new Time(TimeSpan.Parse("1:00")) },
                new Result { Athlete = athlete, Duration = new Time(TimeSpan.Parse("1:10")) },
                new Result { Athlete = athlete, Duration = new Time(TimeSpan.Parse("1:20")) }
            };

            var groupedResult = new GroupedResult(results.GroupBy(r => r.Athlete).First());

            //act
            var avg = groupedResult.Average(2);

            //assert
            Assert.Equal(TimeSpan.Parse("1:05"), avg.Duration.Value);
        }

        [Fact]
        public void CanCompareGroupedResultsByAthleteID()
        {
            //arrange
            var athlete1 = new Athlete { ID = 234 };
            var athlete2 = new Athlete { ID = 123 };

            var results = new List<Result>
            {
                new Result { Athlete = athlete1, Duration = new Time(TimeSpan.Parse("1:00")) },
                new Result { Athlete = athlete2, Duration = new Time(TimeSpan.Parse("1:10")) }
            };

            var groups = results.GroupBy(r => r.Athlete).ToArray();

            //act
            var group1 = new GroupedResult(groups[0]);
            var group2 = new GroupedResult(groups[1]);

            //assert
            Assert.Equal(1, group1.CompareTo(group2));
            Assert.Equal(-1, group2.CompareTo(group1));
        }
    }
}