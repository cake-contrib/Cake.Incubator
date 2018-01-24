// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/. 
namespace Cake.Incubator.Tests
{
    using System.Linq;
    using Core.IO;
    using FluentAssertions;
    using Xunit;

    public class NetCoreProjectParserExtensionsTests
    {
        [Fact]
        public void ParseProject_GetsCorrectAssemblyName()
        {
            var netCoreProjectWithElement = ProjectFileHelpers.GetNetCoreProjectWithElement("AssemblyName", "a");
            var file = new FakeFile(netCoreProjectWithElement);
            file.ParseProject("test").AssemblyName.Should().Be("a");
        }

        [Fact]
        public void ParseProject_AssemblyName_DefaultsToProj()
        {
            var file = new FakeFile(ProjectFileHelpers.GetNetCoreProjectWithString(null), "testAssembly");
            file.ParseProject("test").AssemblyName.Should().Be("testAssembly");
        }

        [Fact]
        public void ParseProject_GetsCorrectConfiguration()
        {
            var file = new FakeFile(ProjectFileHelpers.GetNetCoreProjectWithString(null));

            file.ParseProject("test").Configuration.Should().Be("test");
        }

        [Theory]
        [InlineData("netcoreapp1.0")]
        [InlineData("netcoreapp1.1")]
        [InlineData("netcoreapp2.0")]
        [InlineData("netcoreappX.X")]
        public void ParseProject_SetsIsNetCore(string coreTarget)
        {
            var file = new FakeFile(ProjectFileHelpers.GetNetCoreProjectWithElement("TargetFramework", coreTarget));
            file.ParseProject("test").IsNetCore.Should().BeTrue();
        }

        [Theory]
        [InlineData("netstandard1.0")]
        [InlineData("netstandard1.1")]
        [InlineData("netstandard2.0")]
        [InlineData("netstandardX.X")]
        public void ParseProject_SetsIsNetStandard(string coreTarget)
        {
            var file = new FakeFile(ProjectFileHelpers.GetNetCoreProjectWithElement("TargetFramework", coreTarget));
            file.ParseProject("test").IsNetStandard.Should().BeTrue();
        }

        [Fact]
        public void ParseProject_GetsMultiTargets()
        {
            var file = new FakeFile(ProjectFileHelpers.GetNetCoreProjectWithElement("TargetFrameworks", "net45;net462;netstandard1.6;netcoreapp1.0;"));
            var result = file.ParseProject("test");

            result.IsNetStandard.Should().BeTrue();
            result.IsNetFramework.Should().BeTrue();
            result.IsNetCore.Should().BeTrue();
            result.TargetFrameworkVersions.Should().HaveCount(4).And.BeEquivalentTo("net45", "net462", "netstandard1.6", "netcoreapp1.0");
        }

        [Fact]
        public void ParseProject_IsCoreTestProject()
        {
            var testProject = "<PropertyGroup><TargetFramework>netcoreapp2.0</TargetFramework></PropertyGroup><ItemGroup><PackageReference Include=\"Microsoft.NET.Test.Sdk\" Version=\"15.5.0\" /></ItemGroup>";
            var file = new FakeFile(ProjectFileHelpers.GetNetCoreProjectWithString(testProject));
            file.ParseProject("test").IsTestProject().Should().BeTrue();
            file.ParseProject("test").IsDotNetCliTestProject().Should().BeTrue();
        }

        [Fact]
        public void ParseProject_IsCoreTestProjectForNetFxWithPackage()
        {
            var testProject = "<PropertyGroup><TargetFramework>net462</TargetFramework></PropertyGroup><ItemGroup><PackageReference Include=\"Microsoft.NET.Test.Sdk\" Version=\"15.5.0\" /></ItemGroup>";
            var file = new FakeFile(ProjectFileHelpers.GetNetCoreProjectWithString(testProject));
            file.ParseProject("test").IsTestProject().Should().BeTrue();
            file.ParseProject("test").IsDotNetCliTestProject().Should().BeTrue();
        }

        [Fact]
        public void ParseProject_IsTestProject_ReturnsFalseForNetStandard()
        {
            var testProject = "<PropertyGroup><TargetFramework>netstandard1.0</TargetFramework></PropertyGroup><ItemGroup><PackageReference Include=\"Microsoft.NET.Test.Sdk\" Version=\"15.5.0\" /></ItemGroup>";
            var file = new FakeFile(ProjectFileHelpers.GetNetCoreProjectWithString(testProject));
            file.ParseProject("test").IsTestProject().Should().BeFalse();
            file.ParseProject("test").IsDotNetCliTestProject().Should().BeFalse();
            file.ParseProject("test").IsFrameworkTestProject().Should().BeFalse();
        }

        [Fact]
        public void ParseProject_SetsIsNetCoreForWebSdk()
        {
            var file = new FakeFile(ProjectFileHelpers.GetNetCoreProjectWithString("<PropertyGroup><TargetFramework>netcoreapp2.0</TargetFramework></PropertyGroup>"));
            file.ParseProject("test").IsNetCore.Should().BeTrue();
        }

        [Fact]
        public void ParseProject_DoesNotSetIsNetFramework()
        {
            var file = new FakeFile(ProjectFileHelpers.GetNetCoreProjectWithElement("AssemblyName", "a"));
            file.ParseProject("test").IsNetFramework.Should().BeFalse();
        }

        [Fact]
        public void ParseProject_RootNamespace_ReturnsIfSet()
        {
            var file = new FakeFile(ProjectFileHelpers.GetNetCoreProjectWithElement("RootNamespace", "b"));
            file.ParseProject("test").RootNameSpace.Should().Be("b");
        }

        [Fact]
        public void ParseProject_RootNamespace_FallbackToProjectName()
        {
            var file = new FakeFile(ProjectFileHelpers.GetNetCoreProjectWithString(null), "./any/root.csproj");
            file.ParseProject("test").RootNameSpace.Should().Be("root");
        }

