// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/. 

namespace Cake.Incubator
{
    using System;
    using System.Linq;
    using System.Xml.Linq;
    using Cake.Common.Diagnostics;
    using Cake.Common.Solution.Project;
    using Cake.Core;
    using Cake.Core.Annotations;
    using Cake.Core.IO;

    /// <summary>
    /// Extension methods for parsing msbuild projects (csproj, vbproj, fsproj)
    /// </summary>
    [CakeAliasCategory("MSBuild Resource")]
    public static class ProjectParserExtensions
    {
        /// <summary>
        /// Checks if the project is a library
        /// </summary>
        /// <param name="projectParserResult">the parsed project</param>
        /// <returns>true if the project is a library</returns>
        /// <example>
        /// Check if a parsed project is a library or exe
        /// <code>
        /// CustomParseProjectResult project = ParseProject(new FilePath("test.csproj"), "Release");
        /// if (project.IsLibrary()) { ... }
        /// </code>
        /// </example>
        public static bool IsLibrary(this CustomProjectParserResult projectParserResult)
        {
            return projectParserResult.OutputType.Equals("Library", StringComparison.InvariantCultureIgnoreCase);
        }

        /// <summary>
        /// Returns the parsed projects output assembly extension
        /// </summary>
        /// <param name="projectParserResult">the parsed project</param>
        /// <returns>the output assembly's file extension</returns>
        /// <example>
        /// Gets the output assembly extension
        /// <code>
        /// CustomParseProjectResult project = ParseProject(new FilePath("test.csproj"), "Release");
        /// project.GetExtension(); // ".dll" or ".exe"
        /// </code>
        /// </example>
        public static string GetExtension(this CustomProjectParserResult projectParserResult)
        {
            return projectParserResult.IsLibrary()
                ? ".dll"
                : ".exe";
        }

        /// <summary>
        /// Gets a parsed projects output assembly path
        /// </summary>
        /// <param name="projectParserResult">the parsed project</param>
        /// <returns>the output assembly path</returns>
        /// <example>
        /// Returns the absolute project assembly file path, respects build config and platform settings
        /// <code>
        /// CustomParseProjectResult project = ParseProject(new FilePath("test.csproj"), "Release");
        /// project.GetAssemblyFilePath(); // returns '/root/project/bin/release/test.dll'
        /// </code>
        /// </example>
        public static FilePath GetAssemblyFilePath(this CustomProjectParserResult projectParserResult)
        {
            return
                projectParserResult.OutputPath.CombineWithFilePath(projectParserResult.AssemblyName +
                                                                   projectParserResult.GetExtension());
        }

        /// <summary>
        /// Checks the parsed projects type
        /// </summary>
        /// <param name="projectParserResult">the parsed project</param>
        /// <param name="projectType">the <see cref="ProjectType"/> to check</param>
        /// <returns>true if the project type matches</returns>
        /// <remarks>Project Types are not supported in NetCore, this extension is for the Net Framework only</remarks>
        /// <example>
        /// Checks the project type
        /// <code>
        /// CustomParseProjectResult project = ParseProject(new FilePath("test.csproj"), "Release");
        /// project.IsType(ProjectType.CSharp); // true 
        /// </code>
        /// </example>
        public static bool IsType(this CustomProjectParserResult projectParserResult, ProjectType projectType)
        {
            return projectType.HasFlag(ProjectType.Unspecified) ? projectParserResult.ProjectTypeGuids.IsNullOrEmpty() : projectParserResult.ProjectTypeGuids.Any(x => x.EqualsIgnoreCase(SolutionParserExtensions.Types[projectType]));
        }

        /// <summary>
        /// Parses a csproj file into a strongly typed <see cref="CustomProjectParserResult"/> object
        /// </summary>
        /// <param name="context">the cake context</param>
        /// <param name="project">the project filepath</param>
        /// <param name="configuration">the build configuration</param>
        /// <returns>The parsed project</returns>
        /// <remarks>Defaults to 'AnyCPU' platform, use overload to override this default</remarks>
        /// <example>
        /// Returns the project information specific to a build configuration
        /// <code>
        /// CustomParseProjectResult project = ParseProject(new FilePath("test.csproj"), configuration: "Release");
        /// </code>
        /// </example>
        [CakeMethodAlias]
        public static CustomProjectParserResult ParseProject(this ICakeContext context, FilePath project, string configuration)
        {
            return context.ParseProject(project, configuration, "AnyCPU");
        }

