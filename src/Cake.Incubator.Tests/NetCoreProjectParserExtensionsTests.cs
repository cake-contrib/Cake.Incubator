﻿// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/. 

using System.Reflection;
using Cake.Core;
using Cake.Testing;

namespace Cake.Incubator.Tests
{
    using System.Linq;
    using Cake.Incubator.Project;
    using Cake.Incubator.StringExtensions;
    using FluentAssertions;
    using Xunit;

    public class NetCoreProjectParserExtensionsTests
    {
        private static readonly FakeEnvironment env = FakeEnvironment.CreateUnixEnvironment();
        private static readonly FakeFileSystem fs = new FakeFileSystem(env);
        
        [Fact]
        public void ParseProject_GetsCorrectAssemblyName()
        {
            var netCoreProjectWithElement = ProjectFileHelpers.GetNetCoreProjectWithElement("AssemblyName", "a");
            var file = fs.CreateFakeFile(netCoreProjectWithElement);
            file.ParseProjectFile("test").AssemblyName.Should().Be("a");
        }

        [Fact]
        public void ParseProject_AssemblyName_DefaultsToProj()
        {
            var file = fs.CreateFakeFile(ProjectFileHelpers.GetNetCoreProjectWithString(null), "testAssembly");
            file.ParseProjectFile("test").AssemblyName.Should().Be("testAssembly");
        }

        [Fact]
        public void ParseProject_GetsCorrectConfiguration()
        {
            var file = fs.CreateFakeFile(ProjectFileHelpers.GetNetCoreProjectWithString(null));

            file.ParseProjectFile("test").Configuration.Should().Be("test");
        }

        [Theory]
        [InlineData("netcoreapp1.0")]
        [InlineData("netcoreapp1.1")]
        [InlineData("netcoreapp2.0")]
        public void ParseProject_SetsIsNetCore(string coreTarget)
        {
            var file = fs.CreateFakeFile(ProjectFileHelpers.GetNetCoreProjectWithElement("TargetFramework", coreTarget));
            file.ParseProjectFile("test").IsNetCore.Should().BeTrue();
        }

        [Theory]
        [InlineData("netstandard1.0")]
        [InlineData("netstandard1.1")]
        [InlineData("netstandard2.0")]
        public void ParseProject_SetsIsNetStandard(string coreTarget)
        {
            var file = fs.CreateFakeFile(ProjectFileHelpers.GetNetCoreProjectWithElement("TargetFramework", coreTarget));
            file.ParseProjectFile("test").IsNetStandard.Should().BeTrue();
        }

        [Fact]
        public void ParseProject_GetsMultiTargets()
        {
            var file = fs.CreateFakeFile(ProjectFileHelpers.GetNetCoreProjectWithElement("TargetFrameworks",
                "net45;net462;netstandard1.6;netcoreapp1.0;"));
            var result = file.ParseProjectFile("test");

            result.IsNetStandard.Should().BeTrue();
            result.IsNetFramework.Should().BeTrue();
            result.IsNetCore.Should().BeTrue();
            result.TargetFrameworkVersions.Should().HaveCount(4).And
                .BeEquivalentTo("net45", "net462", "netstandard1.6", "netcoreapp1.0");
        }
        
        [Fact]
        public void ParseProject_GetsFirstAssemblyForMultiTarget()
        {
            var file = fs.CreateFakeFile(ProjectFileHelpers.GetNetCoreProjectWithElement("TargetFrameworks",
                "net45;net462;netstandard1.6;netcoreapp1.0;"));
            var result = file.ParseProjectFile("test");
            var name = file.Path.GetFilenameWithoutExtension();

            result.GetAssemblyFilePath().FullPath.Should().Be($"bin/test/net45/{name}.dll");
        }
        
        [Fact]
        public void ParseProject_GetsAllAssemblyPathsForMultiTarget()
        {
            var file = fs.CreateFakeFile(ProjectFileHelpers.GetNetCoreProjectWithElement("TargetFrameworks",
                "net45;net462;netstandard1.6;"));
            var result = file.ParseProjectFile("test");
            var name = file.Path.GetFilenameWithoutExtension();

            var assemblyFilePaths = result.GetAssemblyFilePaths();
            assemblyFilePaths.Should().HaveCount(3);
            assemblyFilePaths.First().FullPath.Should().Be($"bin/test/net45/{name}.dll");
            assemblyFilePaths.Skip(1).First().FullPath.Should().Be($"bin/test/net462/{name}.dll");
            assemblyFilePaths.Skip(2).First().FullPath.Should().Be($"bin/test/netstandard1.6/{name}.dll");
        }

        [Fact]
        public void ParseProject_IsCoreTestProject()
        {
            var testProject =
                "<PropertyGroup><TargetFramework>netcoreapp2.0</TargetFramework></PropertyGroup><ItemGroup><PackageReference Include=\"Microsoft.NET.Test.Sdk\" Version=\"15.5.0\" /></ItemGroup>";
            var file = fs.CreateFakeFile(ProjectFileHelpers.GetNetCoreProjectWithString(testProject));
            file.ParseProjectFile("test").IsTestProject().Should().BeTrue();
            file.ParseProjectFile("test").IsDotNetCliTestProject().Should().BeTrue();
        }

        [Fact]
        public void ParseProject_IsCoreTestProjectForNetFxWithPackage()
        {
            var testProject =
                "<PropertyGroup><TargetFramework>net462</TargetFramework></PropertyGroup><ItemGroup><PackageReference Include=\"Microsoft.NET.Test.Sdk\" Version=\"15.5.0\" /></ItemGroup>";
            var file = fs.CreateFakeFile(ProjectFileHelpers.GetNetCoreProjectWithString(testProject));
            file.ParseProjectFile("test").IsTestProject().Should().BeTrue();
            file.ParseProjectFile("test").IsDotNetCliTestProject().Should().BeTrue();
        }

        [Fact]
        public void ParseProject_IsTestProject_ReturnsFalseForNetStandard()
        {
            var testProject =
                "<PropertyGroup><TargetFramework>netstandard1.0</TargetFramework></PropertyGroup><ItemGroup><PackageReference Include=\"Microsoft.NET.Test.Sdk\" Version=\"15.5.0\" /></ItemGroup>";
            var file = fs.CreateFakeFile(ProjectFileHelpers.GetNetCoreProjectWithString(testProject));
            file.ParseProjectFile("test").IsTestProject().Should().BeFalse();
            file.ParseProjectFile("test").IsDotNetCliTestProject().Should().BeFalse();
            file.ParseProjectFile("test").IsFrameworkTestProject().Should().BeFalse();
            file.ParseProjectFile("test").IsXUnitTestProject().Should().BeFalse();
            file.ParseProjectFile("test").IsNUnitTestProject().Should().BeFalse();
            file.ParseProjectFile("test").IsMSTestProject().Should().BeFalse();
            file.ParseProjectFile("test").IsFixieTestProject().Should().BeFalse();
            file.ParseProjectFile("test").IsExpectoTestProject().Should().BeFalse();
        }
        
        [Fact]
        public void ParseProject_IsCoreXUnitTestProject()
        {
            var testProject =
                "<PropertyGroup><TargetFramework>netcoreapp2.0</TargetFramework></PropertyGroup><ItemGroup><PackageReference Include=\"xunit\" Version=\"2.0.0\" /><PackageReference Include=\"Microsoft.NET.Test.Sdk\" Version=\"15.5.0\" /></ItemGroup>";
            var file = fs.CreateFakeFile(ProjectFileHelpers.GetNetCoreProjectWithString(testProject));
            file.ParseProjectFile("test").IsTestProject().Should().BeTrue();
            file.ParseProjectFile("test").IsDotNetCliTestProject().Should().BeTrue();
            file.ParseProjectFile("test").IsXUnitTestProject().Should().BeTrue();
        }
        
        [Fact]
        public void ParseProject_IsCoreNUnitTestProject()
        {
            var testProject =
                "<PropertyGroup><TargetFramework>netcoreapp2.0</TargetFramework></PropertyGroup><ItemGroup><PackageReference Include=\"nunit\" Version=\"2.0.0\" /><PackageReference Include=\"Microsoft.NET.Test.Sdk\" Version=\"15.5.0\" /></ItemGroup>";
            var file = fs.CreateFakeFile(ProjectFileHelpers.GetNetCoreProjectWithString(testProject));
            file.ParseProjectFile("test").IsTestProject().Should().BeTrue();
            file.ParseProjectFile("test").IsDotNetCliTestProject().Should().BeTrue();
            file.ParseProjectFile("test").IsNUnitTestProject().Should().BeTrue();
        }
        
