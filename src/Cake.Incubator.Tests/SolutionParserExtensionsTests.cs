// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/. 
namespace Cake.Incubator.Tests
{
    using System.Linq;
    using Cake.Common.Solution;
    using Cake.Core.IO;
    using Cake.Incubator.Project;
    using Cake.Incubator.SolutionParserExtensions;
    using FluentAssertions;
    using Xunit;

    /// <summary>
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

        [Fact]
        public void GetAssemblyFilePath_ReturnsExpectedFilePath()
        {
            var result = GetSolutionParserResult().Projects.First();
            var parserResult = new CustomProjectParserResult { AssemblyName = "a", OutputPath = "./b", OutputType = "library" };

            result.GetAssemblyFilePath(parserResult).ToString().Should().Be("b/a.dll");
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