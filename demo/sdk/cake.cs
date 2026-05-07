#!/usr/bin/env dotnet
#:sdk Cake.Sdk@6.1.1
#:project ../../src/Cake.Incubator/Cake.Incubator.csproj

// Cake SDK consumer demo for Cake.Incubator. Runs as a file-based
// .NET program (introduced in .NET 10) using the Cake.Sdk
// directives. The #:project directive above lets the SDK build the
// addin from source rather than referencing a published nupkg.
//
// To run locally:
//   cd demo/sdk
//   dotnet cake.cs
//
// Mirrors the alias surface exercised by demo/script/incubator.cake
// and demo/frosting/.

using Cake.Incubator.EnumerableExtensions;
using Cake.Incubator.StringExtensions;

var workDir = Directory("./BuildArtifacts/temp/test-incubator-sdk");
var sampleProject = workDir + File("Sample.csproj");

Task("Default")
    .IsDependentOn("Setup")
    .IsDependentOn("Parse-Project")
    .IsDependentOn("String-Extensions")
    .IsDependentOn("Enumerable-Extensions")
    .IsDependentOn("Cleanup");

Task("Setup")
    .Does(() =>
{
    if (DirectoryExists(workDir))
    {
        DeleteDirectory(workDir, new DeleteDirectorySettings { Recursive = true });
    }

    EnsureDirectoryExists(workDir);
    System.IO.File.WriteAllText(
        sampleProject.Path.FullPath,
        @"<Project Sdk=""Microsoft.NET.Sdk"">
  <PropertyGroup>
    <TargetFramework>net8.0</TargetFramework>
    <OutputType>Library</OutputType>
    <AssemblyName>SampleAddin</AssemblyName>
    <RootNamespace>Sample</RootNamespace>
  </PropertyGroup>
  <ItemGroup>
    <PackageReference Include=""Cake.Core"" Version=""6.0.0"" />
  </ItemGroup>
</Project>");
    Information("Setup complete.");
});

Task("Parse-Project")
    .IsDependentOn("Setup")
    .Does(() =>
{
    var result = ParseProject(sampleProject, "Debug");
    AssertThat(result != null, "ParseProject: returned null");
    AssertThat(result.OutputType.EqualsIgnoreCase("Library"), "OutputType: expected Library, got " + result.OutputType);
    AssertThat(result.AssemblyName == "SampleAddin", "AssemblyName: expected SampleAddin, got " + result.AssemblyName);
    AssertThat(result.RootNameSpace == "Sample", "RootNameSpace: expected Sample, got " + result.RootNameSpace);
    Information("ParseProject OK (OutputType={0}, AssemblyName={1}, RootNameSpace={2})",
        result.OutputType, result.AssemblyName, result.RootNameSpace);
});

Task("String-Extensions")
    .Does(() =>
{
    AssertThat("Hello".EqualsIgnoreCase("HELLO"), "EqualsIgnoreCase: case mismatch");
    AssertThat("HelloWorld".StartsWithIgnoreCase("hello"), "StartsWithIgnoreCase: case mismatch");
    AssertThat("HelloWorld".EndsWithIgnoreCase("WORLD"), "EndsWithIgnoreCase: case mismatch");
    AssertThat(!"Hello".EqualsIgnoreCase("Goodbye"), "EqualsIgnoreCase: false-positive on different strings");
    Information("String extensions OK (EqualsIgnoreCase + StartsWithIgnoreCase + EndsWithIgnoreCase)");
});

Task("Enumerable-Extensions")
    .Does(() =>
{
    string[] nullArray = null;
    AssertThat(nullArray.IsNullOrEmpty(), "IsNullOrEmpty: null array should be true");
    AssertThat(new string[0].IsNullOrEmpty(), "IsNullOrEmpty: empty array should be true");
    AssertThat(!new[] { "a" }.IsNullOrEmpty(), "IsNullOrEmpty: non-empty array should be false");
    AssertThat("foo".IsIn("foo", "bar", "baz"), "IsIn: 'foo' should be in list");
    AssertThat(!"qux".IsIn("foo", "bar"), "IsIn: 'qux' should NOT be in list");
    Information("Enumerable extensions OK (IsNullOrEmpty + IsIn)");
});

Task("Cleanup")
    .IsDependentOn("Parse-Project")
    .IsDependentOn("String-Extensions")
    .IsDependentOn("Enumerable-Extensions")
    .Does(() =>
{
    if (DirectoryExists(workDir))
    {
        DeleteDirectory(workDir, new DeleteDirectorySettings { Recursive = true });
    }

    Information("Cleanup complete.");
});

RunTarget("Default");

// ----- Helpers (must come AFTER top-level statements per CS8803) -----

static void AssertThat(bool condition, string message)
{
    if (!condition)
    {
        throw new Exception("Assertion failed: " + message);
    }
}