        [Fact]
        public void ParseProject_IsCoreMSTestProject()
        {
            var testProject =
                "<PropertyGroup><TargetFramework>netcoreapp2.0</TargetFramework></PropertyGroup><ItemGroup><PackageReference Include=\"Microsoft.VisualStudio.TestPlatform\" Version=\"2.0.0\" /><PackageReference Include=\"Microsoft.NET.Test.Sdk\" Version=\"15.5.0\" /></ItemGroup>";
            var file = fs.CreateFakeFile(ProjectFileHelpers.GetNetCoreProjectWithString(testProject));
            file.ParseProjectFile("test").IsTestProject().Should().BeTrue();
            file.ParseProjectFile("test").IsDotNetCliTestProject().Should().BeTrue();
            file.ParseProjectFile("test").IsMSTestProject().Should().BeTrue();
        }
        
        [Fact]
        public void ParseProject_IsCoreFixieTestProject()
        {
            var testProject =
                "<PropertyGroup><TargetFramework>netcoreapp2.0</TargetFramework></PropertyGroup><ItemGroup><PackageReference Include=\"Fixie\" Version=\"2.0.0\" /><PackageReference Include=\"Microsoft.NET.Test.Sdk\" Version=\"15.5.0\" /></ItemGroup>";
            var file = fs.CreateFakeFile(ProjectFileHelpers.GetNetCoreProjectWithString(testProject));
            file.ParseProjectFile("test").IsTestProject().Should().BeTrue();
            file.ParseProjectFile("test").IsDotNetCliTestProject().Should().BeTrue();
            file.ParseProjectFile("test").IsFixieTestProject().Should().BeTrue();
        }
        
        [Fact]
        public void ParseProject_IsCoreExpectoTestProject()
        {
            var testProject =
                "<PropertyGroup><TargetFramework>netcoreapp2.0</TargetFramework></PropertyGroup><ItemGroup><PackageReference Include=\"Expecto\" Version=\"2.0.0\" /><PackageReference Include=\"Microsoft.NET.Test.Sdk\" Version=\"15.5.0\" /></ItemGroup>";
            var file = fs.CreateFakeFile(ProjectFileHelpers.GetNetCoreProjectWithString(testProject));
            file.ParseProjectFile("test").IsTestProject().Should().BeTrue();
            file.ParseProjectFile("test").IsDotNetCliTestProject().Should().BeTrue();
            file.ParseProjectFile("test").IsExpectoTestProject().Should().BeTrue();
        }

        [Fact]
        public void ParseProject_SetsIsNetCoreForWebSdk()
        {
            var file = fs.CreateFakeFile(ProjectFileHelpers.GetNetCoreProjectWithString(
                "<PropertyGroup><TargetFramework>netcoreapp2.0</TargetFramework></PropertyGroup>"));
            file.ParseProjectFile("test").IsNetCore.Should().BeTrue();
        }

        [Fact]
        public void ParseProject_DoesNotSetIsNetFramework()
        {
            var file = fs.CreateFakeFile(ProjectFileHelpers.GetNetCoreProjectWithElement("AssemblyName", "a"));
            file.ParseProjectFile("test").IsNetFramework.Should().BeFalse();
        }

        [Fact]
        public void ParseProject_RootNamespace_ReturnsIfSet()
        {
            var file = fs.CreateFakeFile(ProjectFileHelpers.GetNetCoreProjectWithElement("RootNamespace", "b"));
            file.ParseProjectFile("test").RootNameSpace.Should().Be("b");
        }

        [Fact]
        public void ParseProject_RootNamespace_FallbackToProjectName()
        {
            var file = fs.CreateFakeFile(ProjectFileHelpers.GetNetCoreProjectWithString(null), "./any/root.csproj");
            file.ParseProjectFile("test").RootNameSpace.Should().Be("root");
        }

        [Fact]
        public void ParseProject_GetsCorrectOutputType_WhenNoneSpecified()
        {
            var file = fs.CreateFakeFile(ProjectFileHelpers.GetNetCoreProjectWithElement("AssemblyName", "a"));
            file.ParseProjectFile("test").OutputType.Should().Be("Library");
        }

        [Fact]
        public void ParseProject_GetsCorrectOutputPath_WhenTargetFrameworkSpecified()
        {
            var file = fs.CreateFakeFile(ProjectFileHelpers.GetNetCoreProjectWithElement("TargetFramework", "net45"));
            file.ParseProjectFile("test").OutputPath.FullPath.Should().Be("bin/test/net45");
        }

        [Fact]
        public void ParseProject_GetsCorrectDefaultOutputPath_WhenTargetFrameworksSpecified()
        {
            var file = fs.CreateFakeFile(
                ProjectFileHelpers.GetNetCoreProjectWithElement("TargetFrameworks", "net45;netstandard1.6"));
            file.ParseProjectFile("test").OutputPath.FullPath.Should().Be("bin/test/net45");
        }

        [Fact]
        public void ParseProject_GetsCorrectDefaultOutputPaths_WhenTargetFrameworksSpecified()
        {
            var file = fs.CreateFakeFile(
                ProjectFileHelpers.GetNetCoreProjectWithElement("TargetFrameworks", "net45;netstandard1.6"));
            var paths = file.ParseProjectFile("test").OutputPaths;
            paths.Should().HaveCount(2);
            paths.First().FullPath.Should().Be("bin/test/net45");
            paths.Last().FullPath.Should().Be("bin/test/netstandard1.6");
        }

        [Fact]
        public void ParseProject_GetsCorrectDefaultOutputPaths_WhenTargetFrameworkSpecified()
        {
            var file = fs.CreateFakeFile(ProjectFileHelpers.GetNetCoreProjectWithElement("TargetFramework", "net45"));
            file.ParseProjectFile("test").OutputPaths.Should().ContainSingle().Which.FullPath.Should().Be("bin/test/net45");
        }


        [Fact]
        public void ParseProject_GetsCorrectOutputType_WhenLibrarySpecified()
        {
            var file = fs.CreateFakeFile(ProjectFileHelpers.GetNetCoreProjectWithElement("OutputType", "Library"));
            file.ParseProjectFile("test").OutputType.Should().Be("Library");
        }

        [Fact]
        public void ParseProject_GetsCorrectOutputType_WhenExeSpecified()
        {
            var file = fs.CreateFakeFile(ProjectFileHelpers.GetNetCoreProjectWithElement("OutputType", "Exe"));
            file.ParseProjectFile("test").OutputType.Should().Be("Exe");
        }

        [Fact]
        public void ParseProject_ApplicationIcons_ReturnsIfSet()
        {
            var file = fs.CreateFakeFile(ProjectFileHelpers.GetNetCoreProjectWithElement("ApplicationIcon", "fav.ico"));
            file.ParseProjectFile("test").NetCore.ApplicationIcon.Should().Be("fav.ico");
        }

        [Fact]
        public void ParseProject_GeneratePackageOnBuild_ReturnsIfSet()
        {
            var file = fs.CreateFakeFile(ProjectFileHelpers.GetNetCoreProjectWithElement("GeneratePackageOnBuild", "true"));
            file.ParseProjectFile("test").NetCore.GeneratePackageOnBuild.Should().BeTrue();
        }

        [Fact]
        public void ParseProject_GetsCorrectDefaultPlatform()
        {
            var file = fs.CreateFakeFile(ProjectFileHelpers.GetNetCoreProjectWithString(null));
            file.ParseProjectFile("test").Platform.Should().Be("AnyCPU");
        }

        [Fact]
        public void ParseProject_GetsSpecifiedPlatformElement()
        {
            var file = fs.CreateFakeFile(ProjectFileHelpers.GetNetCoreProjectWithString(null));
            file.ParseProjectFile("test", "x86").Platform.Should().Be("x86");
        }

        [Fact]
        public void ParseProject_IgnoresProjectGuid()
        {
            var file = fs.CreateFakeFile(ProjectFileHelpers.GetNetCoreProjectWithString(null));
            file.ParseProjectFile("test").ProjectGuid.Should().BeNull();
        }

        [Fact]
        public void ParseProject_IgnoresProjectTypeGuids()
        {
            var file = fs.CreateFakeFile(ProjectFileHelpers.GetNetCoreProjectWithString(null));
            file.ParseProjectFile("test").ProjectTypeGuids.Should().BeNull();
        }

