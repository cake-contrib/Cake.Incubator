﻿// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/. 

using System.Linq;
using Cake.Core;
using Cake.Testing;

namespace Cake.Incubator.Tests
{
    using System;
    using System.Collections.Generic;
    using Cake.Incubator.Project;
    using Cake.Incubator.StringExtensions;
    using FluentAssertions;
    using Xunit;

    public class TestProjects
    {
        // ReSharper disable once UnusedMember.Global
        public static IEnumerable<object[]> TestData
        {
            get
            {
                var env = FakeEnvironment.CreateUnixEnvironment();
                var fs = new FakeFileSystem(env);
                var validCsProjMSTestFile = fs.CreateFakeFile("CsProj_ValidMSTestFile".SafeLoad());
                var validCsProjXUnitTestFile = fs.CreateFakeFile("CsProj_ValidXUnitTestFile".SafeLoad());
                var validCsProjNUnitTestFile = fs.CreateFakeFile("CsProjValidNUnitTestFile".SafeLoad());
                var validCsProjFSUnitTestFile = fs.CreateFakeFile("CsProjValidFSUnitTestFile".SafeLoad());
                var validCsProjFixieTestFile = fs.CreateFakeFile("CsProjValidFixieTestFile".SafeLoad());
            
                return new List<object[]>
                {
                    new object[] { validCsProjMSTestFile },
                    new object[] { validCsProjNUnitTestFile },
                    new object[] { validCsProjXUnitTestFile },
                    new object[] { validCsProjFSUnitTestFile },
                    new object[] { validCsProjFixieTestFile },
                };
            }
        }
    }

    public class CustomProjectParserTests
    {
        private readonly FakeFile validCsProjFile;
        private readonly FakeFile validCsProjWebApplicationFile;
        private readonly FakeFile valid2017CsProjFile;
        private readonly FakeFile valid2017CsProjNetcoreFile;
        private readonly FakeFile valid2017CsProjNetstandardFile;
        private readonly FakeFile validCsProjConditionalReferenceFile;
        private readonly FakeFile validCsProjWithAbsoluteFilePaths;
        private readonly FakeFile validCsProjAppendTargetFrameworkFile;
        private readonly FakeFile validCsProjNoAppendTargetFrameworkFile;
        private readonly FakeFileSystem fs;

