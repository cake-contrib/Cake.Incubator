// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/. 
namespace Cake.Incubator.Tests
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using Cake.Core.IO;
    using FluentAssertions;
    using Xunit;
    using Path = Cake.Core.IO.Path;

    public class TestProjects
    {
        private static readonly FakeFile validCsProjMSTestFile = new FakeFile(Resources.CsProj_ValidMSTestFile);
        private static readonly FakeFile validCsProjXUnitTestFile = new FakeFile(Resources.CsProj_ValidXUnitTestFile);
        private static readonly FakeFile validCsProjNUnitTestFile = new FakeFile(Resources.CsProjValidNUnitTestFile);
        private static readonly FakeFile validCsProjFSUnitTestFile = new FakeFile(Resources.CsProjValidFSUnitTestFile);
        private static readonly FakeFile validCsProjFixieTestFile = new FakeFile(Resources.CsProjValidFixieTestFile);

        // ReSharper disable once UnusedMember.Global
        public static IEnumerable<object[]> TestData { get; } = new List<object[]>
        {
            new object[] {validCsProjMSTestFile},
            new object[] {validCsProjNUnitTestFile},
            new object[] {validCsProjXUnitTestFile},
            new object[] {validCsProjFSUnitTestFile},
            new object[] {validCsProjFixieTestFile},
        };
    }

    public class CustomProjectParserTests
    {
        private readonly FakeFile validCsProjFile;
        private readonly FakeFile anotherValidFile;
        private readonly FakeFile validCsProjWebApplicationFile;
        private readonly FakeFile valid2017CsProjFile;
        private readonly FakeFile valid2017CsProjNetcoreFile;
        private readonly FakeFile valid2017CsProjNetstandardFile;
        private readonly FakeFile validCsProjConditionalReferenceFile;
        private readonly FakeFile validCsProjWithAbsoluteFilePaths;
        

        public CustomProjectParserTests()
        {
            validCsProjFile = new FakeFile(Resources.CsProj_ValidFile);
            valid2017CsProjFile = new FakeFile(Resources.VS2017_CsProj_ValidFile);
            valid2017CsProjNetcoreFile = new FakeFile(Resources.VS2017_CsProj_NetCoreDefault);
            valid2017CsProjNetstandardFile = new FakeFile(Resources.VS2017_CsProj_NetStandard_ValidFile);
            validCsProjConditionalReferenceFile = new FakeFile(Resources.CsProj_ConditionReference_ValidFile);
            anotherValidFile = new FakeFile(Resources.AnotherCSProj);
            validCsProjWebApplicationFile = new FakeFile(Resources.CsProj_ValidWebApplication);
            validCsProjWithAbsoluteFilePaths = new FakeFile(Resources.CsProj_AbsolutePath);
        }

        [Fact]
        public void CustomProjectParser_CanParseSampleCsProjFile_ForDebugConfig()
        {
            var result = validCsProjFile.ParseProjectFile("debug");

            result.Configuration.Should().Be("debug");
            result.OutputPath.ToString().Should().Be("bin/Debug");
        }

        [Fact]
        public void CustomProjectParser_CanParseSample2017CsProjFile_ForDebugConfig()
        {
            var result = valid2017CsProjFile.ParseProjectFile("debug");

            result.Configuration.Should().Be("debug");
            result.OutputPath.ToString().Should().Be("bin/Debug");
        }

        [Fact]
        public void CustomProjectParser_CanParseSample2017CsProjNetCoreFile_ForDebugConfig()
        {
            var result = valid2017CsProjNetcoreFile.ParseProjectFile("debug");

            result.Configuration.Should().Be("debug");
            result.OutputPath.ToString().Should().Be("bin/custom/netcoreapp1.1");
            result.OutputType.Should().Be("Exe");
            result.GetAssemblyFilePath().FullPath.Should().Be("bin/custom/netcoreapp1.1/project.dll");
        }
        
        [Fact]
        public void CustomProjectParser_CanGetNetCoreProjectAssembly_ForDebugConfig()
        {
            var result = validCsProjFile.ParseProjectFile("debug");

            result.Configuration.Should().Be("debug");
            result.OutputPath.ToString().Should().Be("bin/Debug");
            result.OutputPaths.Should().ContainSingle(x => x.FullPath.EqualsIgnoreCase("bin/Debug"));
            result.OutputType.Should().Be("Library");
            var paths = result.GetAssemblyFilePaths();
            paths.Should().ContainSingle(x => x.FullPath.EqualsIgnoreCase("bin/debug/cake.common.dll"));
        }


        [Theory]
        [MemberData(memberName: nameof(TestProjects.TestData), MemberType = typeof(TestProjects))]
        public void ParseProject_IsFrameworkTestProject(FakeFile testProject)
        {
            var result = testProject.ParseProjectFile("test");
            result.IsFrameworkTestProject().Should().BeTrue();
        }

        [Fact]
        public void CustomProjectParser_IsFrameworkTestProject_ReturnsFalseForNonTestFrameworkProject()
        {
            var result = valid2017CsProjFile.ParseProjectFile("debug");
            result.IsFrameworkTestProject().Should().BeFalse();
        }

        [Fact]
        public void CustomProjectParser_IsFrameworkTestProject_ReturnsFalseForNonTestCoreProject()
        {
            var result = valid2017CsProjNetcoreFile.ParseProjectFile("debug");
            result.IsFrameworkTestProject().Should().BeFalse();
        }


        [Fact]
        public void CustomProjectParser_ShouldParseProjectWithAbsolutePaths()
        {
            var result = validCsProjWithAbsoluteFilePaths.ParseProjectFile("debug");

            result.References.Should().Contain(x => x.HintPath.FullPath == "C:/Program Files (x86)/Reference Assemblies/Microsoft/Framework/.NETFramework/v4.5.2/System.dll");
        }

        [Fact]
        public void CustomProjectParser_CanParseSample2017CsProjNetCoreFile_ForReleaseConfig()
        {
            var result = valid2017CsProjNetcoreFile.ParseProjectFile("release");

            result.Configuration.Should().Be("release");
            result.OutputPath.ToString().Should().Be("bin/release/netcoreapp1.1");
            result.OutputType.Should().Be("Exe");
            result.GetAssemblyFilePath().FullPath.Should().Be("bin/release/netcoreapp1.1/project.dll");
        }

        [Fact]
        public void CustomProjectParser_CanParseSample2017CsProjNetStandardFile_ForReleaseConfig()
        {
            var result = valid2017CsProjNetstandardFile.ParseProjectFile("debug");

            result.Configuration.Should().Be("debug");
            result.OutputPath.ToString().Should().Be("bin/wayhey/netstandard1.6");
            result.OutputType.Should().Be("Library");
            result.GetAssemblyFilePath().FullPath.Should().Be("bin/wayhey/netstandard1.6/project.dll");
        }

        [Fact]
        public void CustomProjectParser_CanParseCsProjWithConditionalReferences()
        {
            var result = validCsProjConditionalReferenceFile.ParseProjectFile("debug");

            result.References.Should().HaveCount(8).And.Contain(x => x.Name.Equals("Microsoft.VisualStudio.QualityTools.UnitTestFramework"));
        }
        
        [Fact]
        public void CustomProjectParser_CanParseSampleCsProjFile_ForReleaseConfig()
        {
            var result = validCsProjFile.ParseProjectFile("reLEAse");

            result.Configuration.Should().Be("reLEAse");
            result.OutputPath.ToString().Should().Be("bin/Release");
        }

        [Fact]
        public void CustomProjectParser_CanParseSampleCsProjProjectTypeGuids()
        {
            var result = validCsProjFile.ParseProjectFile("Debug");

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

        [Fact]
        public void IsWebApplication_ReturnsFalse_WhenProjectIsOfTypeLibrary()
        {
            // arrange
            var sut = validCsProjFile.ParseProjectFile("debug");

            // act
            var webApp = sut.IsWebApplication();

            // assert
            webApp.Should().BeFalse();
        }
        
        [Fact]
        public void IsWebApplication_ReturnsFalse_When2017ProjectIsOfTypeLibrary()
        {
            // arrange
            var sut = valid2017CsProjFile.ParseProjectFile("debug");

            // act
            var webApp = sut.IsWebApplication();

            // assert
            webApp.Should().BeFalse();
        }

        [Fact]
        public void IsWebApplication_ReturnsTrue_WhenProjectIsOfTypeWebApplication()
        {
            // arrange
            var sut = validCsProjWebApplicationFile.ParseProjectFile("debug");

            // act
            var webApp = sut.IsWebApplication();

            // assert
            webApp.Should().BeTrue();
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

    public class FakeFile : IFile
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

        public FileAttributes Attributes
        {
            get => throw new NotImplementedException();
            set => throw new NotImplementedException();
        }

        Path IFileSystemInfo.Path => Path;

        public bool Exists => throw new NotImplementedException();
        public bool Hidden => throw new NotImplementedException();
    }
}