        [Fact]
        public void ParseProject_GetsCorrectOutputType_WhenNoneSpecified()
        {
            var file = new FakeFile(ProjectFileHelpers.GetNetCoreProjectWithElement("AssemblyName", "a"));
            file.ParseProject("test").OutputType.Should().Be("Library");
        }

        [Fact]
        public void ParseProject_GetsCorrectOutputPath_WhenTargetFrameworkSpecified()
        {
            var file = new FakeFile(ProjectFileHelpers.GetNetCoreProjectWithElement("TargetFramework", "net45"));
            file.ParseProject("test").OutputPath.FullPath.Should().Be("bin/test/net45");
        }

        [Fact]
        public void ParseProject_GetsCorrectDefaultOutputPath_WhenTargetFrameworksSpecified()
        {
            var file = new FakeFile(ProjectFileHelpers.GetNetCoreProjectWithElement("TargetFrameworks", "net45;netstandard1.6"));
            file.ParseProject("test").OutputPath.FullPath.Should().Be("bin/test/net45");
        }

        [Fact]
        public void ParseProject_GetsCorrectDefaultOutputPaths_WhenTargetFrameworksSpecified()
        {
            var file = new FakeFile(ProjectFileHelpers.GetNetCoreProjectWithElement("TargetFrameworks", "net45;netstandard1.6"));
            var paths = file.ParseProject("test").OutputPaths;
            paths.Should().HaveCount(2);
            paths.First().FullPath.Should().Be("bin/test/net45");
            paths.Last().FullPath.Should().Be("bin/test/netstandard1.6");
        }

        [Fact]
        public void ParseProject_GetsCorrectDefaultOutputPaths_WhenTargetFrameworkSpecified()
        {
            var file = new FakeFile(ProjectFileHelpers.GetNetCoreProjectWithElement("TargetFramework", "net45"));
            file.ParseProject("test").OutputPaths.Should().ContainSingle().Which.FullPath.Should().Be("bin/test/net45");
        }


        [Fact]
        public void ParseProject_GetsCorrectOutputType_WhenLibrarySpecified()
        {
            var file = new FakeFile(ProjectFileHelpers.GetNetCoreProjectWithElement("OutputType", "Library"));
            file.ParseProject("test").OutputType.Should().Be("Library");
        }

        [Fact]
        public void ParseProject_GetsCorrectOutputType_WhenExeSpecified()
        {
            var file = new FakeFile(ProjectFileHelpers.GetNetCoreProjectWithElement("OutputType", "Exe"));
            file.ParseProject("test").OutputType.Should().Be("Exe");
        }

        [Fact]
        public void ParseProject_ApplicationIcons_ReturnsIfSet()
        {
            var file = new FakeFile(ProjectFileHelpers.GetNetCoreProjectWithElement("ApplicationIcon", "fav.ico"));
            file.ParseProject("test").NetCore.ApplicationIcon.Should().Be("fav.ico");
        }

        [Fact]
        public void ParseProject_GeneratePackageOnBuild_ReturnsIfSet()
        {
            var file = new FakeFile(ProjectFileHelpers.GetNetCoreProjectWithElement("GeneratePackageOnBuild", "true"));
            file.ParseProject("test").NetCore.GeneratePackageOnBuild.Should().BeTrue();
        }

        [Fact]
        public void ParseProject_GetsCorrectDefaultPlatform()
        {
            var file = new FakeFile(ProjectFileHelpers.GetNetCoreProjectWithString(null));
            file.ParseProject("test").Platform.Should().Be("AnyCPU");
        }

        [Fact]
        public void ParseProject_GetsSpecifiedPlatformElement()
        {
            var file = new FakeFile(ProjectFileHelpers.GetNetCoreProjectWithString(null));
            file.ParseProject("test", "x86").Platform.Should().Be("x86");
        }

        [Fact]
        public void ParseProject_IgnoresProjectGuid()
        {
            var file = new FakeFile(ProjectFileHelpers.GetNetCoreProjectWithString(null));
            file.ParseProject("test").ProjectGuid.Should().BeNull();
        }

        [Fact]
        public void ParseProject_IgnoresProjectTypeGuids()
        {
            var file = new FakeFile(ProjectFileHelpers.GetNetCoreProjectWithString(null));
            file.ParseProject("test").ProjectTypeGuids.Should().BeNull();
        }

        [Fact]
        public void ParseProject_IgnoresTargetFrameworkProfile()
        {
            var file = new FakeFile(ProjectFileHelpers.GetNetCoreProjectWithString(null));
            file.ParseProject("test").TargetFrameworkProfile.Should().BeNull();
        }

        [Fact]
        public void ParseProject_TargetFrameworkVersion_NullIfUnspecified()
        {
            var file = new FakeFile(ProjectFileHelpers.GetNetCoreProjectWithString(null));
            file.ParseProject("test").TargetFrameworkVersion.Should().BeNull();
        }

        [Fact]
        public void ParseProject_TargetFrameworkVersion_ReturnsExpected()
        {
            var file = new FakeFile(ProjectFileHelpers.GetNetCoreProjectWithElement("TargetFramework", "Netstandard1.1"));
            file.ParseProject("test").TargetFrameworkVersion.Should().Be("Netstandard1.1");
        }

        [Fact]
        public void ParseProject_NetCore_IsNotNull()
        {
            var file = new FakeFile(ProjectFileHelpers.GetNetCoreProjectWithString(null));
            file.ParseProject("test").NetCore.Should().NotBeNull();
        }

        [Fact]
        public void ParseProject_NetCore_AllowUnsafe_DefaultsFalse()
        {
            var file = new FakeFile(ProjectFileHelpers.GetNetCoreProjectWithString(null));
            file.ParseProject("test").NetCore.AllowUnsafeBlocks.Should().BeFalse();
        }

