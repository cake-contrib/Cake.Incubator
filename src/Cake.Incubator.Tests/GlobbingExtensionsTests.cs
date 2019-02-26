// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/. 
namespace Cake.Incubator.Tests
{
    using Cake.Common.IO;
    using Cake.Core;
    using Cake.Core.Diagnostics;
    using Cake.Core.IO;
    using Cake.Core.Tooling;
    using Cake.Incubator.GlobbingExtensions;
    using FakeItEasy;
    using FluentAssertions;
    using Xunit;

    public class GlobbingExtensionsTests : IClassFixture<CakeFixture>
    {
        private readonly CakeContext context;

        public GlobbingExtensionsTests(CakeFixture fixture)
        {
            fixture.FileSystem.AddFile(new FakeFile("", "c:/a.txt"));
            fixture.FileSystem.AddFile(new FakeFile("", "c:/a/c.txt"));
            fixture.FileSystem.AddFile(new FakeFile("", "c:/c/a.txt"));
            context = fixture.Context;
        }

        [Fact()]
        public void GetMatchingFiles_ReturnsEmpty_IfNoMatches()
        {
            var patternA = "a.*";
            var patternB = "b.*";

            var files = context.GetFiles(patternA, patternB);
            files.Should().NotBeEmpty();
        }
    }
}