        [Fact]
        public void ParseProject_IgnoresTargetFrameworkProfile()
        {
            var file = fs.CreateFakeFile(ProjectFileHelpers.GetNetCoreProjectWithString(null));
            file.ParseProjectFile("test").TargetFrameworkProfile.Should().BeNull();
        }

        [Fact]
        public void ParseProject_TargetFrameworkVersion_NullIfUnspecified()
        {
            var file = fs.CreateFakeFile(ProjectFileHelpers.GetNetCoreProjectWithString(null));
            file.ParseProjectFile("test").TargetFrameworkVersion.Should().BeNull();
        }

        [Fact]
        public void ParseProject_TargetFrameworkVersion_ReturnsExpected()
        {
            var file = fs.CreateFakeFile(
                ProjectFileHelpers.GetNetCoreProjectWithElement("TargetFramework", "Netstandard1.1"));
            file.ParseProjectFile("test").TargetFrameworkVersion.Should().Be("Netstandard1.1");
        }

        [Fact]
        public void ParseProject_NetCore_IsNotNull()
        {
            var file = fs.CreateFakeFile(ProjectFileHelpers.GetNetCoreProjectWithString(null));
            file.ParseProjectFile("test").NetCore.Should().NotBeNull();
        }

        [Fact]
        public void ParseProject_NetCore_AllowUnsafe_DefaultsFalse()
        {
            var file = fs.CreateFakeFile(ProjectFileHelpers.GetNetCoreProjectWithString(null));
            file.ParseProjectFile("test").NetCore.AllowUnsafeBlocks.Should().BeFalse();
        }

        [Fact]
        public void ParseProject_NetCore_AllowUnsafe_TrueIfSet()
        {
            var file = fs.CreateFakeFile(ProjectFileHelpers.GetNetCoreProjectWithElement("AllowUnsafeBlocks", "true"));
            file.ParseProjectFile("test").NetCore.AllowUnsafeBlocks.Should().BeTrue();
        }

        [Fact]
        public void ParseProject_NetCore_DebugSymbols_TrueIfSet()
        {
            var file = fs.CreateFakeFile(ProjectFileHelpers.GetNetCoreProjectWithElement("DebugSymbols", "true"));
            file.ParseProjectFile("test").NetCore.DebugSymbols.Should().BeTrue();
        }

        [Fact]
        public void ParseProject_NetCore_AssemblyOriginatorKeyFile_ReturnsIfSet()
        {
            var file = fs.CreateFakeFile(
                ProjectFileHelpers.GetNetCoreProjectWithElement("AssemblyOriginatorKeyFile", "key.snk"));
            file.ParseProjectFile("test").NetCore.AssemblyOriginatorKeyFile.Should().Be("key.snk");
        }

        [Fact]
        public void ParseProject_NetCore_BuildOutputTargetFolder_ReturnsIfSet()
        {
            var file = fs.CreateFakeFile(
                ProjectFileHelpers.GetNetCoreProjectWithElement("BuildOutputTargetFolder", "./oompa/loompa"));
            file.ParseProjectFile("test").NetCore.BuildOutputTargetFolder.Should().Be("./oompa/loompa");
        }

        [Fact]
        public void ParseProject_NetCore_IncludeBuildOutput_ReturnsIfSet()
        {
            var file = fs.CreateFakeFile(ProjectFileHelpers.GetNetCoreProjectWithElement("IncludeBuildOutput", "true"));
            file.ParseProjectFile("test").NetCore.IncludeBuildOutput.Should().BeTrue();
        }

        [Fact]
        public void ParseProject_NetCore_IncludeContentInPack_ReturnsIfSet()
        {
            var file = fs.CreateFakeFile(ProjectFileHelpers.GetNetCoreProjectWithElement("IncludeContentInPack", "true"));
            file.ParseProjectFile("test").NetCore.IncludeContentInPack.Should().BeTrue();
        }

        [Fact]
        public void ParseProject_NetCore_IncludeSource_ReturnsIfSet()
        {
            var file = fs.CreateFakeFile(ProjectFileHelpers.GetNetCoreProjectWithElement("IncludeSource", "true"));
            file.ParseProjectFile("test").NetCore.IncludeSource.Should().BeTrue();
        }

        [Fact]
        public void ParseProject_NetCore_IncludeSymbols_ReturnsIfSet()
        {
            var file = fs.CreateFakeFile(ProjectFileHelpers.GetNetCoreProjectWithElement("IncludeSymbols", "true"));
            file.ParseProjectFile("test").NetCore.IncludeSymbols.Should().BeTrue();
        }

        [Fact]
        public void ParseProject_NetCore_IsPackable_ReturnsIfSet()
        {
            var file = fs.CreateFakeFile(ProjectFileHelpers.GetNetCoreProjectWithElement("IsPackable", "true"));
            file.ParseProjectFile("test").NetCore.IsPackable.Should().BeTrue();
        }

        [Fact]
        public void ParseProject_NetCore_IsPackable_DefaultFallback()
        {
            var file = fs.CreateFakeFile(ProjectFileHelpers.GetNetCoreProjectWithString(null));
            file.ParseProjectFile("test").NetCore.IsPackable.Should().BeTrue();
        }

        [Fact]
        public void ParseProject_NetCore_IsTool_ReturnsIfSet()
        {
            var file = fs.CreateFakeFile(ProjectFileHelpers.GetNetCoreProjectWithElement("IsTool", "true"));
            file.ParseProjectFile("test").NetCore.IsTool.Should().BeTrue();
        }

        [Fact]
        public void ParseProject_NetCore_NoPackageAnalysis_ReturnsIfSet()
        {
            var file = fs.CreateFakeFile(ProjectFileHelpers.GetNetCoreProjectWithElement("NoPackageAnalysis", "true"));
            file.ParseProjectFile("test").NetCore.NoPackageAnalysis.Should().BeTrue();
        }

        [Fact]
        public void ParseProject_NetCore_MinClientVersion_ReturnsIfSet()
        {
            var file = fs.CreateFakeFile(ProjectFileHelpers.GetNetCoreProjectWithElement("MinClientVersion", "mouse"));
            file.ParseProjectFile("test").NetCore.MinClientVersion.Should().Be("mouse");
        }

        [Fact]
        public void ParseProject_NetCore_NuspecBasePath_ReturnsIfSet()
        {
            var file = fs.CreateFakeFile(
                ProjectFileHelpers.GetNetCoreProjectWithElement("NuspecBasePath", "./all/about/the/base"));
            file.ParseProjectFile("test").NetCore.NuspecBasePath.Should().Be("./all/about/the/base");
        }

        [Fact]
        public void ParseProject_NetCore_NuspecFile_ReturnsIfSet()
        {
            var file = fs.CreateFakeFile(ProjectFileHelpers.GetNetCoreProjectWithElement("NuspecFile", "./re.spec"));
            file.ParseProjectFile("test").NetCore.NuspecFile.Should().Be("./re.spec");
        }

        [Fact]
        public void ParseProject_NetCore_NuspecProperties_ReturnsIfSet()
        {
            var file = fs.CreateFakeFile(
                ProjectFileHelpers.GetNetCoreProjectWithElement("NuspecProperties", "edgar=poe;jimmini=cricket;"));
            var props = file.ParseProjectFile("test").NetCore.NuspecProperties;
            props.Cast<object>().Should().HaveCount(2);
            props["edgar"].Should().Be("poe");
            props["jimmini"].Should().Be("cricket");
        }

        [Fact]
        public void ParseProject_NetCore_PackageOutputPath_ReturnsIfSet()
        {
            var file = fs.CreateFakeFile(
                ProjectFileHelpers.GetNetCoreProjectWithElement("PackageOutputPath", "./row/row/row/yerboat"));
            file.ParseProjectFile("test").NetCore.PackageOutputPath.Should().Be("./row/row/row/yerboat");
        }

        [Fact]
        public void ParseProject_NetCore_Title_ReturnsIfSet()
        {
            var file = fs.CreateFakeFile(ProjectFileHelpers.GetNetCoreProjectWithElement("Title", "Count Dracula"));
            file.ParseProjectFile("test").NetCore.Title.Should().Be("Count Dracula");
        }

        [Fact]
        public void ParseProject_NetCore_ContentTargetFolders_ReturnsIfSet()
        {
            var file = fs.CreateFakeFile(
                ProjectFileHelpers.GetNetCoreProjectWithElement("ContentTargetFolders", "content;contentFiles;;"));
            file.ParseProjectFile("test").NetCore.ContentTargetFolders.Should().BeEquivalentTo("content", "contentFiles");
        }

