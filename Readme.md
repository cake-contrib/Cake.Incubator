# Cake Extensions

[![Build status](https://ci.appveyor.com/api/projects/status/2wn5r21h6hkpuyrx/branch/master?svg=true)](https://ci.appveyor.com/project/MacLeanElectrical/cake-extensions/branch/master)
[![NuGet version](https://badge.fury.io/nu/Cake.Extensions.svg)](https://badge.fury.io/nu/Cake.Extensions)

A set of extension methods and aliases for [Cake](http://cakebuild.net)

###

Usage: inside build.cake

```csharp
#addin "Cake.Extensions"
// or
#addin "nuget?package=Cake.Extensions"
```

### Assertion Extensions

```csharp
object arg  = null;
var asserted = arg.ThrowIfNull(nameof(arg)); // throws ArgumentNullException("arg");
```

```csharp
var arg = "";
var asserted = arg.ThrowIfNullOrEmpty(nameof(arg)); // throws ArgumentNullException("arg");
```

### Enumerable Extensions

```csharp
var items = new[] { "One", "Two" };
Action<string> action = (item) => item + "append";
items.Each(item => action(item))
```

### FilePath Extensions

identify solution filepaths
```csharp
new FilePath("test.sln").IsSolution(); // true
```

identify project filepaths
```csharp
new FilePath("test.csproj").IsProject(); //true;
```

identify filepaths by filename
```csharp
new FilePath("/folder/testing.cs").HasFileName("testing.cs"); // true
```

multiple file globs
```csharp
// Alias : GetFiles(params string[] patterns) accepts multiple globs
IEnumerable<FilePath> filePaths = GetFiles("**/*.sln", "**/*.csproj");
```

get solution or project assembly filepaths
```csharp
// Alias : GetOutputAssemblies(FilePath solutionOrProject, string configuration)
IEnumerable<FilePath> filePaths = GetOutputAssemblies(new FilePath("test.cs"), "Debug"); // throws ArgumentException("not a project or solution file")
IEnumerable<FilePath> filePaths = GetOutputAssemblies(new FilePath("test.sln"), "Release"); // returns solution output dll/exe's FilePath[] for 'Release' configuration
IEnumerable<FilePath> filePaths = GetOutputAssemblies(new FilePath("test.csproj", "Custom"); // returns project output dll/exe's FilePath[] for 'Custom' configuration
```

get solution assembly filepaths
```csharp
// Alias : GetSolutionAssemblies(FilePath solution, string configuration)
IEnumerable<FilePath> filePaths = GetSolutionAssemblies(new FilePath("test.csproj", "Debug"); // throws ArgumentException("not a solution file")
IEnumerable<FilePath> filePaths = GetSolutionAssemblies(new FilePath("test.sln", "Release"); // returns solution output dll/exe's FilePath[] for 'Release' configuration
```

get project assembly filepath 
```csharp
// Alias : GetProjectAssembly(FilePath solution, string configuration)
IEnumerable<FilePath> filePaths = GetProjectAssembly(new FilePath("test.sln"), "Debug"); // throws ArgumentException("not a project file")
IEnumerable<FilePath> filePaths = GetProjectAssembly(new FilePath("test.csproj"), "Release"); // returns solution output dll/exe FilePath for 'Release' configuration
```
get files with the same name but different extensions in the same directory
```csharp
// Alias : GetMatchingFiles(FilePath file)
// file: output/file.dll
// file: output/file.xml
// file: output/file.pdb
// file: output/another.dll

IEnumerable<FilePath> matchingFiles = GetMatchingFiles(assemblies);

matchingFiles[0]; // output/file.xml
matchingFiles[1]; // output/file.pdb

```
### Solution Extensions

identify solution folders
```csharp
// test.sln { proj1.csproj, solutionFolder }
var projects = ParseSolution(new FilePath("test.sln")).Projects;
projects[0].IsSolutionFolder(); // false
projects[1].IsSolutionFolder(); // true
```

filter out solution folders
```csharp
// test.sln { proj1.csproj, solutionFolder, solutionFolder/proj2.csproj }
var projects = ParseSolution(new FilePath("test.sln")).Projects;
projects.GetProjects(); // returns proj1.csproj, proj2.csproj
```

### Project Extensions

identify library projects
```csharp
// is dll or console app?
ParseProject(new FilePath("test.csproj")).IsLibrary();         
```

get project assembly filepath
```csharp
// returns {outputDir}/{AssemblyName}.[dll|exe]
FilePath assemblyPath = ParseProject(new FilePath("test.csproj")).GetAssemblyFilePath();         
```

get config specific project information
```csharp
// Alias : ParseProject(FilePath project, string configuration)
// overload that returns config specific project info
ParseProjectResult project = ParseProject(new FilePath("test.csproj"), "Release");
```

### DotnetBuildSettings extensions

Add multiple targets
```csharp
var settings = new DotNetBuildSettings()
				.WithTargets(new[] { "Clean", "Build", "Publish" });
```