        [Fact]
        public void ParseProject_NetCore_AllowUnsafe_TrueIfSet()
        {
            var file = new FakeFile(ProjectFileHelpers.GetNetCoreProjectWithElement("AllowUnsafeBlocks", "true"));
            file.ParseProject("test").NetCore.AllowUnsafeBlocks.Should().BeTrue();
        }

        [Fact]
        public void ParseProject_NetCore_DebugSymbols_TrueIfSet()
        {
            var file = new FakeFile(ProjectFileHelpers.GetNetCoreProjectWithElement("DebugSymbols", "true"));
            file.ParseProject("test").NetCore.DebugSymbols.Should().BeTrue();
        }

        [Fact]
        public void ParseProject_NetCore_AssemblyOriginatorKeyFile_ReturnsIfSet()
        {
            var file = new FakeFile(ProjectFileHelpers.GetNetCoreProjectWithElement("AssemblyOriginatorKeyFile", "key.snk"));
            file.ParseProject("test").NetCore.AssemblyOriginatorKeyFile.Should().Be("key.snk");
        }

        [Fact]
        public void ParseProject_NetCore_BuildOutputTargetFolder_ReturnsIfSet()
        {
            var file = new FakeFile(ProjectFileHelpers.GetNetCoreProjectWithElement("BuildOutputTargetFolder", "./oompa/loompa"));
            file.ParseProject("test").NetCore.BuildOutputTargetFolder.Should().Be("./oompa/loompa");
        }

        [Fact]
        public void ParseProject_NetCore_IncludeBuildOutput_ReturnsIfSet()
        {
            var file = new FakeFile(ProjectFileHelpers.GetNetCoreProjectWithElement("IncludeBuildOutput", "true"));
            file.ParseProject("test").NetCore.IncludeBuildOutput.Should().BeTrue();
        }

        [Fact]
        public void ParseProject_NetCore_IncludeContentInPack_ReturnsIfSet()
        {
            var file = new FakeFile(ProjectFileHelpers.GetNetCoreProjectWithElement("IncludeContentInPack", "true"));
            file.ParseProject("test").NetCore.IncludeContentInPack.Should().BeTrue();
        }

        [Fact]
        public void ParseProject_NetCore_IncludeSource_ReturnsIfSet()
        {
            var file = new FakeFile(ProjectFileHelpers.GetNetCoreProjectWithElement("IncludeSource", "true"));
            file.ParseProject("test").NetCore.IncludeSource.Should().BeTrue();
        }

        [Fact]
        public void ParseProject_NetCore_IncludeSymbols_ReturnsIfSet()
        {
            var file = new FakeFile(ProjectFileHelpers.GetNetCoreProjectWithElement("IncludeSymbols", "true"));
            file.ParseProject("test").NetCore.IncludeSymbols.Should().BeTrue();
        }

        [Fact]
        public void ParseProject_NetCore_IsPackable_ReturnsIfSet()
        {
            var file = new FakeFile(ProjectFileHelpers.GetNetCoreProjectWithElement("IsPackable", "true"));
            file.ParseProject("test").NetCore.IsPackable.Should().BeTrue();
        }

        [Fact]
        public void ParseProject_NetCore_IsPackable_DefaultFallback()
        {
            var file = new FakeFile(ProjectFileHelpers.GetNetCoreProjectWithString(null));
            file.ParseProject("test").NetCore.IsPackable.Should().BeTrue();
        }

        [Fact]
        public void ParseProject_NetCore_IsTool_ReturnsIfSet()
        {
            var file = new FakeFile(ProjectFileHelpers.GetNetCoreProjectWithElement("IsTool", "true"));
            file.ParseProject("test").NetCore.IsTool.Should().BeTrue();
        }

        [Fact]
        public void ParseProject_NetCore_NoPackageAnalysis_ReturnsIfSet()
        {
            var file = new FakeFile(ProjectFileHelpers.GetNetCoreProjectWithElement("NoPackageAnalysis", "true"));
            file.ParseProject("test").NetCore.NoPackageAnalysis.Should().BeTrue();
        }

        [Fact]
        public void ParseProject_NetCore_MinClientVersion_ReturnsIfSet()
        {
            var file = new FakeFile(ProjectFileHelpers.GetNetCoreProjectWithElement("MinClientVersion", "mouse"));
            file.ParseProject("test").NetCore.MinClientVersion.Should().Be("mouse");
        }

        [Fact]
        public void ParseProject_NetCore_NuspecBasePath_ReturnsIfSet()
        {
            var file = new FakeFile(ProjectFileHelpers.GetNetCoreProjectWithElement("NuspecBasePath", "./all/about/the/base"));
            file.ParseProject("test").NetCore.NuspecBasePath.Should().Be("./all/about/the/base");
        }

        [Fact]
        public void ParseProject_NetCore_NuspecFile_ReturnsIfSet()
        {
            var file = new FakeFile(ProjectFileHelpers.GetNetCoreProjectWithElement("NuspecFile", "./re.spec"));
            file.ParseProject("test").NetCore.NuspecFile.Should().Be("./re.spec");
        }

        [Fact]
        public void ParseProject_NetCore_NuspecProperties_ReturnsIfSet()
        {
            var file = new FakeFile(ProjectFileHelpers.GetNetCoreProjectWithElement("NuspecProperties", "edgar=poe;jimmini=cricket;"));
            var props = file.ParseProject("test").NetCore.NuspecProperties;
            props.Should().HaveCount(2);
            props["edgar"].Should().Be("poe");
            props["jimmini"].Should().Be("cricket");
        }