        [Fact]
        public void ParseProject_NetCore_AssemblyTitle_ReturnsIfSet()
        {
            var file = fs.CreateFakeFile(ProjectFileHelpers.GetNetCoreProjectWithElement("AssemblyTitle", "my title"));
            file.ParseProjectFile("test").NetCore.AssemblyTitle.Should().Be("my title");
        }

        [Fact]
        public void ParseProject_NetCore_AssemblyTitle_OverridesAssemblyName()
        {
            var file = fs.CreateFakeFile(ProjectFileHelpers.GetNetCoreProjectWithString(
                "<AssemblyName>original</AssemblyName><AssemblyTitle>my title</AssemblyTitle>"));
            file.ParseProjectFile("test").NetCore.AssemblyTitle.Should().Be("my title");
        }

        [Fact]
        public void ParseProject_NetCore_AssemblyTitle_AssemblyNameFallback()
        {
            var file = fs.CreateFakeFile(
                ProjectFileHelpers.GetNetCoreProjectWithString("<AssemblyName>original</AssemblyName>"));
            file.ParseProjectFile("test").NetCore.AssemblyTitle.Should().Be("original");
        }

        [Fact]
        public void ParseProject_NetCore_AssemblyTitle_ProjNameFallback()
        {
            var file = fs.CreateFakeFile(ProjectFileHelpers.GetNetCoreProjectWithString(null), "./projName.csproj");
            file.ParseProjectFile("test").NetCore.AssemblyTitle.Should().Be("projName");
        }

        [Fact]
        public void ParseProject_NetCore_Authors_ReturnsIfSet()
        {
            var file = fs.CreateFakeFile(ProjectFileHelpers.GetNetCoreProjectWithElement("Authors", "one;two;"));
            file.ParseProjectFile("test").NetCore.Authors.Should().BeEquivalentTo("one", "two");
        }

        [Fact]
        public void ParseProject_NetCore_Company_ReturnsIfSet()
        {
            var file = fs.CreateFakeFile(ProjectFileHelpers.GetNetCoreProjectWithElement("Company", "Acme inc"));
            file.ParseProjectFile("test").NetCore.Company.Should().Be("Acme inc");
        }

        [Fact]
        public void ParseProject_NetCore_Copyright_ReturnsIfSet()
        {
            var file = fs.CreateFakeFile(ProjectFileHelpers.GetNetCoreProjectWithElement("Copyright", "moi!"));
            file.ParseProjectFile("test").NetCore.Copyright.Should().Be("moi!");
        }

        [Fact]
        public void ParseProject_NetCore_DebugType_ReturnsIfSet()
        {
            var file = fs.CreateFakeFile(ProjectFileHelpers.GetNetCoreProjectWithElement("DebugType", "Embedded"));
            file.ParseProjectFile("test").NetCore.DebugType.Should().Be("Embedded");
        }

        [Fact]
        public void ParseProject_NetCore_DefineConstants_ReturnsIfSet()
        {
            var file = fs.CreateFakeFile(ProjectFileHelpers.GetNetCoreProjectWithElement("DefineConstants", "DEBUG;WIN;"));
            file.ParseProjectFile("test").NetCore.DefineConstants.Should().BeEquivalentTo("DEBUG", "WIN");
        }

        [Theory]
        [InlineData("test", "oil")]
        [InlineData("bananas", "shoes")]
        public void ParseProject_NetCore_DefineConstants_ReturnsEmptyIfNoConfigMatch(string config, string platform)
        {
            var file = fs.CreateFakeFile(ProjectFileHelpers.GetNetCoreProjectElementWithConfig("DefineConstants",
                "Birth;Death;Taxes;", config, platform));
            file.ParseProjectFile("test", "shoes").NetCore.DefineConstants.Should().BeEmpty();
        }

        [Fact]
        public void ParseProject_NetCore_DefineConstants_ReturnsCorrectConfigMatch()
        {
            var file = fs.CreateFakeFile(ProjectFileHelpers.GetNetCoreProjectElementWithConfig("DefineConstants",
                "Birth;Death;Taxes;", "test", "shoes"));
            file.ParseProjectFile("test", "shoes").NetCore.DefineConstants.Should()
                .BeEquivalentTo("Birth", "Death", "Taxes");
        }

        [Fact]
        public void ParseProject_NetCore_DefineConstants_ReturnsCorrectParentConfigMatch()
        {
            var file = fs.CreateFakeFile(ProjectFileHelpers.GetNetCoreProjectElementWithParentConfig("DefineConstants",
                "Birth;Death;Taxes;", "test", "shoes"));
            file.ParseProjectFile("test", "shoes").NetCore.DefineConstants.Should()
                .BeEquivalentTo("Birth", "Death", "Taxes");
        }

        [Fact]
        public void ParseProject_NetCore_DefineConstants_ReturnsCorrectFallbackIfConditionsDontMatch()
        {
            var projString =
                @"<Project sdk=""Microsoft.NET.Sdk""><DefineConstants>Hoka;Loca;Moca;</DefineConstants><PropertyGroup Condition=""'$(Configuration)|$(Platform)'=='test|shoes'""><DefineConstants>Birth;Death;Taxes;</DefineConstants></PropertyGroup></Project>";
            var file = fs.CreateFakeFile(ProjectFileHelpers.GetNetCoreProjectWithString(projString));
            file.ParseProjectFile("Roca", "shoes").NetCore.DefineConstants.Should().BeEquivalentTo("Hoka", "Loca", "Moca");
        }

        [Fact]
        public void ParseProject_NetCore_Description_ReturnsIfSet()
        {
            var file = fs.CreateFakeFile(ProjectFileHelpers.GetNetCoreProjectWithElement("Description", "Howdy"));
            file.ParseProjectFile("test").NetCore.Description.Should().Be("Howdy");
        }

        [Fact]
        public void ParseProject_NetCore_WarningLevel_ReturnsIfSet()
        {
            var file = fs.CreateFakeFile(ProjectFileHelpers.GetNetCoreProjectWithElement("WarningLevel", "3"));
            file.ParseProjectFile("test").NetCore.WarningLevel.Should().Be("3");
        }

        [Fact]
        public void ParseProject_NetCore_Optimize_ReturnsIfSet()
        {
            var file = fs.CreateFakeFile(ProjectFileHelpers.GetNetCoreProjectWithElement("Optimize", "true"));
            file.ParseProjectFile("test").NetCore.Optimize.Should().BeTrue();
        }

        [Fact]
        public void ParseProject_NetCore_DocumentationFile_ReturnsIfSet()
        {
            var file = fs.CreateFakeFile(ProjectFileHelpers.GetNetCoreProjectWithElement("DocumentationFile", ".a/b/c.xml"));
            file.ParseProjectFile("test").NetCore.DocumentationFile.Should().Be(".a/b/c.xml");
        }

        [Fact]
        public void ParseProject_NetCore_GenerateDocumentationFile_ReturnsIfSet()
        {
            var file = fs.CreateFakeFile(
                ProjectFileHelpers.GetNetCoreProjectWithElement("GenerateDocumentationFile", "true"));
            file.ParseProjectFile("test").NetCore.GenerateDocumentationFile.Should().BeTrue();
        }

        [Fact]
        public void ParseProject_NetCore_NetStandardImplicitPackageVersion_ReturnsIfSet()
        {
            var file = fs.CreateFakeFile(ProjectFileHelpers.GetNetCoreProjectWithElement("NetStandardImplicitPackageVersion",
                "1.1.0-beta5-alpha3-preview6-final3"));
            file.ParseProjectFile("test").NetCore.NetStandardImplicitPackageVersion.Should()
                .Be("1.1.0-beta5-alpha3-preview6-final3");
        }

        [Fact]
        public void ParseProject_NetCore_NeutralLanguage_ReturnsIfSet()
        {
            var file = fs.CreateFakeFile(ProjectFileHelpers.GetNetCoreProjectWithElement("NeutralLanguage", "PigLatin"));
            file.ParseProjectFile("test").NetCore.NeutralLanguage.Should().Be("PigLatin");
        }

        [Fact]
        public void ParseProject_NetCore_NoWarn_ReturnsIfSet()
        {
            var file = fs.CreateFakeFile(
                ProjectFileHelpers.GetNetCoreProjectWithElement("NoWarn", "$(nowarn);CS3132;CS2534;"));
            file.ParseProjectFile("test").NetCore.NoWarn.Should().BeEquivalentTo("CS3132", "CS2534");
        }