        /// <summary>
        /// Parses a csproj file into a strongly typed <see cref="CustomProjectParserResult"/> object
        /// </summary>
        /// <param name="context">the cake context</param>
        /// <param name="project">the project filepath</param>
        /// <param name="configuration">the build configuration</param>
        /// <param name="platform">the build platform</param>
        /// <returns>The parsed project</returns>
        /// <example>
        /// Returns the project information specific to a build configuration
        /// <code>
        /// CustomParseProjectResult project 
        ///         = ParseProject(new FilePath("test.csproj"), configuration: "Release", platform: "x86");
        /// </code>
        /// </example>
        [CakeMethodAlias]
        public static CustomProjectParserResult ParseProject(this ICakeContext context, FilePath project, string configuration, string platform)
        {
            project.ThrowIfNull(nameof(project));
            configuration.ThrowIfNullOrEmpty(nameof(configuration));
            platform.ThrowIfNullOrEmpty(nameof(platform));

            if (project.IsRelative)
            {
                project = project.MakeAbsolute(context.Environment);
            }

            var projectFile = context.FileSystem.GetProjectFile(project);
            var result = projectFile.ParseProject(configuration, platform);

            context.Debug($"Parsed project file {project}\r\n{result.Dump()}");
            return result;
        }


        /// <summary>
        /// Parses a csproj file into a strongly typed <see cref="CustomProjectParserResult"/> object
        /// </summary>
        /// <returns>The parsed project</returns>
        /// <param name="projectFile">the project file</param>
        /// <param name="configuration">the build configuration</param>
        /// <param name="platform">the build configuration platform, defaults to AnyCPU if not specified</param>
        /// <returns>The parsed project</returns>
        /// <example>
        /// Returns the project information specific to a build configuration
        /// <code>
        /// CustomParseProjectResult project 
        ///         = new File("./test.csproj").ParseProject(configuration: "Release", platform: "x86");
        /// </code>
        /// </example>
        public static CustomProjectParserResult ParseProject(this IFile projectFile, string configuration, string platform = "AnyCPU")
        {
            projectFile.ThrowIfNull(nameof(projectFile));
            configuration.ThrowIfNullOrEmpty(nameof(configuration));

            var document = projectFile.LoadXml();

            return document.IsDotNetSdk()
                ? document.ParseNetcoreProjectFile(projectFile, configuration, platform)
                : document.ParseNetFramework(projectFile, configuration, platform);
        }

        internal static CustomProjectParserResult ParseNetFramework(this XDocument document, IFile projectFile, string config, string platform)
        {
            var rootPath = projectFile.Path.GetDirectory();

            var ns = document.Root.Name?.Namespace;
            var projectProperties = GetNetFrameworkProjectProperties(document, config, platform, ns, rootPath);

            if (projectProperties == null)
            {
                throw new CakeException("Failed to parse .net framework project properties");
            }

            var projectFiles = GetNetFrameworkMSBuildProjects(document, ns, rootPath);
            var references = GetNetFrameworkReferences(document, ns, rootPath);
            var projectReferences = GetNetFrameworkProjectReferences(document, ns, rootPath);

            return new CustomProjectParserResult
            {
                Configuration = projectProperties.Configuration,
                Platform = projectProperties.Platform,
                ProjectGuid = projectProperties.ProjectGuid,
                ProjectTypeGuids = projectProperties.ProjectTypeGuids,
                OutputType = projectProperties.OutputType,
                OutputPath = projectProperties.OutputPath,
                RootNameSpace = projectProperties.RootNameSpace,
                AssemblyName = projectProperties.AssemblyName,
                TargetFrameworkVersion = projectProperties.TargetFrameworkVersion,
                TargetFrameworkProfile = projectProperties.TargetFrameworkProfile,
                Files = projectFiles,
                References = references,
                ProjectReferences = projectReferences,
                IsNetFramework = true
            };
        }

