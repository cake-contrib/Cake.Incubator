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
            var solutionProject = new SolutionProject("1", "test", new FilePath("/a"), ProjectTypes.SolutionFolder);

            solutionProject.IsSolutionFolder().Should().BeTrue();
        }

        [Fact]
        public void IsType_ReturnsTrue_ForCorrectType()
        {
            var solutionProject = new SolutionProject("1", "test", new FilePath("/a"), ProjectTypes.CSharp);

            solutionProject.IsType(ProjectType.CSharp).Should().BeTrue();
        }

        [Fact]
        public void CanGetSolutionProjects_WithoutSolutionFolders()
        {
            var result = GetSolutionParserResult();

            result.Projects.Should().HaveCount(2);
            result.GetProjects()
                .Should()
                .HaveCount(1)
                .And.Contain(x => x.Type == ProjectTypes.CSharp);
        }

        private SolutionParserResult GetSolutionParserResult()
        {
            var projects = new[]
                               {
                                   new SolutionProject(
                                       "1",
                                       "test",
                                       "/a.csproj",
                                       ProjectTypes.CSharp),
                                   new SolutionProject(
                                       "2",
                                       "folder",
                                       "/b",
                                       ProjectTypes.SolutionFolder)
                               };
            return new SolutionParserResult("1", "1", "1", projects);
        }
    }
}