        [Fact]
        public void ParseProject_NetCore_PackageOutputPath_ReturnsIfSet()
        {
            var file = new FakeFile(ProjectFileHelpers.GetNetCoreProjectWithElement("PackageOutputPath", "./row/row/row/yerboat"));
            file.ParseProject("test").NetCore.PackageOutputPath.Should().Be("./row/row/row/yerboat");
        }

        [Fact]
        public void ParseProject_NetCore_Title_ReturnsIfSet()
        {
            var file = new FakeFile(ProjectFileHelpers.GetNetCoreProjectWithElement("Title", "Count Dracula"));
            file.ParseProject("test").NetCore.Title.Should().Be("Count Dracula");
        }

        [Fact]
        public void ParseProject_NetCore_ContentTargetFolders_ReturnsIfSet()
        {
            var file = new FakeFile(ProjectFileHelpers.GetNetCoreProjectWithElement("ContentTargetFolders", "content;contentFiles;;"));
            file.ParseProject("test").NetCore.ContentTargetFolders.Should().BeEquivalentTo("content", "contentFiles");
        }

        [Fact]
        public void ParseProject_NetCore_AssemblyTitle_ReturnsIfSet()
        {
            var file = new FakeFile(ProjectFileHelpers.GetNetCoreProjectWithElement("AssemblyTitle", "my title"));
            file.ParseProject("test").NetCore.AssemblyTitle.Should().Be("my title");
        }

        [Fact]
        public void ParseProject_NetCore_AssemblyTitle_OverridesAssemblyName()
        {
            var file = new FakeFile(ProjectFileHelpers.GetNetCoreProjectWithString("<AssemblyName>original</AssemblyName><AssemblyTitle>my title</AssemblyTitle>"));
            file.ParseProject("test").NetCore.AssemblyTitle.Should().Be("my title");
        }

        [Fact]
        public void ParseProject_NetCore_AssemblyTitle_AssemblyNameFallback()
        {
            var file = new FakeFile(ProjectFileHelpers.GetNetCoreProjectWithString("<AssemblyName>original</AssemblyName>"));
            file.ParseProject("test").NetCore.AssemblyTitle.Should().Be("original");
        }

        [Fact]
        public void ParseProject_NetCore_AssemblyTitle_ProjNameFallback()
        {
            var file = new FakeFile(ProjectFileHelpers.GetNetCoreProjectWithString(null), "./projName.csproj");
            file.ParseProject("test").NetCore.AssemblyTitle.Should().Be("projName");
        }

        [Fact]
        public void ParseProject_NetCore_Authors_ReturnsIfSet()
        {
            var file = new FakeFile(ProjectFileHelpers.GetNetCoreProjectWithElement("Authors", "one;two;"));
            file.ParseProject("test").NetCore.Authors.Should().BeEquivalentTo("one", "two");
        }

        [Fact]
        public void ParseProject_NetCore_Company_ReturnsIfSet()
        {
            var file = new FakeFile(ProjectFileHelpers.GetNetCoreProjectWithElement("Company", "Acme inc"));
            file.ParseProject("test").NetCore.Company.Should().Be("Acme inc");
        }

        [Fact]
        public void ParseProject_NetCore_Copyright_ReturnsIfSet()
        {
            var file = new FakeFile(ProjectFileHelpers.GetNetCoreProjectWithElement("Copyright", "moi!"));
            file.ParseProject("test").NetCore.Copyright.Should().Be("moi!");
        }

        [Fact]
        public void ParseProject_NetCore_DebugType_ReturnsIfSet()
        {
            var file = new FakeFile(ProjectFileHelpers.GetNetCoreProjectWithElement("DebugType", "Embedded"));
            file.ParseProject("test").NetCore.DebugType.Should().Be("Embedded");
        }

        [Fact]
        public void ParseProject_NetCore_DefineConstants_ReturnsIfSet()
        {
            var file = new FakeFile(ProjectFileHelpers.GetNetCoreProjectWithElement("DefineConstants", "DEBUG;WIN;"));
            file.ParseProject("test").NetCore.DefineConstants.Should().BeEquivalentTo("DEBUG", "WIN");
        }

        [Theory]
        [InlineData("test", "oil")]
        [InlineData("bananas", "shoes")]
        public void ParseProject_NetCore_DefineConstants_ReturnsEmptyIfNoConfigMatch(string config, string platform)
        {
            var file = new FakeFile(ProjectFileHelpers.GetNetCoreProjectElementWithConfig("DefineConstants", "Birth;Death;Taxes;", config, platform));
            file.ParseProject("test", "shoes").NetCore.DefineConstants.Should().BeEmpty();
        }

        [Fact]
        public void ParseProject_NetCore_DefineConstants_ReturnsCorrectConfigMatch()
        {
            var file = new FakeFile(ProjectFileHelpers.GetNetCoreProjectElementWithConfig("DefineConstants", "Birth;Death;Taxes;", "test", "shoes"));
            file.ParseProject("test", "shoes").NetCore.DefineConstants.Should().BeEquivalentTo("Birth", "Death", "Taxes");
        }

        [Fact]
        public void ParseProject_NetCore_DefineConstants_ReturnsCorrectParentConfigMatch()
        {
            var file = new FakeFile(ProjectFileHelpers.GetNetCoreProjectElementWithParentConfig("DefineConstants", "Birth;Death;Taxes;", "test", "shoes"));
            file.ParseProject("test", "shoes").NetCore.DefineConstants.Should().BeEquivalentTo("Birth", "Death", "Taxes");
        }

        [Fact]
        public void ParseProject_NetCore_DefineConstants_ReturnsCorrectFallbackIfConditionsDontMatch()
        {
            var projString = @"<Project sdk=""Microsoft.NET.Sdk""><DefineConstants>Hoka;Loca;Moca;</DefineConstants><PropertyGroup Condition=""'$(Configuration)|$(Platform)'=='test|shoes'""><DefineConstants>Birth;Death;Taxes;</DefineConstants></PropertyGroup></Project>";
            var file = new FakeFile(ProjectFileHelpers.GetNetCoreProjectWithString(projString));
            file.ParseProject("Roca", "shoes").NetCore.DefineConstants.Should().BeEquivalentTo("Hoka", "Loca", "Moca");
        }

