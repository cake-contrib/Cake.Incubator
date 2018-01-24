// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

namespace Cake.Incubator
{
    using System;
    using System.Diagnostics;
    using System.Linq;
    using System.Text.RegularExpressions;
    using System.Xml.Linq;
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
        private static readonly Regex NetCoreTargetFrameworkRegex = new Regex("([Nn][Ee][Tt])([Cc])\\w+", RegexOptions.Compiled);
        private static readonly Regex NetStandardTargetFrameworkRegex = new Regex("([Nn][Ee][Tt])([Ss])\\w+", RegexOptions.Compiled);
        private static readonly Regex NetFrameworkTargetFrameworkRegex = new Regex("([Nn][Ee][Tt][0-9*])\\w+", RegexOptions.Compiled);

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
            return projectParserResult.OutputType.Equals("Library", StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Checks if the project is a `dotnet test` compatible project
        /// </summary>
        /// <param name="projectParserResult">the parsed project</param>
        /// <returns>true if the project is a dotnet test compatible project</returns>
        /// <example>
        /// Check if a parsed project is a dotnet test compatible project
        /// <code>
        /// CustomParseProjectResult project = ParseProject(new FilePath("test.csproj"), "Release");
        /// if (project.IsDotNetCliTestProject()) { ... }
        /// </code>
        /// </example>
        public static bool IsDotNetCliTestProject(this CustomProjectParserResult projectParserResult)
        {
            // check for package reference 'Microsoft.NET.Test.Sdk' first
            // in 2017 csproj format, it is located in package references
            // in pre 2017 format, it is located in references
            const string microsoftNetTestSdk = "Microsoft.NET.Test.Sdk";

            if (projectParserResult.IsNetCore &&
                projectParserResult.NetCore.PackageReferences.Any(
                    x => x.Name.EqualsIgnoreCase(microsoftNetTestSdk)))
                return true;

            return projectParserResult.IsNetFramework &&
                   !projectParserResult.PackageReferences.IsNullOrEmpty() &&
                   projectParserResult.PackageReferences.Any(x => x.Name.EqualsIgnoreCase(microsoftNetTestSdk));
        }

        /// <summary>
        /// Checks if the project is a pre `dotnet test` compatible project
        /// </summary>
        /// <param name="projectParserResult">the parsed project</param>
        /// <returns>true if the project is a pre `dotnet test` compatible project</returns>
        /// <example>
        /// Check if a parsed project is a pre `dotnet test` compatible project
        /// <code>
        /// CustomParseProjectResult project = ParseProject(new FilePath("test.csproj"), "Release");
        /// if (project.IsFrameworkTestProject()) { ... }
        /// </code>
        /// </example>
        public static bool IsFrameworkTestProject(this CustomProjectParserResult projectParserResult)
        {
            // test libs should target a specific platform, standard does not so isn't recommended
            if (projectParserResult.IsNetStandard) return false;

            var testFrameworkPackageNames = new[] { "Xunit", "Nunit", "Microsoft.VisualStudio.TestPlatform", "fixie", "Expecto" };
            var testFrameworkAssemblyNames = new[] { "xunit.core", "Nunit.framework", "Microsoft.VisualStudio.TestPlatform.TestFramework", "fixie", "Expecto" };
            if (projectParserResult.IsNetFramework)
            {
                // check project guid or for common test package/assembly references
                return projectParserResult.IsType(ProjectType.Test) ||
                       projectParserResult.PackageReferences.Any(x => testFrameworkPackageNames.Contains(x.Name, StringComparer.OrdinalIgnoreCase)) ||
                       projectParserResult.References.Any(x => testFrameworkAssemblyNames.Contains(x.Name, StringComparer.OrdinalIgnoreCase));
            }

            return false;
        }

        /// <summary>
        /// Checks if the project is a test compatible project
        /// </summary>
        /// <param name="projectParserResult">the parsed project</param>
        /// <returns>true if the project is a test compatible project</returns>
        /// <example>
        /// Check if a parsed project is a test compatible project
        /// <code>
        /// CustomParseProjectResult project = ParseProject(new FilePath("test.csproj"), "Release");
        /// if (project.IsTestProject()) { ... }
        /// </code>
        /// </example>
        public static bool IsTestProject(this CustomProjectParserResult projectParserResult)
        {
            return IsDotNetCliTestProject(projectParserResult) || IsFrameworkTestProject(projectParserResult);
        }

        /// <summary>
        /// Checks if the project is a web application.
        /// </summary>
        /// <param name="projectParserResult">the parsed project</param>
        /// <returns>true if the project is a web application</returns>
        /// <example>
        /// Check if a parsed project is a web application
        /// <code>
        /// CustomParseProjectResult project = ParseProject(new FilePath("test.csproj"), "Release");
        /// if (project.IsWebApplication()) { ... }
        /// </code>
        /// </example>
        public static bool IsWebApplication(this CustomProjectParserResult projectParserResult)
        {
            if (projectParserResult.IsNetFramework)
            {
                if (!projectParserResult.ProjectTypeGuids.Any())
                {
                    return false;
                }

                return projectParserResult.IsType(ProjectType.AspNetMvc1) ||
                   projectParserResult.IsType(ProjectType.AspNetMvc2) ||
                   projectParserResult.IsType(ProjectType.AspNetMvc3) ||
                   projectParserResult.IsType(ProjectType.AspNetMvc4) ||
                   projectParserResult.IsType(ProjectType.AspNetMvc5) ||
                   projectParserResult.IsType(ProjectType.WebApplication) ||
                   projectParserResult.IsType(ProjectType.WebSite);
            }

            return projectParserResult.IsNetCore && projectParserResult.NetCore.IsWeb;
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
            if (projectType.HasFlag(ProjectType.Unspecified))
            {
                return projectParserResult.ProjectTypeGuids.IsNullOrEmpty();
            }

            return !projectParserResult.ProjectTypeGuids.IsNullOrEmpty() && projectParserResult.ProjectTypeGuids.Any(x => x.EqualsIgnoreCase(SolutionParserExtensions.Types[projectType]));
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
                ? document.ParseVS2017ProjectFile(projectFile, configuration, platform)
                : document.ParsePreVS2017ProjectFile(projectFile, configuration, platform);
        }

        internal static CustomProjectParserResult ParsePreVS2017ProjectFile(this XDocument document, IFile projectFile, string config, string platform)
        {
            var rootPath = projectFile.Path.GetDirectory();

            var ns = document.Root.Name?.Namespace;
            var projectProperties = GetPreVS2017ProjectProperties(document, config, platform, ns, rootPath);

            if (projectProperties == null)
            {
                throw new CakeException("Failed to parse pre VS2017 project properties");
            }

            var projectFiles = GetNetFrameworkMSBuildProjects(document, ns, rootPath);
            var references = GetNetFrameworkReferences(document, ns, rootPath);
            var projectReferences = GetNetFrameworkProjectReferences(document, ns, rootPath);
            var packageReferences = document.GetPackageReferences();

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
                TargetFrameworkVersion = projectProperties.TargetFrameworkVersions.First(),
                TargetFrameworkVersions = projectProperties.TargetFrameworkVersions,
                TargetFrameworkProfile = projectProperties.TargetFrameworkProfile,
                Files = projectFiles,
                References = references,
                ProjectReferences = projectReferences,
                PackageReferences = packageReferences,
                IsNetFramework = true
            };
        }

