// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/. 
namespace Cake.Incubator.Tests
{
    using System;
    using FluentAssertions;
    using Xunit;

    public class AssertExtensionsTests
    {
        [Fact]
        public void ThrowIfNull_WithMessage_SetsMessage()
        {
            object obj = null;

            Action action = () => obj.ThrowIfNull(nameof(obj), "This is required");
            action.Should().Throw<ArgumentNullException>().WithMessage(
                "This is required\r\nParameter name: obj");
        }
    
        [Fact]
        public void ThrowIfNull_ThrowsWhenNull()
        {
            string a = null;
            Action action = () => a.ThrowIfNull(nameof(a));

            action.Should().Throw<ArgumentNullException>().And.ParamName.Should().Be("a");
        }

        [Fact]
        public void ThrowIfNull_DoesNotThrowIfNotNull()
        {
            var a = "";
            a.ThrowIfNull(nameof(a));
        }
    }
}