        [Fact]
        public void ParseProject_NetCore_Description_ReturnsIfSet()
        {
            var file = new FakeFile(ProjectFileHelpers.GetNetCoreProjectWithElement("Description", "Howdy"));
            file.ParseProject("test").NetCore.Description.Should().Be("Howdy");
        }

        [Fact]
        public void ParseProject_NetCore_WarningLevel_ReturnsIfSet()
        {
            var file = new FakeFile(ProjectFileHelpers.GetNetCoreProjectWithElement("WarningLevel", "3"));
            file.ParseProject("test").NetCore.WarningLevel.Should().Be("3");
        }

        [Fact]
        public void ParseProject_NetCore_Optimize_ReturnsIfSet()
        {
            var file = new FakeFile(ProjectFileHelpers.GetNetCoreProjectWithElement("Optimize", "true"));
            file.ParseProject("test").NetCore.Optimize.Should().BeTrue();
        }

        [Fact]
        public void ParseProject_NetCore_DocumentationFile_ReturnsIfSet()
        {
            var file = new FakeFile(ProjectFileHelpers.GetNetCoreProjectWithElement("DocumentationFile", ".a/b/c.xml"));
            file.ParseProject("test").NetCore.DocumentationFile.Should().Be(".a/b/c.xml");
        }

        [Fact]
        public void ParseProject_NetCore_GenerateDocumentationFile_ReturnsIfSet()
        {
            var file = new FakeFile(ProjectFileHelpers.GetNetCoreProjectWithElement("GenerateDocumentationFile", "true"));
            file.ParseProject("test").NetCore.GenerateDocumentationFile.Should().BeTrue();
        }

        [Fact]
        public void ParseProject_NetCore_NetStandardImplicitPackageVersion_ReturnsIfSet()
        {
            var file = new FakeFile(ProjectFileHelpers.GetNetCoreProjectWithElement("NetStandardImplicitPackageVersion", "1.1.0-beta5-alpha3-preview6-final3"));
            file.ParseProject("test").NetCore.NetStandardImplicitPackageVersion.Should().Be("1.1.0-beta5-alpha3-preview6-final3");
        }

        [Fact]
        public void ArseProjectpay_EtCorenay_EutralLanguagenay_EturnsIfSetray()
        {
            var file = new FakeFile(ProjectFileHelpers.GetNetCoreProjectWithElement("NeutralLanguage", "PigLatin"));
            file.ParseProject("test").NetCore.NeutralLanguage.Should().Be("PigLatin");
        }

        [Fact]
        public void ParseProject_NetCore_NoWarn_ReturnsIfSet()
        {
            var file = new FakeFile(ProjectFileHelpers.GetNetCoreProjectWithElement("NoWarn", "$(nowarn);CS3132;CS2534;"));
            file.ParseProject("test").NetCore.NoWarn.Should().BeEquivalentTo("CS3132", "CS2534");
        }

        [Fact]
        public void ParseProject_NetCore_TreatSpecificWarningsAsErrors_ReturnsIfSet()
        {
            var file = new FakeFile(ProjectFileHelpers.GetNetCoreProjectWithElement("TreatSpecificWarningsAsErrors", ";CS3132;CS2534;"));
            file.ParseProject("test").NetCore.TreatSpecificWarningsAsErrors.Should().BeEquivalentTo("CS3132", "CS2534");
        }

        [Fact]
        public void ParseProject_NetCore_GenerateSerializationAssemblies_ReturnsIfSet()
        {
            var file = new FakeFile(ProjectFileHelpers.GetNetCoreProjectWithElement("GenerateSerializationAssemblies", "autobots"));
            file.ParseProject("test").NetCore.GenerateSerializationAssemblies.Should().Be("autobots");
        }

        [Fact]
        public void ParseProject_NetCore_LangVersion_ReturnsIfSet()
        {
            var file = new FakeFile(ProjectFileHelpers.GetNetCoreProjectWithElement("LangVersion", "GROOT"));
            file.ParseProject("test").NetCore.LangVersion.Should().Be("GROOT");
        }

        [Fact]
        public void ParseProject_NetCore_PackageIconUrl_ReturnsIfSet()
        {
            var file = new FakeFile(ProjectFileHelpers.GetNetCoreProjectWithElement("PackageIconUrl", "http://acme.inc/fav.ico"));
            file.ParseProject("test").NetCore.PackageIconUrl.Should().Be("http://acme.inc/fav.ico");
        }

        [Fact]
        public void ParseProject_NetCore_Product_ReturnsIfSet()
        {
            var file = new FakeFile(ProjectFileHelpers.GetNetCoreProjectWithElement("Product", "acme skyhooks"));
            file.ParseProject("test").NetCore.Product.Should().Be("acme skyhooks");
        }

        [Fact]
        public void ParseProject_NetCore_PackageId_ReturnsIfSet()
        {
            var file = new FakeFile(ProjectFileHelpers.GetNetCoreProjectWithElement("PackageId", "AcmeInc"));
            file.ParseProject("test").NetCore.PackageId.Should().Be("AcmeInc");
        }

        [Fact]
        public void ParseProject_NetCore_PackageId_AssemblyNameFallback()
        {
            var file = new FakeFile(ProjectFileHelpers.GetNetCoreProjectWithElement("AssemblyName", "Woot"));
            file.ParseProject("test").NetCore.PackageId.Should().Be("Woot");
        }