        internal static CustomProjectParserResult ParseVS2017ProjectFile(this XDocument document, IFile projectFile, string config, string platform)
        {
            var sdk = document.GetSdk();
            var version = document.GetVersion();
            var projectName = projectFile.Path.GetFilenameWithoutExtension().ToString();
            var rootnamespace = document.GetFirstElementValue(ProjectXElement.RootNamespace) ?? projectName;
            var targetFramework = document.GetFirstElementValue(ProjectXElement.TargetFramework);
            var targetFrameworks =
                document.GetFirstElementValue(ProjectXElement.TargetFrameworks)?.SplitIgnoreEmpty(';') ??
                (targetFramework != null ? new[] { targetFramework } : new string[0]);
            var applicationIcon = document.GetFirstElementValue(ProjectXElement.ApplicationIcon);
            var assemblyVersion = document.GetFirstElementValue(ProjectXElement.AssemblyVersion) ?? "1.0.0.0";
            var fileVersion = document.GetFirstElementValue(ProjectXElement.FileVersion) ?? "1.0.0.0";
            var outputType = document.GetFirstElementValue(ProjectXElement.OutputType) ?? "Library";
            var debugType = document.GetFirstElementValue(ProjectXElement.DebugType);
            var product = document.GetFirstElementValue(ProjectXElement.Product);
            var documentationFile = document.GetFirstElementValue(ProjectXElement.DocumentationFile, config, platform);
            var defaultOutputPath = projectFile.GetDefaultOutputPath(config, platform, targetFramework);
            var outputPath = document.GetOutputPath(config, platform) ?? defaultOutputPath;
            var packageReferences = document.GetPackageReferences();
            var projectReferences = document.GetProjectReferences(projectFile.Path.GetDirectory());
            var assemblyName = document.GetFirstElementValue(ProjectXElement.AssemblyName) ?? projectName;
            var packageId = document.GetFirstElementValue(ProjectXElement.PackageId) ?? assemblyName;
            var authors = document.GetFirstElementValue(ProjectXElement.Authors)?.SplitIgnoreEmpty(';');
            var company = document.GetFirstElementValue(ProjectXElement.Company);
            var neutralLang = document.GetFirstElementValue(ProjectXElement.NeutralLanguage);
            var assemblyTitle = document.GetFirstElementValue(ProjectXElement.AssemblyTitle);
            var description = document.GetFirstElementValue(ProjectXElement.Description);
            var copyright = document.GetFirstElementValue(ProjectXElement.Copyright);
            var buildOutputTargetFolder = document.GetFirstElementValue(ProjectXElement.BuildOutputTargetFolder);
            var contentTargetFolders = document.GetFirstElementValue(ProjectXElement.ContentTargetFolders)?.SplitIgnoreEmpty(';');
            var netstandardVersion = document.GetFirstElementValue(ProjectXElement.NetStandardImplicitPackageVersion);
            var runtimeFrameworkVersion = document.GetFirstElementValue(ProjectXElement.RuntimeFrameworkVersion);
            var packageTargetFallbacks = document.GetFirstElementValue(ProjectXElement.PackageTargetFallback)?.SplitIgnoreEmpty(';');
            var runtimeIdentifiers = document.GetFirstElementValue(ProjectXElement.RuntimeIdentifiers)?.SplitIgnoreEmpty(';');
            var dotNetCliToolReferences = document.GetDotNetCliToolReferences();
            var assemblyOriginatorKeyFile = document.GetFirstElementValue(ProjectXElement.AssemblyOriginatorKeyFile);
            bool.TryParse(document.GetFirstElementValue(ProjectXElement.GeneratePackageOnBuild), out var generatePackageOnBuild);
            bool.TryParse(document.GetFirstElementValue(ProjectXElement.SignAssembly), out var signAssembly);
            bool.TryParse(document.GetFirstElementValue(ProjectXElement.DelaySign), out var delaySign);
            bool.TryParse(document.GetFirstElementValue(ProjectXElement.DebugSymbols, config, platform), out var debugSymbols);
            bool.TryParse(document.GetFirstElementValue(ProjectXElement.Optimize, config, platform), out var optimize);
            bool.TryParse(document.GetFirstElementValue(ProjectXElement.IncludeBuildOutput), out var includeBuildOutput);
            bool.TryParse(document.GetFirstElementValue(ProjectXElement.IncludeContentInPack), out var includeContentInPack);
            bool.TryParse(document.GetFirstElementValue(ProjectXElement.IncludeSource), out var includeSource);
            bool.TryParse(document.GetFirstElementValue(ProjectXElement.IncludeSymbols), out var includeSymbols);
            if (!bool.TryParse(document.GetFirstElementValue(ProjectXElement.IsPackable), out var isPackable))
            {
                isPackable = true;
            }
            bool.TryParse(document.GetFirstElementValue(ProjectXElement.IsTool), out var isTool);
            bool.TryParse(document.GetFirstElementValue(ProjectXElement.NoPackageAnalysis), out var noPackageAnalysis);
            bool.TryParse(document.GetFirstElementValue(ProjectXElement.PublicSign), out var publicSign);
            bool.TryParse(document.GetFirstElementValue(ProjectXElement.TreatWarningsAsErrors, config, platform), out var treatWarningsAsErrors);
            bool.TryParse(document.GetFirstElementValue(ProjectXElement.GenerateDocumentationFile), out var generateDocumentationFile);
            bool.TryParse(document.GetFirstElementValue(ProjectXElement.PreserveCompilationContext), out var preserveCompilationContext);
            bool.TryParse(document.GetFirstElementValue(ProjectXElement.AllowUnsafeBlocks, config, platform), out var allowUnsafeBlocks);
            bool.TryParse(document.GetFirstElementValue(ProjectXElement.PackageRequireLicenseAcceptance), out var packageRequireLicenseAcceptance);
            var packageTags = document.GetFirstElementValue(ProjectXElement.PackageTags)?.SplitIgnoreEmpty(';');
            var packageReleaseNotes = document.GetFirstElementValue(ProjectXElement.PackageReleaseNotes);
            var minClientVersion = document.GetFirstElementValue(ProjectXElement.MinClientVersion);
            var langVersion = document.GetFirstElementValue(ProjectXElement.LangVersion, config, platform);
            var nuspecBasePath = document.GetFirstElementValue(ProjectXElement.NuspecBasePath);
            var nuspecFile = document.GetFirstElementValue(ProjectXElement.NuspecFile);
            var nuspecProps = document.GetNuspecProps();
            var packageIconUrl = document.GetFirstElementValue(ProjectXElement.PackageIconUrl);
            var packageProjectUrl = document.GetFirstElementValue(ProjectXElement.PackageProjectUrl);
            var packageLicenseUrl = document.GetFirstElementValue(ProjectXElement.PackageLicenseUrl);
            var packageOutputPath = document.GetFirstElementValue(ProjectXElement.PackageOutputPath);
            var title = document.GetFirstElementValue(ProjectXElement.Title);
            var generateSerializationAssemblies = document.GetFirstElementValue(ProjectXElement.GenerateSerializationAssemblies, config, platform);
            var warningLevel = document.GetFirstElementValue(ProjectXElement.WarningLevel, config, platform);
            var repositoryType = document.GetFirstElementValue(ProjectXElement.RepositoryType) ?? "git";
            var repositoryUrl = document.GetFirstElementValue(ProjectXElement.RepositoryUrl);
            bool.TryParse(document.GetFirstElementValue(ProjectXElement.ServerGarbageCollection), out var serverGarbageCollection);
            bool.TryParse(document.GetFirstElementValue(ProjectXElement.ConcurrentGarbageCollection), out var concurrentGarbageCollection);
            bool.TryParse(document.GetFirstElementValue(ProjectXElement.RetainVMGarbageCollection), out var retainVMGarbageCollection);
            int.TryParse(document.GetFirstElementValue(ProjectXElement.ThreadPoolMinThreads), out var threadPoolMinThreads);
            int.TryParse(document.GetFirstElementValue(ProjectXElement.ThreadPoolMaxThreads), out var threadPoolMaxThreads);
            var noWarn = document.GetFirstElementValue(ProjectXElement.NoWarn, config, platform)?.SplitIgnoreEmpty(';').Where(x => !x.StartsWith("$"))?.ToArray() ?? new string[0];
            var treatSpecificWarningsAsErrors = document.GetFirstElementValue(ProjectXElement.TreatSpecificWarningsAsErrors, config, platform)?.SplitIgnoreEmpty(';') ?? new string[0];
            var defineConstants = document.GetFirstElementValue(ProjectXElement.DefineConstants, config, platform)?.SplitIgnoreEmpty(';').Where(x => !x.StartsWith("$"))?.ToArray() ?? new string[0];
            var targets = document.GetTargets();
            var isNetCore = targetFrameworks.Any(x => NetCoreTargetFrameworkRegex.IsMatch(x));
            var isNetStandard = targetFrameworks.Any(x => NetStandardTargetFrameworkRegex.IsMatch(x));
            var isNetFramework = targetFrameworks.Any(x => NetFrameworkTargetFrameworkRegex.IsMatch(x));

            return new CustomProjectParserResult
            {
                AssemblyName = assemblyName,
                Configuration = config,
                OutputType = outputType,
                OutputPath = outputPath,
                Platform = platform,
                ProjectReferences = projectReferences,
                RootNameSpace = rootnamespace,
                TargetFrameworkVersion = targetFramework,
                TargetFrameworkVersions = targetFrameworks,
                IsNetCore = isNetCore,
                IsNetStandard = isNetStandard,
                IsNetFramework = isNetFramework,
                PackageReferences = packageReferences,
                NetCore = new NetCoreProject
                {
                    AllowUnsafeBlocks = allowUnsafeBlocks,
                    ApplicationIcon = applicationIcon,
                    AssemblyOriginatorKeyFile = assemblyOriginatorKeyFile,
                    AssemblyTitle = assemblyTitle ?? assemblyName,
                    AssemblyVersion = assemblyVersion,
                    Authors = authors,
                    BuildOutputTargetFolder = buildOutputTargetFolder,
                    Company = company,
                    ContentTargetFolders = contentTargetFolders,
                    Copyright = copyright,
                    DebugSymbols = debugSymbols,
                    DebugType = debugType,
                    DefineConstants = defineConstants,
                    DelaySign = delaySign,
                    Description = description,
                    DocumentationFile = documentationFile,
                    DotNetCliToolReferences = dotNetCliToolReferences,
                    FileVersion = fileVersion,
                    GenerateDocumentationFile = generateDocumentationFile,
                    GeneratePackageOnBuild = generatePackageOnBuild,
                    GenerateSerializationAssemblies = generateSerializationAssemblies,
                    IncludeBuildOutput = includeBuildOutput,
                    IncludeContentInPack = includeContentInPack,
                    IncludeSource = includeSource,
                    IncludeSymbols = includeSymbols,
                    IsPackable = isPackable,
                    IsTool = isTool,
                    IsWeb = sdk.EqualsIgnoreCase("Microsoft.NET.Sdk.Web"),
                    LangVersion = langVersion,
                    MinClientVersion = minClientVersion,
                    NetStandardImplicitPackageVersion = netstandardVersion,
                    NeutralLanguage = neutralLang,
                    NoPackageAnalysis = noPackageAnalysis,
                    NoWarn = noWarn,
                    NuspecBasePath = nuspecBasePath,
                    NuspecFile = nuspecFile,
                    NuspecProperties = nuspecProps,
                    Optimize = optimize,
                    PackageIconUrl = packageIconUrl,
                    PackageId = packageId,
                    PackageLicenseUrl = packageLicenseUrl,
                    PackageOutputPath = packageOutputPath,
                    PackageProjectUrl = packageProjectUrl,
                    PackageReferences = packageReferences,
                    PackageReleaseNotes = packageReleaseNotes,
                    PackageRequireLicenseAcceptance = packageRequireLicenseAcceptance,
                    PackageTags = packageTags,
                    PackageTargetFallbacks = packageTargetFallbacks,
                    PreserveCompilationContext = preserveCompilationContext,
                    Product = product,
                    ProjectReferences = projectReferences,
                    PublicSign = publicSign,
                    RepositoryType = repositoryType,
                    RepositoryUrl = repositoryUrl,
                    RuntimeFrameworkVersion = runtimeFrameworkVersion,
                    RuntimeIdentifiers = runtimeIdentifiers,
                    Sdk = sdk,
                    SignAssembly = signAssembly,
                    Title = title,
                    TargetFrameworks = targetFrameworks,
                    Targets = targets,
                    TreatWarningsAsErrors = treatWarningsAsErrors,
                    TreatSpecificWarningsAsErrors = treatSpecificWarningsAsErrors,
                    RuntimeOptions = new RuntimeOptions
                    {
                        ServerGarbageCollection = serverGarbageCollection,
                        ConcurrentGarbageCollection = concurrentGarbageCollection,
                        RetainVMGarbageCollection = retainVMGarbageCollection,
                        ThreadPoolMinThreads = threadPoolMinThreads,
                        ThreadPoolMaxThreads = threadPoolMaxThreads
                    },
                    Version = version,
                    WarningLevel = warningLevel
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

        private static DirectoryPath GetDefaultOutputPath(this IFile projectFile, string config, string platform, string targetFramework)
        {
            var template = "bin/";
            if (!platform.IsNullOrEmpty() && !platform.EqualsIgnoreCase("AnyCPU")) template += $"{platform}/";
            if (!config.IsNullOrEmpty()) template += $"{config}/";
            if (!targetFramework.IsNullOrEmpty()) template += $"{targetFramework}/";

            return projectFile.Path.GetDirectory()?.Combine(template);
        }

        private static NetFrameworkProjectProperties GetPreVS2017ProjectProperties(XDocument document, string config, string platform, XNamespace ns, DirectoryPath rootPath)
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
                        TargetFrameworkVersions = propertyGroup.GetTargetFrameworkVersions(ns)?.SplitIgnoreEmpty(';') ?? new[] { propertyGroup.GetTargetFrameworkVersion(ns) },
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
                            : hintPathElement.GetAbsolutePath(rootPath),
                        Name = nameElement?.Value ?? includeValue?.Split(',')?.FirstOrDefault(),
                        FusionName = fusionNameElement?.Value,
                        SpecificVersion = specificVersionElement == null ? (bool?)null : bool.Parse(specificVersionElement.Value),
                        Aliases = aliasesElement?.Value,
                        Private = privateElement == null ? (bool?)null : bool.Parse(privateElement.Value)
                    }).Distinct(x => x.Name).ToArray();
        }

        private static FilePath GetAbsolutePath(this XElement hintPathElement, DirectoryPath rootPath)
        {

            var hintPath = new FilePath(hintPathElement.Value).IsRelative;
            Cake.Core.IO.FilePath absolutePath;
            if (hintPath)
            {
                absolutePath = rootPath.CombineWithFilePath(hintPathElement.Value);
            }
            else
            {
                absolutePath = hintPathElement.Value;
                Debug.WriteLine($"An absolute path {absolutePath} was used in a project reference. It is recommended that projects contain only relative paths for references");
            }

            return absolutePath;
        }
    }
}