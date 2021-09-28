// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/. 

using System.Linq;
using Cake.Testing;

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
            var file = fs.CreateFakeFile("CsProj_ValidFile".SafeLoad());
            var workDir = cakeContext.Environment.WorkingDirectory.FullPath;

            var result = cakeContext.GetOutputAssemblies(file.Path, "Debug", "AnyCPU");
            result.Should().ContainSingle().Which.FullPath.Should().Be($"{workDir}/bin/Debug/Cake.Common.dll");
        }

        [Fact]
        public void CanGetOutputAssemblies_For_VS2017CSProjFile()
        {
            var file = fs.CreateFakeFile("VS2017_CsProj_NetCoreDefault".SafeLoad(), "./abc.csproj");
            var workDir = cakeContext.Environment.WorkingDirectory.FullPath;

            var result = cakeContext.GetOutputAssemblies(file.Path, "Debug", "AnyCPU");
            result.Should().ContainSingle().Which.FullPath.Should().Be($"{workDir}/bin/custom/netcoreapp1.1/abc.dll");
        }

        [Fact]
        public void NullRefException_For_VS2017FSProjFile()
        {
            var file = fs.CreateFakeFile("Cake_Unity_FSharp_Tests_fsproj".SafeLoad(), "/tmp/abc.fsproj");

            var project = cakeContext.ParseProject(file.Path, "Release", "AnyCPU");

            project.IsFsUnitTestProject().Should().BeTrue();
        }

        [Fact]
        public void CanGetOutputAssemblies_For_VS2017CSProjFile_MultiTarget()
        {
            var projectString = ProjectFileHelpers.GetNetCoreProjectWithString(
                "<PropertyGroup><OutputType>Exe</OutputType><TargetFrameworks>netstandard2.0;net45</TargetFrameworks></PropertyGroup>");
            var file = fs.CreateFakeFile(projectString, "./def.csproj");
            var workDir = cakeContext.Environment.WorkingDirectory.FullPath;

            var result = cakeContext.GetOutputAssemblies(file.Path, "Release", "x64");
            result.Should().HaveCount(2).And
                .BeEquivalentTo(
                    new[]
                    {
                        new FilePath($"{workDir}/bin/x64/Release/netstandard2.0/def.exe"),
                        new FilePath($"{workDir}/bin/x64/Release/net45/def.exe")   
                    });
        }

        [Fact]
        public void CanGetOutputAssemblies_For_VS2017CSProjFile_DefaultPlatform()
        {
            var projectString = ProjectFileHelpers.GetNetCoreProjectWithString(
                "<PropertyGroup><OutputType>Exe</OutputType><TargetFrameworks>netstandard2.0;net45</TargetFrameworks></PropertyGroup>");
            var file = fs.CreateFakeFile(projectString, "./ghj.csproj");
            var workDir = cakeContext.Environment.WorkingDirectory.FullPath;

            var result = cakeContext.GetOutputAssemblies(file.Path, "Release");
            result.Should().HaveCount(2).And.BeEquivalentTo(
                new[]
                {
                    new FilePath($"{workDir}/bin/Release/netstandard2.0/ghj.exe"),
                    new FilePath($"{workDir}/bin/Release/net45/ghj.exe")
                });
        }

        [Fact]
        public void CanGetProjectAssemblies_For_VS2017CSProjFile_DefaultPlatform()
        {
            var projectString = ProjectFileHelpers.GetNetCoreProjectWithString(
                "<PropertyGroup><OutputType>Exe</OutputType><TargetFrameworks>netstandard2.0;net45</TargetFrameworks></PropertyGroup>");
            var file = fs.CreateFakeFile(projectString, "./klm.csproj");
            var workDir = cakeContext.Environment.WorkingDirectory.FullPath;

            var result = cakeContext.GetProjectAssemblies(file.Path, "Release");
            result.Should().HaveCount(2).And.BeEquivalentTo(
                new[]
                {
                    new FilePath($"{workDir}/bin/Release/netstandard2.0/klm.exe"),
                    new FilePath($"{workDir}/bin/Release/net45/klm.exe")    
                });
        }

        [Fact]
        public void CanGetProjectAssemblies_For_VS2017CSProjFile_WithPlatform()
        {
            var projectString = ProjectFileHelpers.GetNetCoreProjectWithString(
                "<PropertyGroup><OutputType>Exe</OutputType><TargetFrameworks>netstandard2.0;net45</TargetFrameworks></PropertyGroup>");
            var file = fs.CreateFakeFile(projectString, "./nop.csproj");
            var workDir = cakeContext.Environment.WorkingDirectory.FullPath;

            var result = cakeContext.GetProjectAssemblies(file.Path, "Release", "x86");
            result.Should().HaveCount(2).And.BeEquivalentTo(
                new[]
                {
                    new FilePath($"{workDir}/bin/x86/Release/netstandard2.0/nop.exe"),
                    new FilePath($"{workDir}/bin/x86/Release/net45/nop.exe")
                });
        }
    }
}