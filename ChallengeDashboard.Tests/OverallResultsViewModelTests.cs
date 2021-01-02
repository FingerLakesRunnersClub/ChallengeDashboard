﻿using Xunit;

namespace FLRC.ChallengeDashboard.Tests
{
    public class OverallResultsViewModelTests
    {
        [Fact]
        public void CanGetTitle()
        {
            //arrange
            var vm = new OverallResultsViewModel {ResultType = "Unit Test"};

            //act
            var title = vm.Title;

            //assert
            Assert.Equal("Unit Test", title);
        }

        [Theory]
        [InlineData("Top Teams", "lg")]
        [InlineData("Most Miles", "sm")]
        [InlineData("Most Points (F)", "lg")]
        [InlineData("Most Points (M)", "lg")]
        public void ResponsiveBreakpointSet(string resultType, string expected)
        {
            //arrange
            var vm = new OverallResultsViewModel {ResultType = resultType};
            
            //act
            var breakpoint = vm.ResponsiveBreakpoint;
            
            //assert
            Assert.Equal(expected, breakpoint);
        }
}
}