        internal static CustomProjectParserResult ParseNetcoreProjectFile(this XDocument document, IFile projectFile, string config, string platform)
        {
            var sdk = document.GetSdk();
            var version = document.GetVersion();
            var targetFramework = document.GetFirstElementValue(ProjectXElement.TargetFramework);
            var targetFrameworks = document.GetFirstElementValue(ProjectXElement.TargetFrameworks)?.Split(';') ?? new[] { targetFramework };
            var outputType = document.GetFirstElementValue(ProjectXElement.OutputType) ?? "Library";
            var debugType = document.GetFirstElementValue(ProjectXElement.DebugType);
            var defaultOutputPath = projectFile.Path.GetDirectory()?.Combine($"bin/{config}/{targetFramework}/");
            var outputPath = document.GetOutputPath(config) ?? defaultOutputPath;
            var packageReferences = document.GetPackageReferences();
            var projectReferences = document.GetProjectReferences(projectFile.Path.GetDirectory());
            var assemblyName = document.GetFirstElementValue(ProjectXElement.AssemblyName) ?? $"{projectFile.Path.GetFilenameWithoutExtension()}";
            var packageId = document.GetFirstElementValue(ProjectXElement.PackageId) ?? assemblyName;
            var authors = document.GetFirstElementValue(ProjectXElement.Authors)?.Split(';') ?? new string[0];
            var company = document.GetFirstElementValue(ProjectXElement.Company);
            var neutralLang = document.GetFirstElementValue(ProjectXElement.NeutralLanguage);
            var assemblyTitle = document.GetFirstElementValue(ProjectXElement.AssemblyTitle);
            var description = document.GetFirstElementValue(ProjectXElement.Description);
            var copyright = document.GetFirstElementValue(ProjectXElement.Copyright);
            var netstandardVersion = document.GetFirstElementValue(ProjectXElement.NetStandardImplicitPackageVersion);
            var runtimeFrameworkVersion = document.GetFirstElementValue(ProjectXElement.RuntimeFrameworkVersion);
            var packageTargetFallbacks = document.GetFirstElementValue(ProjectXElement.PackageTargetFallback)?.Split(';') ?? new string[0];
            var runtimeIdentifiers = document.GetFirstElementValue(ProjectXElement.RuntimeIdentifiers)?.Split(';') ?? new string[0];
            var dotNetCliToolReferences = document.GetDotNetCliToolReferences();
            var assemblyOriginatorKeyFile = document.GetFirstElementValue(ProjectXElement.AssemblyOriginatorKeyFile);
            bool.TryParse(document.GetFirstElementValue(ProjectXElement.SignAssembly), out var signAssembly);
            bool.TryParse(document.GetFirstElementValue(ProjectXElement.PublicSign), out var publicSign);
            bool.TryParse(document.GetFirstElementValue(ProjectXElement.TreatWarningsAsErrors), out var treatWarningsAsErrors);
            bool.TryParse(document.GetFirstElementValue(ProjectXElement.GenerateDocumentationFile), out var generateDocumentationFile);
            bool.TryParse(document.GetFirstElementValue(ProjectXElement.PreserveCompilationContext), out var preserveCompilationContext);
            bool.TryParse(document.GetFirstElementValue(ProjectXElement.AllowUnsafeBlocks), out var allowUnsafeBlocks);
            bool.TryParse(document.GetFirstElementValue(ProjectXElement.PackageRequireLicenseAcceptance), out var packageRequireLicenseAcceptance);
            var packageTags = document.GetFirstElementValue(ProjectXElement.PackageTags)?.Split(';') ?? new string[0];
            var packageReleaseNotes = document.GetFirstElementValue(ProjectXElement.PackageReleaseNotes);
            var packageIconUrl = document.GetFirstElementValue(ProjectXElement.PackageIconUrl);
            var packageProjectUrl = document.GetFirstElementValue(ProjectXElement.PackageProjectUrl);
            var packageLicenseUrl = document.GetFirstElementValue(ProjectXElement.PackageLicenseUrl);
            var repositoryType = document.GetFirstElementValue(ProjectXElement.RepositoryType);
            var repositoryUrl = document.GetFirstElementValue(ProjectXElement.RepositoryUrl);
            bool.TryParse(document.GetFirstElementValue(ProjectXElement.ServerGarbageCollection), out var serverGarbageCollection);
            bool.TryParse(document.GetFirstElementValue(ProjectXElement.ConcurrentGarbageCollection), out var concurrentGarbageCollection);
            bool.TryParse(document.GetFirstElementValue(ProjectXElement.RetainVMGarbageCollection), out var retainVMGarbageCollection);
            var threadPoolMinThreads = document.GetFirstElementValue(ProjectXElement.ThreadPoolMinThreads);
            var threadPoolMaxThreads = document.GetFirstElementValue(ProjectXElement.ThreadPoolMaxThreads);
            var noWarn = document.GetFirstElementValue(ProjectXElement.NoWarn)?.Split(';').Where(x => !x.StartsWith("$"))?.ToArray() ?? new string[0];
            var defineConstants = document.GetFirstElementValue(ProjectXElement.DefineConstants)?.Split(';').Where(x => !x.StartsWith("$"))?.ToArray() ?? new string[0];
            var targets = document.GetTargets();

            return new CustomProjectParserResult
            {
                Configuration = config,
                Platform = platform,
                OutputType = outputType,
                OutputPath = outputPath,
                AssemblyName = assemblyName,
                TargetFrameworkProfile = targetFramework,
                ProjectReferences = projectReferences,
                IsNetCore = true,
                NetCore = new NetCoreProject
                {
                    Sdk = sdk,
                    IsWeb = sdk.EqualsIgnoreCase("Microsoft.NET.Sdk.Web"),
                    Version = version,
                    PackageId = packageId,
                    PackageReferences = packageReferences,
                    ProjectReferences = projectReferences,
                    TargetFrameworks = targetFrameworks,
                    DebugType = debugType,
                    Authors = authors,
                    Company = company,
                    NeutralLanguage = neutralLang,
                    AssemblyTitle = assemblyTitle,
                    Description = description,
                    Copyright = copyright,
                    NetStandardImplicitPackageVersion = netstandardVersion,
                    RuntimeFrameworkVersion = runtimeFrameworkVersion,
                    PackageTargetFallbacks = packageTargetFallbacks,
                    RuntimeIdentifiers = runtimeIdentifiers,
                    DotNetCliToolReferences = dotNetCliToolReferences,
                    AssemblyOriginatorKeyFile = assemblyOriginatorKeyFile,
                    SignAssembly = signAssembly,
                    PublicSign = publicSign,
                    TreatWarningsAsErrors = treatWarningsAsErrors,
                    GenerateDocumentationFile = generateDocumentationFile,
                    PreserveCompilationContext = preserveCompilationContext,
                    AllowUnsafeBlocks = allowUnsafeBlocks,
                    PackageRequireLicenseAcceptance = packageRequireLicenseAcceptance,
                    PackageTags = packageTags,
                    PackageReleaseNotes = packageReleaseNotes,
                    PackageIconUrl = packageIconUrl,
                    PackageProjectUrl = packageProjectUrl,
                    PackageLicenseUrl = packageLicenseUrl,
                    RepositoryType = repositoryType,
                    RepositoryUrl = repositoryUrl,
                    NoWarn = noWarn,
                    DefineConstants = defineConstants,
                    Targets = targets,
                    RuntimeOptions = new RuntimeOptions
                    {
                        ServerGarbageCollection = serverGarbageCollection,
                        ConcurrentGarbageCollection = concurrentGarbageCollection,
                        RetainVMGarbageCollection = retainVMGarbageCollection,
                        ThreadPoolMinThreads = threadPoolMinThreads,
                        ThreadPoolMaxThreads = threadPoolMaxThreads
                    }
                }
            };

            // TODO: Add support for file contents
            // default globs: https://docs.microsoft.com/en-us/dotnet/articles/core/tools/project-json-to-csproj#files
            /*
            <ItemGroup>
              <Compile Include="..\Shared\*.cs" Exclude="..\Shared\Not\*.cs" />
              <EmbeddedResource Include="..\Shared\*.resx" />
              <Content Include="Views\**\*" PackagePath="%(Identity)" />
              <None Include="some/path/in/project.txt" Pack="true" PackagePath="in/package.txt" />

              <None Include="notes.txt" CopyToOutputDirectory="Always" />
              <!-- CopyToOutputDirectory = { Always, PreserveNewest, Never } -->

              <Content Include="files\**\*" CopyToPublishDirectory="PreserveNewest" />
              <None Include="publishnotes.txt" CopyToPublishDirectory="Always" />
              <!-- CopyToPublishDirectory = { Always, PreserveNewest, Never } -->
            </ItemGroup>
             * */
        }

