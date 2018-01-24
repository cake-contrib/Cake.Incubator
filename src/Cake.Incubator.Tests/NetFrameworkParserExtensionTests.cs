namespace Cake.Incubator.Tests
{
    using Cake.Core.IO;
    using FluentAssertions;
    using Xunit;

    public class NetFrameworkParserExtensionTests
    {
        [Fact(Skip="To fix - Runs locally, fails when shadow copied during cake build")]
        public void ParseCakeIncubatorProject_ReturnsAsExpected()
        {
            var fileSystem = new Cake.Core.IO.FileSystem();
            var file = fileSystem.GetFile(new FilePath("../../../Cake.Incubator/Cake.Incubator.csproj"));
            var project = file.ParseProject("Release");

            project.AssemblyName.Should().Be("Cake.Incubator");
            project.Configuration.Should().Be("Release");
            project.IsNetFramework.Should().BeTrue();
            project.IsNetStandard.Should().BeFalse();
            project.IsNetCore.Should().BeFalse();
            project.NetCore.Should().BeNull();
            project.OutputPath.ToString().Should().Be("../../../Cake.Incubator/bin/Release");
            project.OutputType.Should().Be("Library");
            project.Platform.Should().Be("AnyCPU");
            project.ProjectReferences.Should().BeEmpty();
            project.References.Should().ContainSingle(x => x.Name.Equals("Cake.Common"));
            project.References.Should().ContainSingle(x => x.Name.Equals("Cake.Core"));
            project.RootNameSpace.Should().Be("Cake.Incubator");
            project.TargetFrameworkVersion.Should().Be("v4.5");
            project.TargetFrameworkVersions.Should().ContainSingle().And.Equal("v4.5");
        }
    }
}
