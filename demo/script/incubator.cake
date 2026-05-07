#reference "../../BuildArtifacts/temp/_PublishedLibraries/Cake.Incubator/net8.0/Cake.Incubator.dll"

// Self-contained smoke test of Cake.Incubator's marquee features:
// project parsing (the [CakeMethodAlias] surface) plus a couple of
// extension-method namespaces that are auto-imported via the addin's
// CakeNamespaceImportAttribute set (StringExtensions, EnumerableExtensions).
// Each task asserts the expected outcome and throws on mismatch — the
// script fails (non-zero exit) if any alias misbehaves.

var workDir = Directory("./BuildArtifacts/temp/test-incubator");
var sampleProject = workDir + File("Sample.csproj");

void AssertThat(bool condition, string message)
{
    if (!condition)
    {
        throw new Exception("Assertion failed: " + message);
    }
}

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