        private static NetFrameworkProjectProperties GetNetFrameworkProjectProperties(XDocument document, string config, string platform, XNamespace ns, DirectoryPath rootPath)
        {
            return (from project in document.Elements(ns + ProjectXElement.Project)
                    from propertyGroup in project.GetPropertyGroups(ns)
                    let configuration = config
                    let platformBackup = propertyGroup.GetPlatform(ns)
                    let configPropertyGroups = project.GetPropertyGroups(ns)
                        .Where(x =>
                        {
                            var xAttribute = x.Attribute("Condition");
                            return xAttribute != null && (x.Elements(ns + ProjectXElement.OutputPath).Any() &&
                                                                        xAttribute
                                                                            .Value.ToLowerInvariant()
                                                                            .Contains($"== '{config}|{platform}'".ToLowerInvariant()));
                        })
                    where !string.IsNullOrWhiteSpace(configuration)
                    select new NetFrameworkProjectProperties
                    {
                        Configuration = configuration,
                        Platform = platform,
                        ProjectGuid = propertyGroup.GetProjectGuid(ns),
                        ProjectTypeGuids = propertyGroup.GetProjectType(ns)?.Split(';'),
                        OutputType = propertyGroup.GetOutputType(ns),
                        OutputPath = configPropertyGroups.GetOutputPath(ns, rootPath),
                        RootNameSpace = propertyGroup.GetRootNamespace(ns),
                        AssemblyName = propertyGroup.GetAssemblyName(ns),
                        TargetFrameworkVersion = propertyGroup.GetTargetFrameworkVersion(ns),
                        TargetFrameworkProfile = propertyGroup.GetTargetFrameworkProfile(ns)
                    }).FirstOrDefault();
        }