        [Fact]
        public void ParseProject_NetCore_TreatSpecificWarningsAsErrors_ReturnsIfSet()
        {
            var file = fs.CreateFakeFile(
                ProjectFileHelpers.GetNetCoreProjectWithElement("TreatSpecificWarningsAsErrors", ";CS3132;CS2534;"));
            file.ParseProjectFile("test").NetCore.TreatSpecificWarningsAsErrors.Should().BeEquivalentTo("CS3132", "CS2534");
        }

        [Fact]
        public void ParseProject_NetCore_GenerateSerializationAssemblies_ReturnsIfSet()
        {
            var file = fs.CreateFakeFile(
                ProjectFileHelpers.GetNetCoreProjectWithElement("GenerateSerializationAssemblies", "autobots"));
            file.ParseProjectFile("test").NetCore.GenerateSerializationAssemblies.Should().Be("autobots");
        }

        [Fact]
        public void ParseProject_NetCore_LangVersion_ReturnsIfSet()
        {
            var file = fs.CreateFakeFile(ProjectFileHelpers.GetNetCoreProjectWithElement("LangVersion", "GROOT"));
            file.ParseProjectFile("test").NetCore.LangVersion.Should().Be("GROOT");
        }

        [Fact]
        public void ParseProject_NetCore_PackageIconUrl_ReturnsIfSet()
        {
            var file = fs.CreateFakeFile(
                ProjectFileHelpers.GetNetCoreProjectWithElement("PackageIconUrl", "http://acme.inc/fav.ico"));
            file.ParseProjectFile("test").NetCore.PackageIconUrl.Should().Be("http://acme.inc/fav.ico");
        }

        [Fact]
        public void ParseProject_NetCore_Product_ReturnsIfSet()
        {
            var file = fs.CreateFakeFile(ProjectFileHelpers.GetNetCoreProjectWithElement("Product", "acme skyhooks"));
            file.ParseProjectFile("test").NetCore.Product.Should().Be("acme skyhooks");
        }

        [Fact]
        public void ParseProject_NetCore_PackageId_ReturnsIfSet()
        {
            var file = fs.CreateFakeFile(ProjectFileHelpers.GetNetCoreProjectWithElement("PackageId", "AcmeInc"));
            file.ParseProjectFile("test").NetCore.PackageId.Should().Be("AcmeInc");
        }

        [Fact]
        public void ParseProject_NetCore_PackageId_AssemblyNameFallback()
        {
            var file = fs.CreateFakeFile(ProjectFileHelpers.GetNetCoreProjectWithElement("AssemblyName", "Woot"));
            file.ParseProjectFile("test").NetCore.PackageId.Should().Be("Woot");
        }

        [Fact]
        public void ParseProject_NetCore_PackageId_ProjectNameFallback()
        {
            var file = fs.CreateFakeFile(ProjectFileHelpers.GetNetCoreProjectWithString(null), "./Twit/Twoot.fsproj");
            file.ParseProjectFile("test").NetCore.PackageId.Should().Be("Twoot");
        }

        [Fact]
        public void ParseProject_NetCore_PackageLicenseUrl_ReturnsIfSet()
        {
            var file = fs.CreateFakeFile(
                ProjectFileHelpers.GetNetCoreProjectWithElement("PackageLicenseUrl", "http://death.star/legal"));
            file.ParseProjectFile("test").NetCore.PackageLicenseUrl.Should().Be("http://death.star/legal");
        }

        [Fact]
        public void ParseProject_NetCore_PackageProjectUrl_ReturnsIfSet()
        {
            var file = fs.CreateFakeFile(ProjectFileHelpers.GetNetCoreProjectWithElement("PackageProjectUrl",
                "https://github.com/cake-contrib/Cake.Incubator"));
            file.ParseProjectFile("test").NetCore.PackageProjectUrl.Should()
                .Be("https://github.com/cake-contrib/Cake.Incubator");
        }

        [Fact]
        public void ParseProject_NetCore_PackageReleaseNotes_ReturnsIfSet()
        {
            var file = fs.CreateFakeFile(ProjectFileHelpers.GetNetCoreProjectWithElement("PackageReleaseNotes",
                "Wery well, we shall weelesse wodger"));
            file.ParseProjectFile("test").NetCore.PackageReleaseNotes.Should().Be("Wery well, we shall weelesse wodger");
        }

        [Fact]
        public void ParseProject_NetCore_PackageRequireLicenseAcceptance_ReturnsIfSet()
        {
            var file = fs.CreateFakeFile(
                ProjectFileHelpers.GetNetCoreProjectWithElement("PackageRequireLicenseAcceptance", "true"));
            file.ParseProjectFile("test").NetCore.PackageRequireLicenseAcceptance.Should().BeTrue();
        }

        [Fact]
        public void ParseProject_NetCore_PackageTags_ReturnsIfSet()
        {
            var file = fs.CreateFakeFile(
                ProjectFileHelpers.GetNetCoreProjectWithElement("PackageTags", "Eenie;Meenie;Moe;"));
            file.ParseProjectFile("test").NetCore.PackageTags.Should().BeEquivalentTo("Eenie", "Meenie", "Moe");
        }

        [Fact]
        public void ParseProject_NetCore_PackageTargetFallbacks_ReturnsIfSet()
        {
            var file = fs.CreateFakeFile(ProjectFileHelpers.GetNetCoreProjectWithElement("PackageTargetFallback",
                "net45;net46;netunstandard-10.gamma.cheese;"));
            file.ParseProjectFile("test").NetCore.PackageTargetFallbacks.Should()
                .BeEquivalentTo("net45", "net46", "netunstandard-10.gamma.cheese");
        }

        [Fact]
        public void ParseProject_NetCore_PreserveCompilationContext_ReturnsIfSet()
        {
            var file = fs.CreateFakeFile(
                ProjectFileHelpers.GetNetCoreProjectWithElement("PreserveCompilationContext", "true"));
            file.ParseProjectFile("test").NetCore.PreserveCompilationContext.Should().BeTrue();
        }

        [Fact]
        public void ParseProject_NetCore_PublicSign_ReturnsIfSet()
        {
            var file = fs.CreateFakeFile(ProjectFileHelpers.GetNetCoreProjectWithElement("PublicSign", "true"));
            file.ParseProjectFile("test").NetCore.PublicSign.Should().BeTrue();
        }

        [Fact]
        public void ParseProject_NetCore_DelaySign_ReturnsIfSet()
        {
            var file = fs.CreateFakeFile(ProjectFileHelpers.GetNetCoreProjectWithElement("DelaySign", "true"));
            file.ParseProjectFile("test").NetCore.DelaySign.Should().BeTrue();
        }

        [Fact]
        public void ParseProject_NetCore_RepositoryType_ReturnsIfSet()
        {
            var file = fs.CreateFakeFile(ProjectFileHelpers.GetNetCoreProjectWithElement("RepositoryType", "gitgat"));
            file.ParseProjectFile("test").NetCore.RepositoryType.Should().Be("gitgat");
        }

        [Fact]
        public void ParseProject_NetCore_RepositoryType_DefaultFallback()
        {
            var file = fs.CreateFakeFile(ProjectFileHelpers.GetNetCoreProjectWithString(null));
            file.ParseProjectFile("test").NetCore.RepositoryType.Should().Be("git");
        }

        [Fact]
        public void ParseProject_NetCore_RepositoryUrl_ReturnsIfSet()
        {
            var file = fs.CreateFakeFile(ProjectFileHelpers.GetNetCoreProjectWithElement("RepositoryUrl", "git://gat"));
            file.ParseProjectFile("test").NetCore.RepositoryUrl.Should().Be("git://gat");
        }

        [Fact]
        public void ParseProject_NetCore_RuntimeFrameworkVersion_ReturnsIfSet()
        {
            var file = fs.CreateFakeFile(ProjectFileHelpers.GetNetCoreProjectWithElement("RuntimeFrameworkVersion",
                "minustwentzillion.epsilonEcho.woof"));
            file.ParseProjectFile("test").NetCore.RuntimeFrameworkVersion.Should().Be("minustwentzillion.epsilonEcho.woof");
        }

        [Fact]
        public void ParseProject_NetCore_RuntimeIdentifiers_ReturnsIfSet()
        {
            var file = fs.CreateFakeFile(
                ProjectFileHelpers.GetNetCoreProjectWithElement("RuntimeIdentifiers", "charlie;delta;echo;;"));
            file.ParseProjectFile("test").NetCore.RuntimeIdentifiers.Should().BeEquivalentTo("charlie", "delta", "echo");
        }

