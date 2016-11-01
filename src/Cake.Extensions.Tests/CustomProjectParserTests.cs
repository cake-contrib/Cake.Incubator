// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/. 
namespace Cake.Extensions.Tests
{
    using System;
    using System.IO;
    using System.Text;

    using Cake.Core;
    using Cake.Core.IO;
    using FakeItEasy;
    using FluentAssertions;
    using Xunit;
    using Path = Cake.Core.IO.Path;

    public class CustomProjectParserTests
    {
        private readonly IFileSystem fileSystem;
        private readonly ICakeEnvironment environment;
        private readonly CustomProjectParser parser;
        private readonly FakeFile validFile;
        private readonly FakeFile anotherValidFile;

        public CustomProjectParserTests()
        {
            fileSystem = A.Fake<IFileSystem>();
            environment = A.Fake<ICakeEnvironment>();
            parser = new CustomProjectParser(fileSystem, environment);
            validFile = new FakeFile(Resources.CsProj_ValidFile);
            anotherValidFile = new FakeFile(Resources.AnotherCSProj);
        }

        [Fact]
        public void CustomProjectParser_CanParseSampleFile_ForDebugConfig()
        {
            var path = new FilePath("/a");
            A.CallTo(() => fileSystem.GetFile(path)).Returns(validFile);

            var result = parser.Parse(path, "debug");

            result.Configuration.Should().Be("debug");
            result.OutputPath.ToString().Should().Be("bin/Debug");
        }

        [Fact]
        public void CustomProjectParser_CanParseSampleFile_ForReleaseConfig()
        {
            var path = new FilePath("/a");

            A.CallTo(() => fileSystem.GetFile(path)).Returns(validFile);

            var result = parser.Parse(path, "reLEAse");

            result.Configuration.Should().Be("reLEAse");
            result.OutputPath.ToString().Should().Be("bin/Release");
        }

        [Fact]
        public void CustomProjectParser_CanParseProjectTypeGuids()
        {
            var path = new FilePath("/a");

            A.CallTo(() => fileSystem.GetFile(path)).Returns(validFile);

            var result = parser.Parse(path, "Debug");

            result.IsType(ProjectType.CSharp).Should().BeTrue();
            result.IsType(ProjectType.PortableClassLibrary).Should().BeTrue();
            result.IsType(ProjectType.FSharp).Should().BeFalse();
        }

        [Fact]
        public void CanGetProjectType_WhenProjectTypeGuidsIsNull()
        {
            var result = new TestProjectParserResult { ProjectTypeGuids = null };
            result.IsType(ProjectType.Undefined).Should().BeTrue();
        }

        [Fact]
        public void CanGetProjectTypeFromMulitple()
        {
            var result = new TestProjectParserResult()
                             {
                                 ProjectTypeGuids =
                                     new[] { ProjectTypes.CSharp, ProjectTypes.AspNetMvc1 }
                             };

            result.IsType(ProjectType.Undefined).Should().BeFalse();
            result.IsType(ProjectType.CSharp).Should().BeTrue();
            result.IsType(ProjectType.AspNetMvc1).Should().BeTrue();
        }
        
    }

    internal class TestProjectParserResult : CustomProjectParserResult
    {
        public TestProjectParserResult()
            : base("Debug", "x86", Guid.NewGuid().ToString(), new []{ ProjectTypes.CSharp }, "Library", "/bin/Debug", "RootNamespace", "AssemblyName", "v4.5", null, null, null, null)
        {
        }
    }

    internal class FakeFile : IFile
    {
        private readonly string content;

        public FakeFile(string content)
        {
            this.content = content;
            Exists = true;
            Path = "a.sln";
        }

        public void Copy(FilePath destination, bool overwrite)
        {
            throw new System.NotImplementedException();
        }

        public void Move(FilePath destination)
        {
            throw new System.NotImplementedException();
        }

        public void Delete()
        {
            throw new System.NotImplementedException();
        }

        public Stream Open(FileMode fileMode, FileAccess fileAccess, FileShare fileShare)
        {
            return new MemoryStream(Encoding.UTF8.GetBytes(content));
        }

        public FilePath Path { get; }
        public long Length { get; }

        Path IFileSystemInfo.Path => Path;

        public bool Exists { get; }
        public bool Hidden { get; }
    }
}