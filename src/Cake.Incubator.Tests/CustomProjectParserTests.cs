// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/. 
namespace Cake.Incubator.Tests
{
    using System;
    using System.IO;
    using System.Text;
    using Cake.Core.IO;
    using FluentAssertions;
    using Xunit;
    using Path = Cake.Core.IO.Path;

    public class CustomProjectParserTests
    {
        private readonly FakeFile validCsProjFile;
        private readonly FakeFile anotherValidFile;
        private readonly FakeFile valid2017CsProjFile;
        private readonly FakeFile valid2017CsProjNetcoreFile;
        private readonly FakeFile valid2017CsProjNetstandardFile;
        private readonly FakeFile validCsProjConditionalReferenceFile;

        public CustomProjectParserTests()
        {
            validCsProjFile = new FakeFile(Resources.CsProj_ValidFile);
            valid2017CsProjFile = new FakeFile(Resources.VS2017_CsProj_ValidFile);
            valid2017CsProjNetcoreFile = new FakeFile(Resources.VS2017_CsProj_NetCoreDefault);
            valid2017CsProjNetstandardFile = new FakeFile(Resources.VS2017_CsProj_NetStandard_ValidFile);
            validCsProjConditionalReferenceFile = new FakeFile(Resources.CsProj_ConditionReference_ValidFile);
            anotherValidFile = new FakeFile(Resources.AnotherCSProj);
        }

        [Fact]
        public void CustomProjectParser_CanParseSampleCsProjFile_ForDebugConfig()
        {
            var result = validCsProjFile.ParseProject("debug");

            result.Configuration.Should().Be("debug");
            result.OutputPath.ToString().Should().Be("bin/Debug");
        }

        [Fact]
        public void CustomProjectParser_CanParseSample2017CsProjFile_ForDebugConfig()
        {
            var result = valid2017CsProjFile.ParseProject("debug");

            result.Configuration.Should().Be("debug");
            result.OutputPath.ToString().Should().Be("bin/Debug");
        }

        [Fact]
        public void CustomProjectParser_CanParseSample2017CsProjNetCoreFile_ForDebugConfig()
        {
            var result = valid2017CsProjNetcoreFile.ParseProject("debug");

            result.Configuration.Should().Be("debug");
            result.OutputPath.ToString().Should().Be("bin/custom");
            result.OutputType.Should().Be("Exe");
            result.GetAssemblyFilePath().FullPath.Should().Be("bin/custom/project.exe");
        }

        [Fact]
        public void CustomProjectParser_CanParseSample2017CsProjNetCoreFile_ForReleaseConfig()
        {
            var result = valid2017CsProjNetcoreFile.ParseProject("release");

            result.Configuration.Should().Be("release");
            result.OutputPath.ToString().Should().Be("bin/release/netcoreapp1.1");
            result.OutputType.Should().Be("Exe");
            result.GetAssemblyFilePath().FullPath.Should().Be("bin/release/netcoreapp1.1/project.exe");
        }

        [Fact]
        public void CustomProjectParser_CanParseSample2017CsProjNetStandardFile_ForReleaseConfig()
        {
            var result = valid2017CsProjNetstandardFile.ParseProject("debug");

            result.Configuration.Should().Be("debug");
            result.OutputPath.ToString().Should().Be("bin/wayhey");
            result.OutputType.Should().Be("Library");
            result.GetAssemblyFilePath().FullPath.Should().Be("bin/wayhey/project.dll");
        }

        [Fact]
        public void CustomProjectParser_CanParseCsProjWithConditionalReferences()
        {
            var result = validCsProjConditionalReferenceFile.ParseProject("debug");

            result.References.Should().HaveCount(8).And.Contain(x => x.Name.Equals("Microsoft.VisualStudio.QualityTools.UnitTestFramework"));
        }
        
        [Fact]
        public void CustomProjectParser_CanParseSampleCsProjFile_ForReleaseConfig()
        {
            var result = validCsProjFile.ParseProject("reLEAse");

            result.Configuration.Should().Be("reLEAse");
            result.OutputPath.ToString().Should().Be("bin/Release");
        }

        [Fact]
        public void CustomProjectParser_CanParseSampleCsProjProjectTypeGuids()
        {
            var result = validCsProjFile.ParseProject("Debug");

            result.IsType(ProjectType.CSharp).Should().BeTrue();
            result.IsType(ProjectType.PortableClassLibrary).Should().BeTrue();
            result.IsType(ProjectType.FSharp).Should().BeFalse();
        }

        [Fact]
        public void CanGetProjectType_WhenProjectTypeGuidsIsNull()
        {
            var result = new TestProjectParserResult { ProjectTypeGuids = null };
            result.IsType(ProjectType.Unspecified).Should().BeTrue();
        }

        [Fact]
        public void CanGetProjectTypeFromMultiple()
        {
            var result = new TestProjectParserResult()
                             {
                                 ProjectTypeGuids =
                                     new[] { ProjectTypes.CSharp, ProjectTypes.AspNetMvc1 }
                             };

            result.IsType(ProjectType.Unspecified).Should().BeFalse();
            result.IsType(ProjectType.CSharp).Should().BeTrue();
            result.IsType(ProjectType.AspNetMvc1).Should().BeTrue();
        }
    }

    internal class TestProjectParserResult : CustomProjectParserResult
    {
        public TestProjectParserResult()
        {
            Configuration = "Debug";
            Platform = "x86";
            ProjectGuid = Guid.NewGuid().ToString();
            ProjectTypeGuids = new[] { ProjectTypes.CSharp };
            OutputType = "Library";
            OutputPath = "/bin/Debug";
            RootNameSpace = "RootNamespace";
            AssemblyName = "AssemblyName";
            TargetFrameworkVersion = "v4.5";
        }
    }

    internal class FakeFile : IFile
    {
        private readonly string content;

        public FakeFile(string content, string path = "./project.csproj")
        {
            this.content = content;
            Path = path;
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

        public long Length => throw new NotImplementedException();

        Path IFileSystemInfo.Path => Path;

        public bool Exists => throw new NotImplementedException();
        public bool Hidden => throw new NotImplementedException();
    }
}