        [Fact]
        public void ParseProject_NetCore_Sdk_ReturnsIfSet()
        {
            var file = fs.CreateFakeFile(ProjectFileHelpers.GetNetCoreProjectWithString(null));
            file.ParseProjectFile("test").NetCore.Sdk
                .ToLowerInvariant().Should().BeEquivalentTo("Microsoft.Net.Sdk".ToLowerInvariant());
        }

        [Fact]
        public void ParseProject_NetCore_WebSdk_ReturnsIfSet()
        {
            var file = fs.CreateFakeFile(ProjectFileHelpers.GetNetCoreWebProjectWithString(null));
            file.ParseProjectFile("test").NetCore.Sdk
                .ToLowerInvariant().Should().BeEquivalentTo("Microsoft.Net.Sdk.Web".ToLowerInvariant());
        }

        [Fact]
        public void ParseProject_NetCore_SignAssembly_ReturnsIfSet()
        {
            var file = fs.CreateFakeFile(ProjectFileHelpers.GetNetCoreProjectWithElement("SignAssembly", "TRUE"));
            file.ParseProjectFile("test").NetCore.SignAssembly.Should().BeTrue();
        }

        [Fact]
        public void ParseProject_NetCore_TargetFrameworks_ReturnsIfSet()
        {
            var file = fs.CreateFakeFile(
                ProjectFileHelpers.GetNetCoreProjectWithElement("TargetFrameworks", "net45;net46;12345;;"));
            file.ParseProjectFile("test").NetCore.TargetFrameworks.Should().BeEquivalentTo("net45", "net46", "12345");
        }

        [Fact]
        public void ParseProject_NetCore_TargetFrameworks_TargetFrameworkFallback()
        {
            var file = fs.CreateFakeFile(ProjectFileHelpers.GetNetCoreProjectWithElement("TargetFramework", "net45"));
            file.ParseProjectFile("test").NetCore.TargetFrameworks.Should().BeEquivalentTo("net45");
        }

        [Fact]
        public void ParseProject_NetCore_TreatWarningsAsErrors_ReturnsIfSet()
        {
            var file = fs.CreateFakeFile(ProjectFileHelpers.GetNetCoreProjectWithElement("TreatWarningsAsErrors", "true"));
            file.ParseProjectFile("test").NetCore.TreatWarningsAsErrors.Should().BeTrue();
        }

        [Fact]
        public void ParseProject_NetCore_Version_ReturnsIfSet()
        {
            var file = fs.CreateFakeFile(ProjectFileHelpers.GetNetCoreProjectWithString(
                "<VersionPrefix>1.2.3</VersionPrefix><VersionSuffix>5813</VersionSuffix><Version>8.9.10</Version>"));
            file.ParseProjectFile("test").NetCore.Version.Should().Be("1.2.3.5813");
        }

        [Fact]
        public void ParseProject_NetCore_Version_ReturnsIfSemver()
        {
            var file = fs.CreateFakeFile(ProjectFileHelpers.GetNetCoreProjectWithString(
                "<VersionPrefix>1.2.3</VersionPrefix><VersionSuffix>alpha5813</VersionSuffix><Version>8.9.10</Version>"));
            file.ParseProjectFile("test").NetCore.Version.Should().Be("1.2.3-alpha5813");
        }

        [Fact]
        public void ParseProject_NetCore_Version_VersionFallback()
        {
            var file = fs.CreateFakeFile(ProjectFileHelpers.GetNetCoreProjectWithElement("Version", "1.2.3.5"));
            file.ParseProjectFile("test").NetCore.Version.Should().Be("1.2.3.5");
        }

        [Fact]
        public void ParseProject_NetCore_AssemblyVersion_ReturnsIfSet()
        {
            var file = fs.CreateFakeFile(ProjectFileHelpers.GetNetCoreProjectWithElement("AssemblyVersion", "1.4.3.5"));
            file.ParseProjectFile("test").NetCore.AssemblyVersion.Should().Be("1.4.3.5");
        }

        [Fact]
        public void ParseProject_NetCore_AssemblyVersion_DefaultFallback()
        {
            var file = fs.CreateFakeFile(ProjectFileHelpers.GetNetCoreProjectWithString(null));
            file.ParseProjectFile("test").NetCore.AssemblyVersion.Should().Be("1.0.0.0");
        }

        [Fact]
        public void ParseProject_NetCore_FileVersion_ReturnsIfSet()
        {
            var file = fs.CreateFakeFile(ProjectFileHelpers.GetNetCoreProjectWithElement("FileVersion", "1.4.3.5"));
            file.ParseProjectFile("test").NetCore.FileVersion.Should().Be("1.4.3.5");
        }

        [Fact]
        public void ParseProject_NetCore_FileVersion_DefaultFallback()
        {
            var file = fs.CreateFakeFile(ProjectFileHelpers.GetNetCoreProjectWithString(null));
            file.ParseProjectFile("test").NetCore.FileVersion.Should().Be("1.0.0.0");
        }

        [Fact]
        public void ParseProject_NetCore_Version_DefaultFallback()
        {
            var file = fs.CreateFakeFile(ProjectFileHelpers.GetNetCoreProjectWithString(null));
            file.ParseProjectFile("test").NetCore.Version.Should().Be("1.0.0");
        }

        [Fact]
        public void ParseProject_NetCore_ConcurrentGarbageCollection_ReturnsIfSet()
        {
            var file = fs.CreateFakeFile(
                ProjectFileHelpers.GetNetCoreProjectWithElement("ConcurrentGarbageCollection", "true"));
            file.ParseProjectFile("test").NetCore.RuntimeOptions.ConcurrentGarbageCollection.Should().BeTrue();
        }

        [Fact]
        public void ParseProject_NetCore_RetainVMGarbageCollection_ReturnsIfSet()
        {
            var file = fs.CreateFakeFile(
                ProjectFileHelpers.GetNetCoreProjectWithElement("RetainVMGarbageCollection", "true"));
            file.ParseProjectFile("test").NetCore.RuntimeOptions.RetainVMGarbageCollection.Should().BeTrue();
        }

        [Fact]
        public void ParseProject_NetCore_ServerGarbageCollection_ReturnsIfSet()
        {
            var file = fs.CreateFakeFile(ProjectFileHelpers.GetNetCoreProjectWithElement("ServerGarbageCollection", "true"));
            file.ParseProjectFile("test").NetCore.RuntimeOptions.ServerGarbageCollection.Should().BeTrue();
        }

        [Fact]
        public void ParseProject_NetCore_ThreadPoolMaxThreads_ReturnsIfSet()
        {
            var file = fs.CreateFakeFile(ProjectFileHelpers.GetNetCoreProjectWithElement("ThreadPoolMaxThreads", "3"));
            file.ParseProjectFile("test").NetCore.RuntimeOptions.ThreadPoolMaxThreads.Should().Be(3);
        }

        [Fact]
        public void ParseProject_NetCore_ThreadPoolMinThreads_ReturnsIfSet()
        {
            var file = fs.CreateFakeFile(ProjectFileHelpers.GetNetCoreProjectWithElement("ThreadPoolMinThreads", "3"));
            file.ParseProjectFile("test").NetCore.RuntimeOptions.ThreadPoolMinThreads.Should().Be(3);
        }

        [Fact]
        public void ParseProject_NetCore_ProjectReferences_ReturnIfSet()
        {
            var file = fs.CreateFakeFile(
                ProjectFileHelpers.GetNetCoreProjectWithString(
                    "<ProjectReference Include=\"..\\a\\b.csproj\" /><ProjectReference Include=\"..\\c\\d.csproj\" />"),
                "/project/src/x.csproj");
            var references = file.ParseProjectFile("test").NetCore.ProjectReferences;

            references.Should().HaveCount(2);

            var first = references.First();
            first.Name.Should().Be("b");
            first.RelativePath.Should().Be("../a/b.csproj");
            first.FilePath.ToString().Should().Be("/project/a/b.csproj");
            first.Package.Should().BeNull();
            first.Project.Should().BeNull();

            var second = references.Last();
            second.Name.Should().Be("d");
            second.RelativePath.Should().Be("../c/d.csproj");
            second.FilePath.ToString().Should().Be("/project/c/d.csproj");
            second.Package.Should().BeNull();
            second.Project.Should().BeNull();
        }