        private static CustomProjectFile[] GetNetFrameworkMSBuildProjects(XDocument document, XNamespace ns, DirectoryPath rootPath)
        {
            return (from project in document.Elements(ns + ProjectXElement.Project)
                    from itemGroup in project.Elements(ns + ProjectXElement.ItemGroup)
                    from element in itemGroup.Elements()
                    where element.Name != ns + ProjectXElement.Reference &&
                          element.Name != ns + ProjectXElement.Import &&
                          element.Name != ns + ProjectXElement.BootstrapperPackage &&
                          element.Name != ns + ProjectXElement.ProjectReference &&
                          element.Name != ns + ProjectXElement.Service
                    from include in element.Attributes("Include")
                    let value = include.Value
                    where !string.IsNullOrEmpty(value)
                    let filePath = rootPath.CombineWithProjectPath(value)
                    select new CustomProjectFile
                    {
                        FilePath = filePath,
                        RelativePath = value,
                        Compile = element.Name == ns + ProjectXElement.Compile
                    }).ToArray();
        }

        private static ProjectReference[] GetNetFrameworkProjectReferences(XDocument document, XNamespace ns, DirectoryPath rootPath)
        {
            return (from project in document.Elements(ns + ProjectXElement.Project)
                    from itemGroup in project.Elements(ns + ProjectXElement.ItemGroup)
                    from element in itemGroup.Elements()
                    where element.Name == ns + ProjectXElement.ProjectReference
                    from include in element.Attributes("Include")
                    let value = include.Value
                    where !string.IsNullOrEmpty(value)
                    let filePath = rootPath.CombineWithFilePath(value)
                    let nameElement = element.Element(ns + ProjectXElement.Name)
                    let projectElement = element.Element(ns + ProjectXElement.Project)
                    let packageElement = element.Element(ns + ProjectXElement.Package)
                    select new ProjectReference
                    {
                        FilePath = filePath,
                        RelativePath = value,
                        Name = nameElement?.Value,
                        Project = projectElement?.Value,
                        Package = string.IsNullOrEmpty(packageElement?.Value)
                            ? null
                            : rootPath.CombineWithFilePath(packageElement.Value)
                    }).ToArray();
        }

        private static ProjectAssemblyReference[] GetNetFrameworkReferences(XDocument document, XNamespace ns, DirectoryPath rootPath)
        {
            return (from reference in document.Descendants(ns + ProjectXElement.Reference)
                    from include in reference.Attributes("Include")
                    let includeValue = include.Value
                    let hintPathElement = reference.Element(ns + ProjectXElement.HintPath)
                    let nameElement = reference.Element(ns + ProjectXElement.Name)
                    let fusionNameElement = reference.Element(ns + ProjectXElement.FusionName)
                    let specificVersionElement = reference.Element(ns + ProjectXElement.SpecificVersion)
                    let aliasesElement = reference.Element(ns + ProjectXElement.Aliases)
                    let privateElement = reference.Element(ns + ProjectXElement.Private)
                    select new ProjectAssemblyReference
                    {
                        Include = includeValue,
                        HintPath = string.IsNullOrEmpty(hintPathElement?.Value)
                            ? null
                            : rootPath.CombineWithFilePath(hintPathElement.Value),
                        Name = nameElement?.Value ?? includeValue?.Split(',')?.FirstOrDefault(),
                        FusionName = fusionNameElement?.Value,
                        SpecificVersion = specificVersionElement == null ? (bool?)null : bool.Parse(specificVersionElement.Value),
                        Aliases = aliasesElement?.Value,
                        Private = privateElement == null ? (bool?)null : bool.Parse(privateElement.Value)
                    }).Distinct(x => x.Name).ToArray();
        }
    }
}