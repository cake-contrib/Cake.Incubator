// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/. 
namespace Cake.Incubator.Tests
{
    using Cake.Incubator.StringExtensions;
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
        
        [Fact]
        public void StartsWithIgnoreCase_MatchesCorrectly()
        {
            "Abcsd".StartsWithIgnoreCase("a").Should().BeTrue();
        }

        [Fact]
        public void StartsWithIgnoreCase_DoesNotMatchCorrectly()
        {
            "Adas".StartsWithIgnoreCase("AB").Should().BeFalse();
        }

        [Fact]
        public void EndsWithIgnoreCase_MatchesCorrectly()
        {
            "Abcsd".EndsWithIgnoreCase("SD").Should().BeTrue();
        }

        [Fact]
        public void EndsWithIgnoreCase_DoesNotMatchCorrectly()
        {
            "Adas".EndsWithIgnoreCase("AB").Should().BeFalse();
        }
    }
}