        [Fact]
        public void ParseProject_NetCore_PackageReference_ReturnsWithTargetFrameworkInParentIfSet()
        {
            var packageRef =
                @"<ItemGroup Condition=""'$(TargetFramework)'== 'net451'"">
                    <PackageReference Include=""System.Collections.Immutable"" Version=""1.3.1"" />
                </ItemGroup>
                <ItemGroup Condition=""'$(TargetFramework)'== 'netstandard1.5' "">
                    <PackageReference Include=""Newtonsoft.Json"" Version=""9.0.1"" />
                </ItemGroup>";
            var file = fs.CreateFakeFile(ProjectFileHelpers.GetNetCoreProjectWithString(packageRef));
            var references = file.ParseProjectFile("test").NetCore.PackageReferences;

            references.Should().HaveCount(2);

            var first = references.First();
            first.ExcludeAssets.Should().BeNull();
            first.IncludeAssets.Should().BeNull();
            first.PrivateAssets.Should().BeNull();
            first.Name.Should().Be("System.Collections.Immutable");
            first.TargetFramework.Should().Be("net451");
            first.Version.Should().Be("1.3.1");
        }

        [Fact]
        public void ParseProject_NetCore_PackageReference_ReturnsWithVersionAsChildElement()
        {
            var packageRef =
                @"<PackageReference Include=""Cake.Core"">
                    <Version>1.3.1</Version>
                    </PackageReference>";
            var file = fs.CreateFakeFile(ProjectFileHelpers.GetNetCoreProjectWithString(packageRef));
            var references = file.ParseProjectFile("test").NetCore.PackageReferences;

            references.Should().HaveCount(1);

            var first = references.First();
            first.Version.Should().Be("1.3.1");
        }

        [Fact]
        public void ParseProject_NetCore_PackageReference_ReturnsWithIncludeAsChildElement()
        {
            var packageRef =
                @"<PackageReference>
                    <Include>Cake.Core</Include>
                    </PackageReference>";
            var file = fs.CreateFakeFile(ProjectFileHelpers.GetNetCoreProjectWithString(packageRef));
            var references = file.ParseProjectFile("test").NetCore.PackageReferences;

            references.Should().HaveCount(1);

            var first = references.First();
            first.Name.Should().Be("Cake.Core");
        }

        [Fact]
        public void ParseProject_NetCore_PackageReference_ReturnsWithTargetFrameworkInAttributeIfSet()
        {
            var packageRef =
                @"<ItemGroup>
                    <PackageReference Include=""System.Collections.Immutable"" Version=""1.3.1"" Condition=""'$(TargetFramework)'== 'net451'"" />
                </ItemGroup>";
            var file = fs.CreateFakeFile(ProjectFileHelpers.GetNetCoreProjectWithString(packageRef));
            var references = file.ParseProjectFile("test").NetCore.PackageReferences;

            references.Should().HaveCount(1);

            var first = references.First();
            first.ExcludeAssets.Should().BeNull();
            first.IncludeAssets.Should().BeNull();
            first.PrivateAssets.Should().BeNull();
            first.Name.Should().Be("System.Collections.Immutable");
            first.TargetFramework.Should().Be("net451");
            first.Version.Should().Be("1.3.1");
        }

        [Fact]
        public void
            ParseProject_NetCore_PackageReference_ReturnsDuplicatesWithDifferentTargetFrameworkInAttributeIfSet()
        {
            var packageRef =
                @"<ItemGroup>
                    <PackageReference Include=""System.Collections.Immutable"" Version=""1.3.1"" Condition=""'$(TargetFramework)'== 'net451'"" />
                    <PackageReference Include=""System.Collections.Immutable"" Version=""1.3.2"" Condition=""'$(TargetFramework)'== 'netstandard1.5'"" />
                </ItemGroup>";
            var file = fs.CreateFakeFile(ProjectFileHelpers.GetNetCoreProjectWithString(packageRef));
            var references = file.ParseProjectFile("test").NetCore.PackageReferences;

            references.Should().HaveCount(2);

            var first = references.First();
            first.ExcludeAssets.Should().BeNull();
            first.IncludeAssets.Should().BeNull();
            first.PrivateAssets.Should().BeNull();
            first.Name.Should().Be("System.Collections.Immutable");
            first.TargetFramework.Should().Be("net451");
            first.Version.Should().Be("1.3.1");

            references.Last().TargetFramework.Should().Be("netstandard1.5");
            references.Last().Version.Should().Be("1.3.2");
        }

        [Fact]
        public void ParseProject_NetCore_DotNetCliToolReferences_ReturnsIfSet()
        {
            var cliRefs = @"<ItemGroup>
                          <DotNetCliToolReference Include=""Blerk1"" Version=""1.0.0"" />
                          <DotNetCliToolReference Include=""Blerk2"" Version=""2.3.4"" />
                            </ItemGroup>";
            var file = fs.CreateFakeFile(ProjectFileHelpers.GetNetCoreProjectWithString(cliRefs));
            var references = file.ParseProjectFile("test").NetCore.DotNetCliToolReferences;

            references.Should().HaveCount(2);

            var first = references.First();
            first.Name.Should().Be("Blerk1");
            first.Version.Should().Be("1.0.0");

            var last = references.Last();
            last.Name.Should().Be("Blerk2");
            last.Version.Should().Be("2.3.4");
        }

        [Fact]
        public void ParseProject_NetCore_Targets_ReturnsIfSet()
        {
            var targetString =
                @"<Target Name=""Jogging"" BeforeTargets=""Stretch;Jump;;"" AfterTargets=""IceBath"" DependsOn=""Mood"">
                                  <Exec Command=""hop.cmd"" />
                                  <Exec Command=""skip.cmd"" />
                                </Target>";
            var file = fs.CreateFakeFile(ProjectFileHelpers.GetNetCoreProjectWithString(targetString));
            var targets = file.ParseProjectFile("test").NetCore.Targets;

            var first = targets.Should().ContainSingle().Subject;
            first.BeforeTargets.Should().BeEquivalentTo("Stretch", "Jump");
            first.AfterTargets.Should().BeEquivalentTo("IceBath");
            first.DependsOn.Should().BeEquivalentTo("Mood");
            first.Name.Should().Be("Jogging");

            first.Executables.Should().HaveCount(2);

            first.Executables.First().Command.Should().Be("hop.cmd");
            first.Executables.Last().Command.Should().Be("skip.cmd");
        }


        [Fact]
        public void ParseProject_GetsCorrectOutputPath_FallbackConfigOnly()
        {
            var file = fs.CreateFakeFile(ProjectFileHelpers.GetNetCoreProjectWithString(null));
            file.ParseProjectFile("test").OutputPath.ToString().Should().Be("bin/test");
        }

        [Fact]
        public void ParseProject_GetsCorrectOutputPath_FallbackConfigAndPlatform()
        {
            var file = fs.CreateFakeFile(ProjectFileHelpers.GetNetCoreProjectWithString(null));
            file.ParseProjectFile("test", "x86").OutputPath.ToString().Should().Be("bin/x86/test");
        }

