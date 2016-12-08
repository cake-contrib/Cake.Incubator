// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/. 
namespace Cake.Extensions.Tests
{
    using System;
    using FluentAssertions;
    using Xunit;

    public class AssertExtensionTests
    {
        [Fact]
        public void ThrowIfNull_WithMessage_SetsMessage()
        {
            object obj = null;

            Action action = () => obj.ThrowIfNull(nameof(obj), "This is required");
            action.ShouldThrow<ArgumentNullException>().WithMessage(
            "This is required\r\nParameter name: obj");
        }
    }
}