        public CustomProjectParserTests()
        {
            fs = new FakeFileSystem(FakeEnvironment.CreateUnixEnvironment());
            validCsProjFile = fs.CreateFakeFile("CsProj_ValidFile".SafeLoad());
            valid2017CsProjFile = fs.CreateFakeFile("VS2017_CsProj_ValidFile".SafeLoad());
            valid2017CsProjNetcoreFile = fs.CreateFakeFile("VS2017_CsProj_NetCoreDefault".SafeLoad());
            valid2017CsProjNetstandardFile = fs.CreateFakeFile("VS2017_CsProj_NetStandard_ValidFile".SafeLoad());
            validCsProjConditionalReferenceFile = fs.CreateFakeFile("CsProj_ConditionReference_ValidFile".SafeLoad());
            validCsProjWebApplicationFile = fs.CreateFakeFile("CsProj_ValidWebApplication".SafeLoad());
            validCsProjWithAbsoluteFilePaths = fs.CreateFakeFile("CsProj_AbsolutePath".SafeLoad());
            validCsProjAppendTargetFrameworkFile = fs.CreateFakeFile("CsProj_AppendTargetFramework".SafeLoad());
            validCsProjNoAppendTargetFrameworkFile = fs.CreateFakeFile("CsProj_NoAppendTargetFramework".SafeLoad());
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
            var fileName = valid2017CsProjNetcoreFile.Path.GetFilenameWithoutExtension().FullPath;
            
            result.Configuration.Should().Be("debug");
            result.OutputPath.ToString().Should().Be("bin/custom/netcoreapp1.1");
            result.OutputType.Should().Be("Exe");
            result.GetAssemblyFilePath().FullPath.Should().Be($"bin/custom/netcoreapp1.1/{fileName}.dll");
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

        [Fact]
        public void CustomProjectParser_RespectNoAppendTargetFrameworkToOutputPath_ForDebugConfig()
        {
            var result = validCsProjNoAppendTargetFrameworkFile.ParseProjectFile("debug");

            result.Configuration.Should().Be("debug");
            result.OutputPath.ToString().Should().Be("bin/debug");
        }

        [Fact]
        public void CustomProjectParser_RespectAppendTargetFrameworkToOutputPath_ForDebugConfig()
        {
            var result = validCsProjAppendTargetFrameworkFile.ParseProjectFile("debug");

            result.Configuration.Should().Be("debug");
            result.OutputPath.ToString().Should().Be("bin/debug/net48");
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

            result.References.FirstOrDefault(x =>
                x.HintPath?.FullPath ==
                    "C:/Program Files (x86)/Reference Assemblies/Microsoft/Framework/.NETFramework/v4.5.2/System.dll"
            ).Should().NotBeNull();
        }

        [Fact]
        public void CustomProjectParser_CanParseSample2017CsProjNetCoreFile_ForReleaseConfig()
        {
            var result = valid2017CsProjNetcoreFile.ParseProjectFile("release");
            var fileName = valid2017CsProjNetcoreFile.Path.GetFilenameWithoutExtension().FullPath;

            result.Configuration.Should().Be("release");
            result.OutputPath.ToString().Should().Be("bin/release/netcoreapp1.1");
            result.OutputType.Should().Be("Exe");
            result.GetAssemblyFilePath().FullPath.Should().Be($"bin/release/netcoreapp1.1/{fileName}.dll");
        }

        [Fact]
        public void CustomProjectParser_CanParseSample2017CsProjNetStandardFile_ForReleaseConfig()
        {
            var result = valid2017CsProjNetstandardFile.ParseProjectFile("debug");
            var fileName = valid2017CsProjNetstandardFile.Path.GetFilenameWithoutExtension().FullPath;
            
            result.Configuration.Should().Be("debug");
            result.OutputPath.ToString().Should().Be("bin/wayhey/netstandard1.6");
            result.OutputType.Should().Be("Library");
            result.GetAssemblyFilePath().FullPath.Should().Be($"bin/wayhey/netstandard1.6/{fileName}.dll");
        }

        [Fact]
        public void CustomProjectParser_CanParseCsProjWithConditionalReferences()
        {
            var result = validCsProjConditionalReferenceFile.ParseProjectFile("debug");

            result.References.Should().HaveCount(8).And.Contain(x =>
                x.Name.Equals("Microsoft.VisualStudio.QualityTools.UnitTestFramework"));
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

        [Fact]
        public void GetProjectProperty_ReturnsNull_ForMissingProperty()
        {
            // arrange
            var sut = validCsProjWebApplicationFile.ParseProjectFile("debug");

            // act
            var webApp = sut.GetProjectProperty("any");

            // assert
            webApp.Should().BeNull();
        }

        [Fact]
        public void GetProjectProperty_ReturnsValue_ForExistingProperty()
        {
            // arrange
            var sut = validCsProjWebApplicationFile.ParseProjectFile("debug");

            // act
            var webApp = sut.GetProjectProperty("AppDesignerFolder");

            // assert
            webApp.Should().Be("Properties");
        }

        [Fact]
        public void GetProjectProperty2017_ReturnsValue_ForExistingProperty()
        {
            // arrange
            var sut = valid2017CsProjNetcoreFile.ParseProjectFile("debug");

            // act
            var webApp = sut.GetProjectProperty("OutputPath");

            // assert
            webApp.Should().Be(@"bin\custom\");
        }
        
        [Fact]
        public void ForTfm_net451_IsNetFramework_IsSet()
        {
            var projectString = ProjectFileHelpers.GetNetCoreProjectWithString(
                "<PropertyGroup><TargetFramework>net451</TargetFramework></PropertyGroup>");
            var file = fs.CreateFakeFile(projectString);

            var project = file.ParseProjectFile("Release");
            project.IsNetFramework.Should().BeTrue();
            project.IsNetCore.Should().BeFalse();
            project.IsNetStandard.Should().BeFalse();
        }

        [Fact]
        public void ForTfm_netcoreapp31_IsNetCore_IsSet()
        {
            var projectString = ProjectFileHelpers.GetNetCoreProjectWithString(
                "<PropertyGroup><TargetFramework>netcoreapp3.1</TargetFramework></PropertyGroup>");
            var file = fs.CreateFakeFile(projectString);

            var project = file.ParseProjectFile("Release");
            project.IsNetFramework.Should().BeFalse();
            project.IsNetCore.Should().BeTrue();
            project.IsNetStandard.Should().BeFalse();
        }
        
        [Fact]
        public void ForTfm_netstandard20_IsNetStandard_IsSet()
        {
            var projectString = ProjectFileHelpers.GetNetCoreProjectWithString(
                "<PropertyGroup><TargetFramework>netstandard2.0</TargetFramework></PropertyGroup>");
            var file = fs.CreateFakeFile(projectString);

            var project = file.ParseProjectFile("Release");
            project.IsNetFramework.Should().BeFalse();
            project.IsNetCore.Should().BeFalse();
            project.IsNetStandard.Should().BeTrue();
        }
        
        [Fact]
        public void ForTfm_net50_IsNetCore_IsSet()
        {
            var projectString = ProjectFileHelpers.GetNetCoreProjectWithString(
                "<PropertyGroup><TargetFramework>net5.0</TargetFramework></PropertyGroup>");
            var file = fs.CreateFakeFile(projectString);

            var project = file.ParseProjectFile("Release");
            project.IsNetFramework.Should().BeFalse();
            project.IsNetCore.Should().BeTrue();
            project.IsNetStandard.Should().BeFalse();
        }
        
        [Fact]
        public void ForTfm_net50windows_IsNetCore_IsSet()
        {
            var projectString = ProjectFileHelpers.GetNetCoreProjectWithString(
                "<PropertyGroup><TargetFramework>net5.0-windows</TargetFramework></PropertyGroup>");
            var file = fs.CreateFakeFile(projectString);

            var project = file.ParseProjectFile("Release");
            project.IsNetFramework.Should().BeFalse();
            project.IsNetCore.Should().BeTrue();
            project.IsNetStandard.Should().BeFalse();
        }
        
        [Fact]
        public void ForTfm_net60ios_With_Version_IsNetCore_IsSet()
        {
            var projectString = ProjectFileHelpers.GetNetCoreProjectWithString(
                "<PropertyGroup><TargetFramework>net6.0-ios14.0</TargetFramework></PropertyGroup>");
            var file = fs.CreateFakeFile(projectString);

            var project = file.ParseProjectFile("Release");
            project.IsNetFramework.Should().BeFalse();
            project.IsNetCore.Should().BeTrue();
            project.IsNetStandard.Should().BeFalse();
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
}