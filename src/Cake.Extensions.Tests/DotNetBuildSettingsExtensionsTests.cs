// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.
namespace Cake.Extensions.Tests
{
    using Cake.Common.Tools;

    using FluentAssertions;

    using Xunit;

    public class DotNetBuildSettingsExtensionsTests
    {
        [Fact]
        public void CanAddMultipleTargets()
        {
            var targets = new[] { "One", "Two" };

            var settings = new DotNetBuildSettings("a.sln");
            settings.WithTargets(targets);

            settings.Targets.Should().HaveCount(2).And.Contain(targets);
        }
    }
}