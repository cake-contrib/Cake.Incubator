// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/. 
namespace Cake.Incubator.Tests
{
    using FluentAssertions;
    using Xunit;

    public class StringExtensionsTests
    {
        [Fact]
        public void EqualsIgnoreCase_MatchesCorrectly()
        {
            "A".EqualsIgnoreCase("a").Should().BeTrue();
        }

        [Fact]
        public void EqualsIgnoreCase_DoesNotMatchCorrectly()
        {
            "A".EqualsIgnoreCase("B").Should().BeFalse();
        }

        //[Theory]
        //[InlineData(null)]
        //[InlineData("")]
        //[InlineData(" ")]
        //public void HasCondition_ReturnsFalseIfNullOrEmpty(string condition)
        //{
        //    condition.HasTargetFrameworkCondition().Should().BeFalse();
        //}

        //[Theory]
        //[InlineData("'$(TargetFramework)'==")]
        //[InlineData("'$(TargetFramework)' ==")]
        //[InlineData("'$(TargetFramework)'  ==")]
        //[InlineData(" '$(TargetFramework)'==  ")]
        //public void HasCondition_ReturnsTrue(string condition)
        //{
        //    condition.HasTargetFrameworkCondition().Should().BeTrue();
        //}

        //[Theory]
        //[InlineData("'$(TargetFramework)'=='net45' ", "net45")]
        //[InlineData("'$(TargetFramework)' =='net462'", "net462")]
        //[InlineData("'$(TargetFramework)'  == 'netstandard1_6' ", "netstandard1_6")]
        //[InlineData(" '$(TargetFramework)'==  'bobbins'  ", "bobbins")]
        //public void GetCondition_ReturnsExpected(string condition, string expected)
        //{
        //    condition.GetConditionTargetFramework().Should().Be(expected);
        //}
    }
}