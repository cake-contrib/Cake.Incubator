// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/. 
namespace Cake.Extensions.Tests
{
    using Cake.Common.Solution;
    using Cake.Core.IO;
    using FluentAssertions;
    using Xunit;

    /// <summary>
    /// TODO Handle more project types
    /// Parse multiple types 
    /// </summary>
    public class SolutionParserExtensionsTests
    {
        [Fact]
        public void IsSolutionFolder_ReturnsTrue_ForSolutionFolder()
        {
            new SolutionProject("1", "test", new FilePath("/a"), ProjectTypes.SolutionFolder).IsSolutionFolder().Should().BeTrue();
        }
    }
}