        [Fact]
        public void ParseProject_NetCore_PackageId_ProjectNameFallback()
        {
            var file = new FakeFile(ProjectFileHelpers.GetNetCoreProjectWithString(null), "./Twit/Twoot.fsproj");
            file.ParseProject("test").NetCore.PackageId.Should().Be("Twoot");
        }

        [Fact]
        public void ParseProject_NetCore_PackageLicenseUrl_ReturnsIfSet()
        {
            var file = new FakeFile(ProjectFileHelpers.GetNetCoreProjectWithElement("PackageLicenseUrl", "http://death.star/legal"));
            file.ParseProject("test").NetCore.PackageLicenseUrl.Should().Be("http://death.star/legal");
        }

        [Fact]
        public void ParseProject_NetCore_PackageProjectUrl_ReturnsIfSet()
        {
            var file = new FakeFile(ProjectFileHelpers.GetNetCoreProjectWithElement("PackageProjectUrl", "https://github.com/cake-contrib/Cake.Incubator"));
            file.ParseProject("test").NetCore.PackageProjectUrl.Should().Be("https://github.com/cake-contrib/Cake.Incubator");
        }

        [Fact]
        public void ParseProject_NetCore_PackageReleaseNotes_ReturnsIfSet()
        {
            var file = new FakeFile(ProjectFileHelpers.GetNetCoreProjectWithElement("PackageReleaseNotes", "Wery well, we shall weelesse wodger"));
            file.ParseProject("test").NetCore.PackageReleaseNotes.Should().Be("Wery well, we shall weelesse wodger");
        }

        [Fact]
        public void ParseProject_NetCore_PackageRequireLicenseAcceptance_ReturnsIfSet()
        {
            var file = new FakeFile(ProjectFileHelpers.GetNetCoreProjectWithElement("PackageRequireLicenseAcceptance", "true"));
            file.ParseProject("test").NetCore.PackageRequireLicenseAcceptance.Should().BeTrue();
        }

        [Fact]
        public void ParseProject_NetCore_PackageTags_ReturnsIfSet()
        {
            var file = new FakeFile(ProjectFileHelpers.GetNetCoreProjectWithElement("PackageTags", "Eenie;Meenie;Moe;"));
            file.ParseProject("test").NetCore.PackageTags.Should().BeEquivalentTo("Eenie", "Meenie", "Moe");
        }

        [Fact]
        public void ParseProject_NetCore_PackageTargetFallbacks_ReturnsIfSet()
        {
            var file = new FakeFile(ProjectFileHelpers.GetNetCoreProjectWithElement("PackageTargetFallback", "net45;net46;netunstandard-10.gamma.cheese;"));
            file.ParseProject("test").NetCore.PackageTargetFallbacks.Should().BeEquivalentTo("net45", "net46", "netunstandard-10.gamma.cheese");
        }

        [Fact]
        public void ParseProject_NetCore_PreserveCompilationContext_ReturnsIfSet()
        {
            var file = new FakeFile(ProjectFileHelpers.GetNetCoreProjectWithElement("PreserveCompilationContext", "true"));
            file.ParseProject("test").NetCore.PreserveCompilationContext.Should().BeTrue();
        }

        [Fact]
        public void ParseProject_NetCore_PublicSign_ReturnsIfSet()
        {
            var file = new FakeFile(ProjectFileHelpers.GetNetCoreProjectWithElement("PublicSign", "true"));
            file.ParseProject("test").NetCore.PublicSign.Should().BeTrue();
        }

        [Fact]
        public void ParseProject_NetCore_DelaySign_ReturnsIfSet()
        {
            var file = new FakeFile(ProjectFileHelpers.GetNetCoreProjectWithElement("DelaySign", "true"));
            file.ParseProject("test").NetCore.DelaySign.Should().BeTrue();
        }

        [Fact]
        public void ParseProject_NetCore_RepositoryType_ReturnsIfSet()
        {
            var file = new FakeFile(ProjectFileHelpers.GetNetCoreProjectWithElement("RepositoryType", "gitgat"));
            file.ParseProject("test").NetCore.RepositoryType.Should().Be("gitgat");
        }

        [Fact]
        public void ParseProject_NetCore_RepositoryType_DefaultFallback()
        {
            var file = new FakeFile(ProjectFileHelpers.GetNetCoreProjectWithString(null));
            file.ParseProject("test").NetCore.RepositoryType.Should().Be("git");
        }

        [Fact]
        public void ParseProject_NetCore_RepositoryUrl_ReturnsIfSet()
        {
            var file = new FakeFile(ProjectFileHelpers.GetNetCoreProjectWithElement("RepositoryUrl", "git://gat"));
            file.ParseProject("test").NetCore.RepositoryUrl.Should().Be("git://gat");
        }

        [Fact]
        public void ParseProject_NetCore_RuntimeFrameworkVersion_ReturnsIfSet()
        {
            var file = new FakeFile(ProjectFileHelpers.GetNetCoreProjectWithElement("RuntimeFrameworkVersion", "minustwentzillion.epsilonEcho.woof"));
            file.ParseProject("test").NetCore.RuntimeFrameworkVersion.Should().Be("minustwentzillion.epsilonEcho.woof");
        }

        [Fact]
        public void ParseProject_NetCore_RuntimeIdentifiers_ReturnsIfSet()
        {
            var file = new FakeFile(ProjectFileHelpers.GetNetCoreProjectWithElement("RuntimeIdentifiers", "charlie;delta;echo;;"));
            file.ParseProject("test").NetCore.RuntimeIdentifiers.Should().BeEquivalentTo("charlie", "delta", "echo");
        }

        [Fact]
        public void ParseProject_NetCore_Sdk_ReturnsIfSet()
        {
            var file = new FakeFile(ProjectFileHelpers.GetNetCoreProjectWithString(null));
            file.ParseProject("test").NetCore.Sdk.Should().BeEquivalentTo("Microsoft.Net.Sdk");
        }

