// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/. 
namespace Cake.Incubator.Tests
{
    using FluentAssertions;
    using Xunit;

    public class ProjectTypesTests
    {
        [Fact]
        public void HasFlag_FindsCorrectType()
        {
            var projectType = ProjectType.AspNetMvc1 | ProjectType.CSharp;
            projectType.HasFlag(ProjectType.CSharp).Should().BeTrue();
            projectType.HasFlag(ProjectType.AspNetMvc1).Should().BeTrue();
        }
    }
}