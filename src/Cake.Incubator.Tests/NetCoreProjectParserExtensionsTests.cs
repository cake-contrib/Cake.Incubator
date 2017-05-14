// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/. 
namespace Cake.Incubator.Tests
{
    using FluentAssertions;
    using Xunit;

    public class NetCoreProjectParserExtensionsTests
    {
        [Fact]
        public void ParseProject_GetsCorrectAssemblyName()
        {
            var file = new FakeFile(ProjectFileHelpers.GetNetCoreProjectWithElement("AssemblyName", "a"));
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

        [Fact]
        public void ParseProject_SetsIsNetCore()
        {
            var file = new FakeFile(ProjectFileHelpers.GetNetCoreProjectWithString(null));
            file.ParseProject("test").IsNetCore.Should().BeTrue();
        }

        [Fact]
        public void ParseProject_SetsIsNetCoreForWeb()
        {
            var file = new FakeFile(ProjectFileHelpers.GetNetCoreProjectWithString(null));
            file.ParseProject("test").IsNetCore.Should().BeTrue();
        }

        [Fact]
        public void ParseProject_DoesNotSetIsNetFramework()
        {
            var file = new FakeFile(ProjectFileHelpers.GetNetCoreProjectWithElement("AssemblyName", "a"));
            file.ParseProject("test").IsNetFramework.Should().BeFalse();
        }

        [Fact]
        public void ParseProject_GetsCorrectOutputType_WhenNoneSpecified()
        {
            var file = new FakeFile(ProjectFileHelpers.GetNetCoreProjectWithElement("AssemblyName", "a"));
            file.ParseProject("test").OutputType.Should().Be("Library");
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
        public void ParseProject_IgnoresRootnamespace()
        {
            var file = new FakeFile(ProjectFileHelpers.GetNetCoreProjectWithString(null));
            file.ParseProject("test").RootNameSpace.Should().BeNull();
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
        public void ParseProject_NetCore_AssemblyOriginatorKeyFile_ReturnsIfSet()
        {
            var file = new FakeFile(ProjectFileHelpers.GetNetCoreProjectWithElement("AssemblyOriginatorKeyFile", "key.snk"));
            file.ParseProject("test").NetCore.AssemblyOriginatorKeyFile.Should().Be("key.snk");
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

        [Fact]
        public void ParseProject_NetCore_Description_ReturnsIfSet()
        {
            var file = new FakeFile(ProjectFileHelpers.GetNetCoreProjectWithElement("Description", "Howdy"));
            file.ParseProject("test").NetCore.Description.Should().Be("Howdy");
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
            file.ParseProject("test").NetCore.NoWarn.Should().BeEquivalentTo("CS3132","CS2534");
        }

        [Fact]
        public void ParseProject_NetCore_PackageIconUrl_ReturnsIfSet()
        {
            var file = new FakeFile(ProjectFileHelpers.GetNetCoreProjectWithElement("PackageIconUrl", "http://acme.inc/fav.ico"));
            file.ParseProject("test").NetCore.PackageIconUrl.Should().Be("http://acme.inc/fav.ico");
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
        public void ParseProject_NetCore_RepositoryType_ReturnsIfSet()
        {
            var file = new FakeFile(ProjectFileHelpers.GetNetCoreProjectWithElement("RepositoryType", "gitgat"));
            file.ParseProject("test").NetCore.RepositoryType.Should().Be("gitgat");
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
            var file = new FakeFile(ProjectFileHelpers.GetNetCoreProjectWithElement("TargetFrameworks", "net45;net46;###**%%.1;;"));
            file.ParseProject("test").NetCore.TargetFrameworks.Should().BeEquivalentTo("net45","net46", "###**%%.1");
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
            var file = new FakeFile(ProjectFileHelpers.GetNetCoreProjectWithString("<VersionPrefix>1.2.3</VersionPrefix><VersionSuffix>5813</VersionSuffix>"));
            file.ParseProject("test").NetCore.Version.Should().Be("1.2.3.5813");
        }

        [Fact]
        public void ParseProject_NetCore_Version_ReturnsIfSemver()
        {
            var file = new FakeFile(ProjectFileHelpers.GetNetCoreProjectWithString("<VersionPrefix>1.2.3</VersionPrefix><VersionSuffix>alpha5813</VersionSuffix>"));
            file.ParseProject("test").NetCore.Version.Should().Be("1.2.3-alpha5813");
        }

        [Fact]
        public void ParseProject_NetCore_Version_VersionFallback()
        {
            var file = new FakeFile(ProjectFileHelpers.GetNetCoreProjectWithElement("Version", "1.2.3.5"));
            file.ParseProject("test").NetCore.Version.Should().Be("1.2.3.5");
        }

        // TODO: Output Paths, Conditions, PackageReference, ProjectReference, RuntimeOptions
        [Fact(Skip="need to check conditions")]
        public void ParseProject_GetsCorrectOutputPath()
        {
            var file = new FakeFile(ProjectFileHelpers.GetNetCoreProjectWithElement("OutputPath", "b"));
            file.ParseProject("test").OutputPath.Should().Be("b");
        }
    }
}