        [Fact]
        public void ParseProject_NetCore_WebSdk_ReturnsIfSet()
        {
            var file = new FakeFile(ProjectFileHelpers.GetNetCoreWebProjectWithString(null));
            file.ParseProject("test").NetCore.Sdk.Should().BeEquivalentTo("Microsoft.Net.Sdk.Web");
        }

        [Fact]
        public void ParseProject_NetCore_SignAssembly_ReturnsIfSet()
        {
            var file = new FakeFile(ProjectFileHelpers.GetNetCoreProjectWithElement("SignAssembly", "TRUE"));
            file.ParseProject("test").NetCore.SignAssembly.Should().BeTrue();
        }

        [Fact]
        public void ParseProject_NetCore_TargetFrameworks_ReturnsIfSet()
        {
            var file = new FakeFile(ProjectFileHelpers.GetNetCoreProjectWithElement("TargetFrameworks", "net45;net46;12345;;"));
            file.ParseProject("test").NetCore.TargetFrameworks.Should().BeEquivalentTo("net45", "net46", "12345");
        }

        [Fact]
        public void ParseProject_NetCore_TargetFrameworks_TargetFrameworkFallback()
        {
            var file = new FakeFile(ProjectFileHelpers.GetNetCoreProjectWithElement("TargetFramework", "net45"));
            file.ParseProject("test").NetCore.TargetFrameworks.Should().BeEquivalentTo("net45");
        }

        [Fact]
        public void ParseProject_NetCore_TreatWarningsAsErrors_ReturnsIfSet()
        {
            var file = new FakeFile(ProjectFileHelpers.GetNetCoreProjectWithElement("TreatWarningsAsErrors", "true"));
            file.ParseProject("test").NetCore.TreatWarningsAsErrors.Should().BeTrue();
        }

        [Fact]
        public void ParseProject_NetCore_Version_ReturnsIfSet()
        {
            var file = new FakeFile(ProjectFileHelpers.GetNetCoreProjectWithString("<VersionPrefix>1.2.3</VersionPrefix><VersionSuffix>5813</VersionSuffix><Version>8.9.10</Version>"));
            file.ParseProject("test").NetCore.Version.Should().Be("1.2.3.5813");
        }

        [Fact]
        public void ParseProject_NetCore_Version_ReturnsIfSemver()
        {
            var file = new FakeFile(ProjectFileHelpers.GetNetCoreProjectWithString("<VersionPrefix>1.2.3</VersionPrefix><VersionSuffix>alpha5813</VersionSuffix><Version>8.9.10</Version>"));
            file.ParseProject("test").NetCore.Version.Should().Be("1.2.3-alpha5813");
        }

        [Fact]
        public void ParseProject_NetCore_Version_VersionFallback()
        {
            var file = new FakeFile(ProjectFileHelpers.GetNetCoreProjectWithElement("Version", "1.2.3.5"));
            file.ParseProject("test").NetCore.Version.Should().Be("1.2.3.5");
        }

        [Fact]
        public void ParseProject_NetCore_AssemblyVersion_ReturnsIfSet()
        {
            var file = new FakeFile(ProjectFileHelpers.GetNetCoreProjectWithElement("AssemblyVersion", "1.4.3.5"));
            file.ParseProject("test").NetCore.AssemblyVersion.Should().Be("1.4.3.5");
        }

        [Fact]
        public void ParseProject_NetCore_AssemblyVersion_DefaultFallback()
        {
            var file = new FakeFile(ProjectFileHelpers.GetNetCoreProjectWithString(null));
            file.ParseProject("test").NetCore.AssemblyVersion.Should().Be("1.0.0.0");
        }

        [Fact]
        public void ParseProject_NetCore_FileVersion_ReturnsIfSet()
        {
            var file = new FakeFile(ProjectFileHelpers.GetNetCoreProjectWithElement("FileVersion", "1.4.3.5"));
            file.ParseProject("test").NetCore.FileVersion.Should().Be("1.4.3.5");
        }

        [Fact]
        public void ParseProject_NetCore_FileVersion_DefaultFallback()
        {
            var file = new FakeFile(ProjectFileHelpers.GetNetCoreProjectWithString(null));
            file.ParseProject("test").NetCore.FileVersion.Should().Be("1.0.0.0");
        }

        [Fact]
        public void ParseProject_NetCore_Version_DefaultFallback()
        {
            var file = new FakeFile(ProjectFileHelpers.GetNetCoreProjectWithString(null));
            file.ParseProject("test").NetCore.Version.Should().Be("1.0.0");
        }

        [Fact]
        public void ParseProject_NetCore_ConcurrentGarbageCollection_ReturnsIfSet()
        {
            var file = new FakeFile(ProjectFileHelpers.GetNetCoreProjectWithElement("ConcurrentGarbageCollection", "true"));
            file.ParseProject("test").NetCore.RuntimeOptions.ConcurrentGarbageCollection.Should().BeTrue();
        }

        [Fact]
        public void ParseProject_NetCore_RetainVMGarbageCollection_ReturnsIfSet()
        {
            var file = new FakeFile(ProjectFileHelpers.GetNetCoreProjectWithElement("RetainVMGarbageCollection", "true"));
            file.ParseProject("test").NetCore.RuntimeOptions.RetainVMGarbageCollection.Should().BeTrue();
        }

        [Fact]
        public void ParseProject_NetCore_ServerGarbageCollection_ReturnsIfSet()
        {
            var file = new FakeFile(ProjectFileHelpers.GetNetCoreProjectWithElement("ServerGarbageCollection", "true"));
            file.ParseProject("test").NetCore.RuntimeOptions.ServerGarbageCollection.Should().BeTrue();
        }

