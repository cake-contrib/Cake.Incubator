# Cake Extensions

A set of extension methods and aliases for [Cake](http://cakebuild.net)

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

```csharp
new FilePath("test.sln").IsSolution(); // true
```

```csharp
new FilePath("test.csproj").IsProject(); //true;
```

```csharp
new FilePath("/folder/testing.cs").HasFileName("testing.cs"); // true
```

```csharp
// Alias : GetFiles(params string[] patterns) accepts multiple globs
IEnumerable<FilePath> filePaths = GetFiles("**/*.sln", "**/*.csproj");
```

```csharp
// Alias : GetOutputAssemblies(FilePath solutionOrProject, string configuration)
IEnumerable<FilePath> filePaths = GetOutputAssemblies(new FilePath("test.cs"), "Debug"); // throws ArgumentException("not a project or solution file")
IEnumerable<FilePath> filePaths = GetOutputAssemblies(new FilePath("test.sln"), "Release"); // returns solution output dll/exe's FilePath[] for 'Release' configuration
IEnumerable<FilePath> filePaths = GetOutputAssemblies(new FilePath("test.csproj", "Custom"); // returns project output dll/exe's FilePath[] for 'Custom' configuration
```

```csharp
// Alias : GetSolutionAssemblies(FilePath solution, string configuration)
IEnumerable<FilePath> filePaths = GetSolutionAssemblies(new FilePath("test.csproj", "Debug"); // throws ArgumentException("not a solution file")
IEnumerable<FilePath> filePaths = GetSolutionAssemblies(new FilePath("test.sln", "Release"); // returns solution output dll/exe's FilePath[] for 'Release' configuration
```

```csharp
// Alias : GetProjectAssembly(FilePath solution, string configuration)
IEnumerable<FilePath> filePaths = GetProjectAssembly(new FilePath("test.sln"), "Debug"); // throws ArgumentException("not a project file")
IEnumerable<FilePath> filePaths = GetProjectAssembly(new FilePath("test.csproj"), "Release"); // returns solution output dll/exe FilePath for 'Release' configuration
```

### Solution Extensions

```csharp
// test.sln { proj1.csproj, solutionFolder }
var projects = ParseSolution(new FilePath("test.sln")).Projects;
projects[0].IsSolutionFolder(); // false
projects[1].IsSolutionFolder(); // true
```

```csharp
// test.sln { proj1.csproj, solutionFolder, solutionFolder/proj2.csproj }
var projects = ParseSolution(new FilePath("test.sln")).Projects;
projects.GetProjects(); // returns proj1.csproj, proj2.csproj
```

### Project Extensions