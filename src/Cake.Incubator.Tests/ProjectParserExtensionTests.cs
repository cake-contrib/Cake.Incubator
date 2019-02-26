// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/. 
namespace Cake.Incubator.Tests
{
    using Cake.Core;
    using Cake.Core.IO;
    using Cake.Incubator.Project;
    using FluentAssertions;
    using Xunit;

    public class ProjectParserExtensionTests : IClassFixture<CakeFixture>
    {
        private readonly CakeContext cakeContext;
        private readonly FakeFileSystem fs;

        public ProjectParserExtensionTests(CakeFixture fixture)
        {
            fs = fixture.FileSystem;
            cakeContext = fixture.Context;
        }

        [Fact]
        public void CanGetOutputAssemblies_For_CSProjFile()
        {
            var file = new FakeFile("CsProj_ValidFile".SafeLoad());
            fs.AddFile(file);

            var result = cakeContext.GetOutputAssemblies(file.Path, "Debug", "AnyCPU");
            result.Should().ContainSingle().Which.FullPath.Should().Be("bin/Debug/Cake.Common.dll");
        }

        [Fact]
        public void CanGetOutputAssemblies_For_VS2017CSProjFile()
        {
            var file = new FakeFile("VS2017_CsProj_NetCoreDefault".SafeLoad(), "./abc.csproj");
            fs.AddFile(file);

            var result = cakeContext.GetOutputAssemblies(file.Path, "Debug", "AnyCPU");
            result.Should().ContainSingle().Which.FullPath.Should().Be("bin/custom/netcoreapp1.1/abc.dll");
        }

        [Fact]
        public void CanGetOutputAssemblies_For_VS2017CSProjFile_MultiTarget()
        {
            var projectString = ProjectFileHelpers.GetNetCoreProjectWithString(
                "<PropertyGroup><OutputType>Exe</OutputType><TargetFrameworks>netstandard2.0;net45</TargetFrameworks></PropertyGroup>");
            var file = new FakeFile(projectString, "./def.csproj");
            fs.AddFile(file);

            var result = cakeContext.GetOutputAssemblies(file.Path, "Release", "x64");
            result.Should().HaveCount(2).And
                .BeEquivalentTo(new FilePath("bin/x64/Release/netstandard2.0/def.exe"),
                    new FilePath("bin/x64/Release/net45/def.exe"));
        }

        [Fact]
        public void CanGetOutputAssemblies_For_VS2017CSProjFile_DefaultPlatform()
        {
            var projectString = ProjectFileHelpers.GetNetCoreProjectWithString(
                "<PropertyGroup><OutputType>Exe</OutputType><TargetFrameworks>netstandard2.0;net45</TargetFrameworks></PropertyGroup>");
            var file = new FakeFile(projectString, "./ghj.csproj");
            fs.AddFile(file);

            var result = cakeContext.GetOutputAssemblies(file.Path, "Release");
            result.Should().HaveCount(2).And.BeEquivalentTo(new FilePath("bin/Release/netstandard2.0/ghj.exe"),
                new FilePath("bin/Release/net45/ghj.exe"));
        }

        [Fact]
        public void CanGetProjectAssemblies_For_VS2017CSProjFile_DefaultPlatform()
        {
            var projectString = ProjectFileHelpers.GetNetCoreProjectWithString(
                "<PropertyGroup><OutputType>Exe</OutputType><TargetFrameworks>netstandard2.0;net45</TargetFrameworks></PropertyGroup>");
            var file = new FakeFile(projectString, "./klm.csproj");
            fs.AddFile(file);

            var result = cakeContext.GetProjectAssemblies(file.Path, "Release");
            result.Should().HaveCount(2).And.BeEquivalentTo(new FilePath("bin/Release/netstandard2.0/klm.exe"),
                new FilePath("bin/Release/net45/klm.exe"));
        }

        [Fact]
        public void CanGetProjectAssemblies_For_VS2017CSProjFile_WithPlatform()
        {
            var projectString = ProjectFileHelpers.GetNetCoreProjectWithString(
                "<PropertyGroup><OutputType>Exe</OutputType><TargetFrameworks>netstandard2.0;net45</TargetFrameworks></PropertyGroup>");
            var file = new FakeFile(projectString, "./nop.csproj");
            fs.AddFile(file);

            var result = cakeContext.GetProjectAssemblies(file.Path, "Release", "x86");
            result.Should().HaveCount(2).And.BeEquivalentTo(new FilePath("bin/x86/Release/netstandard2.0/nop.exe"),
                new FilePath("bin/x86/Release/net45/nop.exe"));
        }
    }
}