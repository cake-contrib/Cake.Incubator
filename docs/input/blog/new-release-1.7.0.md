---
Title: New Release - 1.7.0
Published: 25/01/2018
Category: Release
Author: wwwlicious
---

# 1.7.0 Release

As part of this release we had [7 issues](https://github.com/cake-contrib/Cake.Incubator/issues?milestone=10&state=closed) closed.

The main feature of this release is the improvement in project parsing and test project detection for both pre and post 2017 project formats.

You can now detect the type of project using:

```csharp
CustomProjectParserResult result = ParseProject("./some.csproj");

result.IsNetStandard; // true | false
result.IsNetCore; // true | false
result.IsNetFramework; // true | false
```

Combined with the existing methods below creates a powerful way to filter and route your projects during your build pipeline.

```csharp
result.IsLibrary(); // true | false
result.IsType(ProjectTypes.FSharp); // true | false
result.IsWebApplication(); // true | false
```

The flags are not mutually exclusive so support those projects with multi-targeting configured.
The support for multi-targeting has also been improved with new Properties being added as follows

```csharp
result.GetOutputPaths(); // uses target frameworks to generate all of the artifact output paths (dll's, exe's)
result.GetAssemblyFilePaths(); // again will now include all possible artifact output paths (dll's, exe's)
```

Checking packages and references has also improved

```csharp
result.HasPackage("nunit"); // true | false 
result.HasPackage("nunit", "net45"); // also supports targetframework specific package lookups
var pkg = result.GetPackage("nunit"); // returns PackageReference object or null

result.HasReference("xunit.core"); // true | false
result.GetReferemce("xunit.core"); // returns ProjectAssemblyReference object or null
```

The test detection makes the above even easier to work across a range of projects with the following new test extensions, especially if you are using `dotnet test` for running your tests.

```csharp
result.IsTestProject(); // true | false (currently works for nunit, xunit, mstest, fsunit, fixie, Expecto)
result.IsDotNetCliTestProject(); // true is test project can be executed with 'dotnet test' otherwise false
result.IsFrameworkTestProject(); // true if project will require a non 'dotnet test' runner to be executed
```

There are a few more minor improvements also. Check the issues below for more details.


__Bug__

- [__#54__](https://github.com/cake-contrib/Cake.Incubator/issues/54) Fix detection of netcore, netframework and add netstandard

__Improvements__

- [__#58__](https://github.com/cake-contrib/Cake.Incubator/issues/58) Populate project references for vs2017 project formats
- [__#56__](https://github.com/cake-contrib/Cake.Incubator/pull/56) Feature/testprojectdetection
- [__#55__](https://github.com/cake-contrib/Cake.Incubator/issues/55) Updating output paths to support TargetFrameworks
- [__#53__](https://github.com/cake-contrib/Cake.Incubator/issues/53) Test project detection
- [__#52__](https://github.com/cake-contrib/Cake.Incubator/issues/52) Add support for ienumerable iteration for dump extension
- [__#51__](https://github.com/cake-contrib/Cake.Incubator/issues/51) Make detecting TargetFramework for PackageReference more robust