        [Fact]
        public void ParseProject_GetsCorrectOutputPath_FallbackConfigPlatformAndTargetFramework()
        {
            var file = fs.CreateFakeFile(ProjectFileHelpers.GetNetCoreProjectWithString(@"<PropertyGroup>
                            <TargetFramework>NetStandard1.6</TargetFramework>
                          </PropertyGroup>"));
            file.ParseProjectFile("test", "x86").OutputPath.ToString().Should().Be("bin/x86/test/NetStandard1.6");
        }

        [Fact]
        public void ParseProject_GetsCorrectOutputPath_UsesConditionOverrideWithDefaultPlatform()
        {
            var file = fs.CreateFakeFile(ProjectFileHelpers.GetNetCoreProjectWithString(
                @"<PropertyGroup Condition=""'$(Configuration)|$(Platform)'=='Test|AnyCPU'"">
                            <OutputPath>bin\wayhey\</OutputPath>
                          </PropertyGroup>"));
            file.ParseProjectFile("test").OutputPath.ToString().Should().Be("bin/wayhey");
        }

        [Fact]
        public void ParseProject_GetsCorrectOutputPath_UsesConditionOverride()
        {
            var file = fs.CreateFakeFile(ProjectFileHelpers.GetNetCoreProjectWithString(
                @"<PropertyGroup Condition=""'$(Configuration)|$(Platform)'=='Test|x86'"">
                            <OutputPath>bin\wayhey\</OutputPath>
                          </PropertyGroup>"));
            file.ParseProjectFile("test", "x86").OutputPath.ToString().Should().Be("bin/wayhey");
        }

        [Fact]
        public void HasPackage_ReturnsTrueWhenPackageFound()
        {
            var packageRef =
                @"<ItemGroup>
                    <PackageReference Include=""System.Collections.Immutable"" Version=""1.3.1"" Condition=""'$(TargetFramework)'== 'net451'"" />
                </ItemGroup>";
            var file = fs.CreateFakeFile(ProjectFileHelpers.GetNetCoreProjectWithString(packageRef));

            var project = file.ParseProjectFile("test");
            project.HasPackage("System.Collections.Immutable").Should().BeTrue();
            project.HasPackage("System.Collections.Immutable", "net451").Should().BeTrue();
            project.HasPackage("System.Collections.Immutable", "net452").Should().BeFalse();
        }


        [Fact]
        public void HasPackage_ReturnsFalseWhenPackageNotFound()
        {
            var file = fs.CreateFakeFile(ProjectFileHelpers.GetNetCoreProjectWithString(""));
            var project = file.ParseProjectFile("test");
            project.HasPackage("Moogli").Should().BeFalse();
            project.HasPackage("Moogli", "net452").Should().BeFalse();
        }

        [Fact]
        public void HasPackage_ReturnsTrueWhenPackageFoundAndParentCondition()
        {
            var packageRef =
                @"<ItemGroup Condition=""'$(TargetFramework)'== 'net451'"">
                    <PackageReference Include=""System.Collections.Immutable"" Version=""1.3.1"" />
                </ItemGroup>";
            var file = fs.CreateFakeFile(ProjectFileHelpers.GetNetCoreProjectWithString(packageRef));

            var project = file.ParseProjectFile("test");
            project.HasPackage("System.Collections.Immutable").Should().BeTrue();
            project.HasPackage("System.Collections.Immutable", "net451").Should().BeTrue();
            project.HasPackage("System.Collections.Immutable", "net452").Should().BeFalse();
        }

        [Fact]
        public void GetPackage_ReturnsTrueWhenPackageFoundAndParentCondition()
        {
            var packageRef =
                @"<ItemGroup Condition=""'$(TargetFramework)'== 'net451'"">
                    <PackageReference Include=""System.Collections.Immutable"" Version=""1.3.1"" />
                </ItemGroup>";
            var file = fs.CreateFakeFile(ProjectFileHelpers.GetNetCoreProjectWithString(packageRef));

            var project = file.ParseProjectFile("test");
            project.GetPackage("System.Collections.Immutable").Should().NotBeNull();
            project.GetPackage("System.Collections.Immutable", "net451").Should().NotBeNull();
            project.GetPackage("System.Collections.Immutable", "net452").Should().BeNull();
        }

        [Fact]
        public void HasReference_ReturnsTrueWhenPackageFound()
        {
            var reference = @"<ItemGroup><Reference Include=""Microsoft.CSharp"" /></ItemGroup>";
            var file = fs.CreateFakeFile(ProjectFileHelpers.GetNetCoreProjectWithString(reference));

            var project = file.ParseProjectFile("test");
            project.HasReference("Microsoft.CSharp").Should().BeTrue();
            project.HasReference("Blerk").Should().BeFalse();
        }

        [Fact]
        public void GetReference_ReturnsTrueWhenPackageFound()
        {
            var reference = @"<ItemGroup><Reference Include=""Microsoft.CSharp"" /></ItemGroup>";
            var file = fs.CreateFakeFile(ProjectFileHelpers.GetNetCoreProjectWithString(reference));

            var project = file.ParseProjectFile("test");
            project.GetReference("Microsoft.CSharp").Should().NotBeNull();
            project.GetReference("Blerk").Should().BeNull();
        }

        [Fact]
        public void HasDotNetCliToolReference_ReturnsTrueWhenPackageFound()
        {
            var reference =
                @"<ItemGroup><DotNetCliToolReference Include=""dotnet-xunit"" Version=""2.3.1"" /></ItemGroup>";
            var file = fs.CreateFakeFile(ProjectFileHelpers.GetNetCoreProjectWithString(reference));

            var project = file.ParseProjectFile("test");
            project.HasDotNetCliToolReference("dotnet-xunit").Should().BeTrue();
            project.HasDotNetCliToolReference("Blerk").Should().BeFalse();
        }

        [Fact]
        public void GetDotNetCliToolReference_ReturnsTrueWhenPackageFound()
        {
            var reference =
                @"<ItemGroup><DotNetCliToolReference Include=""dotnet-xunit"" Version=""2.3.1"" /></ItemGroup>";
            var file = fs.CreateFakeFile(ProjectFileHelpers.GetNetCoreProjectWithString(reference));

            var project = file.ParseProjectFile("test");
            project.GetDotNetCliToolReference("dotnet-xunit").Should().BeOfType<DotNetCliToolReference>().Which.Version
                .Should().Be("2.3.1");
            project.GetDotNetCliToolReference("Blerk").Should().BeNull();
        }

        [Fact]
        public void GetProjectFormat_ReturnsTrueWhenVS2017Format()
        {
            var file = fs.CreateFakeFile(ProjectFileHelpers.GetNetCoreProjectWithString(""));
            file.ParseProjectFile("test").IsVS2017ProjectFormat.Should().BeTrue();
        }

        [Fact]
        public void GetProjectFormat_ReturnsFalseWhenVS2017Format()
        {
            var file = fs.CreateFakeFile("CsProj_ValidFile".SafeLoad());
            file.ParseProjectFile("test").IsVS2017ProjectFormat.Should().BeFalse();
        }

        [Fact]
        public void ParseProjectTest()
        {
            var file = fs.CreateFakeFile(@"<Project Sdk=""Microsoft.NET.Sdk"">
  <PropertyGroup>
    <TargetFrameworks>netstandard1.6;net46</TargetFrameworks>
  </PropertyGroup>
  <PropertyGroup>
    <Version>0.0.0</Version>
    <Company>WCOMB</Company>
    <Authors>devlead; WCOMB</Authors>
    <CodeAnalysisRuleSet>..\Cake.Kudu.ruleset</CodeAnalysisRuleSet>
  </PropertyGroup>
  <PropertyGroup Condition=""'$(Configuration)|$(TargetFramework)|$(Platform)'=='Debug|netstandard1.6|AnyCPU'"">
    <DocumentationFile>bin\Debug\netstandard1.6\Cake.Kudu.xml</DocumentationFile>
  </PropertyGroup>
  <PropertyGroup Condition=""'$(Configuration)|$(TargetFramework)|$(Platform)'=='Release|netstandard1.6|AnyCPU'"">
    <DocumentationFile>bin\Release\netstandard1.6\Cake.Kudu.xml</DocumentationFile>
  </PropertyGroup>
  <PropertyGroup Condition=""'$(Configuration)|$(TargetFramework)|$(Platform)'=='Debug|net46|AnyCPU'"">
    <DocumentationFile>bin\Debug\net46\Cake.Kudu.xml</DocumentationFile>
  </PropertyGroup>
  <PropertyGroup Condition=""'$(Configuration)|$(TargetFramework)|$(Platform)'=='Release|net46|AnyCPU'"">
    <DocumentationFile>bin\Release\net46\Cake.Kudu.xml</DocumentationFile>
  </PropertyGroup>
  <ItemGroup>
    <PackageReference Include=""Cake.Common"" Version=""0.23.0"" />
    <PackageReference Include=""Cake.Core"" Version=""0.23.0"" />
    <PackageReference Include=""StyleCop.Analyzers"" Version=""1.0.2"" />
  </ItemGroup>
</Project>", "Cake.Kudu.csproj");

            var result = file.ParseProjectFile("Release");
            result.IsVS2017ProjectFormat.Should().BeTrue();
            result.IsNetCore.Should().BeFalse();
            result.IsNetStandard.Should().BeTrue();
            result.IsNetFramework.Should().BeTrue();
            result.OutputPath.FullPath.Should().Be("bin/Release/netstandard1.6");
            result.OutputPaths.Should().Contain(x => x.FullPath.EqualsIgnoreCase("bin/Release/netstandard1.6"));
            result.OutputPaths.Should().Contain(x => x.FullPath.EqualsIgnoreCase("bin/Release/net46"));
            result.NetCore.TargetFrameworks.Should().BeEquivalentTo(new []{ "netstandard1.6", "net46"});
            result.RootNameSpace.Should().Be("Cake.Kudu");
            result.IsLibrary().Should().BeTrue();
        }

        [Fact]
        public void ParseProject_ReturnsGlobalTool_WhenExeSpecified()
        {
            var file = fs.CreateFakeFile(ProjectFileHelpers.GetNetCoreProjectWithElement("PackAsTool", "true"));
            file.ParseProjectFile("test").IsGlobalTool().Should().BeTrue();
        }
    }
}