        [Fact]
        public void ParseProject_NetCore_ThreadPoolMaxThreads_ReturnsIfSet()
        {
            var file = new FakeFile(ProjectFileHelpers.GetNetCoreProjectWithElement("ThreadPoolMaxThreads", "3"));
            file.ParseProject("test").NetCore.RuntimeOptions.ThreadPoolMaxThreads.Should().Be(3);
        }

        [Fact]
        public void ParseProject_NetCore_ThreadPoolMinThreads_ReturnsIfSet()
        {
            var file = new FakeFile(ProjectFileHelpers.GetNetCoreProjectWithElement("ThreadPoolMinThreads", "3"));
            file.ParseProject("test").NetCore.RuntimeOptions.ThreadPoolMinThreads.Should().Be(3);
        }

        [Fact]
        public void ParseProject_NetCore_ProjectReferences_ReturnIfSet()
        {
            var file = new FakeFile(ProjectFileHelpers.GetNetCoreProjectWithString("<ProjectReference Include=\"..\\a\\b.csproj\" /><ProjectReference Include=\"..\\c\\d.csproj\" />"), "c:/project/src/x.csproj");
            var references = file.ParseProject("test").NetCore.ProjectReferences;

            references.Should().HaveCount(2);

            var first = references.First();
            first.Name.Should().Be("b");
            first.RelativePath.Should().Be("../a/b.csproj");
            first.FilePath.ToString().Should().Be("c:/project/a/b.csproj");
            first.Package.Should().BeNull();
            first.Project.Should().BeNull();

            var second = references.Last();
            second.Name.Should().Be("d");
            second.RelativePath.Should().Be("../c/d.csproj");
            second.FilePath.ToString().Should().Be("c:/project/c/d.csproj");
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
            var file = new FakeFile(ProjectFileHelpers.GetNetCoreProjectWithString(packageRef));
            var references = file.ParseProject("test").NetCore.PackageReferences;

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
            var file = new FakeFile(ProjectFileHelpers.GetNetCoreProjectWithString(packageRef));
            var references = file.ParseProject("test").NetCore.PackageReferences;

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
            var file = new FakeFile(ProjectFileHelpers.GetNetCoreProjectWithString(packageRef));
            var references = file.ParseProject("test").NetCore.PackageReferences;

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
            var file = new FakeFile(ProjectFileHelpers.GetNetCoreProjectWithString(packageRef));
            var references = file.ParseProject("test").NetCore.PackageReferences;

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
        public void ParseProject_NetCore_PackageReference_ReturnsDuplicatesWithDifferentTargetFrameworkInAttributeIfSet()
        {
            var packageRef =
                @"<ItemGroup>
                    <PackageReference Include=""System.Collections.Immutable"" Version=""1.3.1"" Condition=""'$(TargetFramework)'== 'net451'"" />
                    <PackageReference Include=""System.Collections.Immutable"" Version=""1.3.2"" Condition=""'$(TargetFramework)'== 'netstandard1.5'"" />
                </ItemGroup>";
            var file = new FakeFile(ProjectFileHelpers.GetNetCoreProjectWithString(packageRef));
            var references = file.ParseProject("test").NetCore.PackageReferences;

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
            var file = new FakeFile(ProjectFileHelpers.GetNetCoreProjectWithString(cliRefs));
            var references = file.ParseProject("test").NetCore.DotNetCliToolReferences;

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
            var targetString = @"<Target Name=""Jogging"" BeforeTargets=""Stretch;Jump;;"" AfterTargets=""IceBath"" DependsOn=""Mood"">
                                  <Exec Command=""hop.cmd"" />
                                  <Exec Command=""skip.cmd"" />
                                </Target>";
            var file = new FakeFile(ProjectFileHelpers.GetNetCoreProjectWithString(targetString));
            var targets = file.ParseProject("test").NetCore.Targets;

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
            var file = new FakeFile(ProjectFileHelpers.GetNetCoreProjectWithString(null));
            file.ParseProject("test").OutputPath.ToString().Should().Be("bin/test");
        }

        [Fact]
        public void ParseProject_GetsCorrectOutputPath_FallbackConfigAndPlatform()
        {
            var file = new FakeFile(ProjectFileHelpers.GetNetCoreProjectWithString(null));
            file.ParseProject("test", "x86").OutputPath.ToString().Should().Be("bin/x86/test");
        }

        [Fact]
        public void ParseProject_GetsCorrectOutputPath_FallbackConfigPlatformAndTargetFramework()
        {
            var file = new FakeFile(ProjectFileHelpers.GetNetCoreProjectWithString(@"<PropertyGroup>
                            <TargetFramework>NetStandard1.6</TargetFramework>
                          </PropertyGroup>"));
            file.ParseProject("test", "x86").OutputPath.ToString().Should().Be("bin/x86/test/NetStandard1.6");
        }

        [Fact]
        public void ParseProject_GetsCorrectOutputPath_UsesConditionOverrideWithDefaultPlatform()
        {
            var file = new FakeFile(ProjectFileHelpers.GetNetCoreProjectWithString(@"<PropertyGroup Condition=""'$(Configuration)|$(Platform)'=='Test|AnyCPU'"">
                            <OutputPath>bin\wayhey\</OutputPath>
                          </PropertyGroup>"));
            file.ParseProject("test").OutputPath.ToString().Should().Be("bin/wayhey");
        }

        [Fact]
        public void ParseProject_GetsCorrectOutputPath_UsesConditionOverride()
        {
            var file = new FakeFile(ProjectFileHelpers.GetNetCoreProjectWithString(@"<PropertyGroup Condition=""'$(Configuration)|$(Platform)'=='Test|x86'"">
                            <OutputPath>bin\wayhey\</OutputPath>
                          </PropertyGroup>"));
            file.ParseProject("test", "x86").OutputPath.ToString().Should().Be("bin/wayhey");
        }
    }
}