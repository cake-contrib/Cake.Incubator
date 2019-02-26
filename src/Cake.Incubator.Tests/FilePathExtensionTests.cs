// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/. 

namespace Cake.Incubator.Tests
{
    using Cake.Core.IO;
    using Cake.Incubator.FilePathExtensions;
    using FluentAssertions;
    using Xunit;

    public class FilePathExtensionTests
    {
        [Theory]
        [InlineData("test")]
        [InlineData("test.cs")]
        [InlineData(".g")]
        public void IsSolution_ReturnsFalse_ForNonSolution(string fileName)
        {
            new FilePath(fileName).IsSolution().Should().BeFalse();
        }

        [Fact]
        public void IsSolution_ReturnsTrue_ForSolution()
        {
            new FilePath("a.sln").IsSolution().Should().BeTrue();
        }

        [Theory]
        [InlineData("test")]
        [InlineData("test.cs")]
        [InlineData(".g")]
        public void IsProject_ReturnsFalse_ForNonProject(string fileName)
        {
            new FilePath(fileName).IsSolution().Should().BeFalse();
        }

        [Fact]
        public void IsProject_ReturnsTrue_ForCSProject()
        {
            new FilePath("a.csproj").IsProject().Should().BeTrue();
        }

        [Fact]
        public void IsProject_ReturnsTrue_ForFSProject()
        {
            new FilePath("a.fsproj").IsProject().Should().BeTrue();
        }

        [Theory]
        [InlineData("file", "file")]
        [InlineData("file.cs", "file.txt")]
        [InlineData("filE.cs", "File.txt")]
        public void HasFileName_ReturnsTrue_ForMatch(string fileName, string expected)
        {
            new FilePath(expected).HasFileName(fileName).Should().BeTrue();
        }

        [Theory]
        [InlineData("file", "file1")]
        [InlineData("file.cs", "file1.txt")]
        public void HasFileName_ReturnsFalse_ForNonMatch(string fileName, string expected)
        {
            new FilePath(expected).HasFileName(fileName).Should().BeFalse();
        }
        
        [Theory]
        [InlineData("file", ".cs")]
        [InlineData("file.cs", ".txt")]
        public void HasFileExtension_ReturnsFalse_ForNonMatch(string fileName, string expected)
        {
            new FilePath(expected).HasFileExtension(fileName).Should().BeFalse();
        }
        
        [Theory]
        [InlineData("file.cs", ".CS")]
        [InlineData("file.cs", "Cs")]
        [InlineData("filE.cs", ".cs")]
        public void HasFileExtension_ReturnsTrue_ForMatch(string expected, string fileName)
        {
            new FilePath(expected).HasFileExtension(fileName).Should().BeTrue();
        }
    }
}
