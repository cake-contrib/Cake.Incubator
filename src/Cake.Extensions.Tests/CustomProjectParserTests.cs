// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/. 
namespace Cake.Extensions.Tests
{
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

        public CustomProjectParserTests()
        {
            fileSystem = A.Fake<IFileSystem>();
            environment = A.Fake<ICakeEnvironment>();
            parser = new CustomProjectParser(fileSystem, environment);
            validFile = new FakeFile(Resources.CsProj_ValidFile);
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
    }

    public class FakeFile : IFile
    {
        private readonly string _content;

        public FakeFile(string content)
        {
            _content = content;
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
            return new MemoryStream(Encoding.UTF8.GetBytes(_content));
        }

        public FilePath Path { get; }
        public long Length { get; }

        Path IFileSystemInfo.Path => Path;

        public bool Exists { get; }
        public bool Hidden { get; }
    }
}