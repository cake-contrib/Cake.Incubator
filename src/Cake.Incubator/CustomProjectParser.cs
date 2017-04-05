// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/. 

namespace Cake.Incubator
{
    using System;
    using System.Globalization;
    using System.Linq;
    using System.Xml.Linq;
    using Cake.Common.Solution.Project;
    using Cake.Core;
    using Cake.Core.Annotations;
    using Cake.Core.IO;

    /// <summary>
    /// Class to handle parsing of Visual Studio Project Files.
    /// </summary>
    [CakeAliasCategory("MSBuild Resource")]
    public class CustomProjectParser
    {
        private readonly IFileSystem _fileSystem;
        private readonly ICakeEnvironment _environment;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProjectParser"/> class.
        /// </summary>
        /// <param name="fileSystem">The file system.</param>
        /// <param name="environment">The environment.</param>
        public CustomProjectParser(IFileSystem fileSystem, ICakeEnvironment environment)
        {
            _fileSystem = fileSystem.ThrowIfNull(nameof(fileSystem));
            _environment = environment.ThrowIfNull(nameof(environment));
        }

        /// <summary>
        /// Parses a project file for a specific configuration.
        /// </summary>
        /// <param name="projectPath">The project path.</param>
        /// <param name="config">The project configuration</param>
        /// <returns>The parsed project.</returns>
        public CustomProjectParserResult Parse(FilePath projectPath, string config)
        {
            projectPath.ThrowIfNull(nameof(projectPath));
            if (projectPath.IsRelative)
            {
                projectPath = projectPath.MakeAbsolute(_environment);
            }

            var extension = projectPath.GetExtension().ToLower();

            switch (extension)
            {                
                case ".xproj":
#pragma warning disable CS0618 // Type or member is obsolete
                    return ParseXprojFile(projectPath, config);                
#pragma warning restore CS0618 // Type or member is obsolete
                default:
                    return ParseCsProjFile(projectPath, config);
            }
        }

        private CustomProjectParserResult ParseCsProjFile(FilePath projectPath, string config)
        {
            // Get the project file.
            var file = _fileSystem.GetFile(projectPath);
            if (!file.Exists)
            {
                const string format = "Project file '{0}' does not exist.";
                var message = string.Format(CultureInfo.InvariantCulture, format, projectPath.FullPath);
                throw new CakeException(message);
            }

            if (!file.Path.HasExtension)
            {
                throw new CakeException("Project file type could not be determined by extension.");
            }

            XDocument document;
            using (var stream = file.OpenRead())
            {
                document = XDocument.Load(stream);
            }

            // netcore?
            return document.IsDotNetSdk()
                ? ParseNetcoreProjectFile(config, document, file)
                : ParseNetFramework(config, document, file);

        }

        private static CustomProjectParserResult ParseNetFramework(string config, XDocument document, IFile projFile)
        {
            var rootPath = projFile.Path.GetDirectory();

            var projectProperties =
            (from project in document.Elements(ProjectXElement.Project)
                from propertyGroup in project.Elements(ProjectXElement.PropertyGroup)
                let configuration = config
                let platform = propertyGroup
                    .Elements(ProjectXElement.Platform)
                    .Select(cfg => cfg.Value)
                    .FirstOrDefault()
                let configPropertyGroups = project.Elements(ProjectXElement.PropertyGroup)
                    .Where(x => x.Elements(ProjectXElement.OutputPath).Any() && x.Attribute("Condition") != null)
                    .Where(
                        x =>
                            x.Attribute("Condition")
                                .Value.ToLowerInvariant()
                                .Contains($"== '{config}|{platform}'".ToLowerInvariant()))
                //'$(Configuration)|$(Platform)' == 'Debug|AnyCPU' "
                where !string.IsNullOrWhiteSpace(configuration)
                select new
                {
                    Configuration = configuration,
                    Platform = platform,
                    ProjectGuid = propertyGroup
                        .Elements(ProjectXElement.ProjectGuid)
                        .Select(projectGuid => projectGuid.Value)
                        .FirstOrDefault(),
                    ProjectTypeGuid = propertyGroup
                        .Elements(ProjectXElement.ProjectTypeGuids)
                        .Select(projectTypeGuid => projectTypeGuid.Value)
                        .FirstOrDefault(),
                    OutputType = propertyGroup
                        .Elements(ProjectXElement.OutputType)
                        .Select(outputType => outputType.Value)
                        .FirstOrDefault(),
                    OutputPath = configPropertyGroups
                        .Elements(ProjectXElement.OutputPath)
                        .Select(outputPath => rootPath.Combine(DirectoryPath.FromString(outputPath.Value)))
                        .FirstOrDefault(),
                    RootNameSpace = propertyGroup
                        .Elements(ProjectXElement.RootNamespace)
                        .Select(rootNameSpace => rootNameSpace.Value)
                        .FirstOrDefault(),
                    AssemblyName = propertyGroup
                        .Elements(ProjectXElement.AssemblyName)
                        .Select(assemblyName => assemblyName.Value)
                        .FirstOrDefault(),
                    TargetFrameworkVersion = propertyGroup
                        .Elements(ProjectXElement.TargetFrameworkVersion)
                        .Select(targetFrameworkVersion => targetFrameworkVersion.Value)
                        .FirstOrDefault(),
                    TargetFrameworkProfile = propertyGroup
                        .Elements(ProjectXElement.TargetFrameworkProfile)
                        .Select(targetFrameworkProfile => targetFrameworkProfile.Value)
                        .FirstOrDefault()
                }).FirstOrDefault();

            if (projectProperties == null)
            {
                throw new CakeException("Failed to parse project properties");
            }

            var projectFiles =
            (from project in document.Elements(ProjectXElement.Project)
                from itemGroup in project.Elements(ProjectXElement.ItemGroup)
                from element in itemGroup.Elements()
                where element.Name != ProjectXElement.Reference &&
                      element.Name != ProjectXElement.Import &&
                      element.Name != ProjectXElement.BootstrapperPackage &&
                      element.Name != ProjectXElement.ProjectReference &&
                      element.Name != ProjectXElement.Service
                from include in element.Attributes("Include")
                let value = include.Value
                where !string.IsNullOrEmpty(value)
                let filePath = rootPath.CombineWithProjectPath(value)
                select new CustomProjectFile
                {
                    FilePath = filePath,
                    RelativePath = value,
                    Compile = element.Name == ProjectXElement.Compile
                }).ToArray();

            var references = 
            (from reference in document.Descendants(ProjectXElement.Reference)
                from include in reference.Attributes("Include")
                let includeValue = include.Value
                let hintPathElement = reference.Element(ProjectXElement.HintPath)
                let nameElement = reference.Element(ProjectXElement.Name)
                let fusionNameElement = reference.Element(ProjectXElement.FusionName)
                let specificVersionElement = reference.Element(ProjectXElement.SpecificVersion)
                let aliasesElement = reference.Element(ProjectXElement.Aliases)
                let privateElement = reference.Element(ProjectXElement.Private)
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

            var projectReferences =
            (from project in document.Elements(ProjectXElement.Project)
                from itemGroup in project.Elements(ProjectXElement.ItemGroup)
                from element in itemGroup.Elements()
                where element.Name == ProjectXElement.ProjectReference
                from include in element.Attributes("Include")
                let value = include.Value
                where !string.IsNullOrEmpty(value)
                let filePath = rootPath.CombineWithFilePath(value)
                let nameElement = element.Element(ProjectXElement.Name)
                let projectElement = element.Element(ProjectXElement.Project)
                let packageElement = element.Element(ProjectXElement.Package)
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

            return new CustomProjectParserResult(
                projectProperties.Configuration,
                projectProperties.Platform,
                projectProperties.ProjectGuid,
                projectProperties.ProjectTypeGuid?.Split(';'),
                projectProperties.OutputType,
                projectProperties.OutputPath,
                projectProperties.RootNameSpace,
                projectProperties.AssemblyName,
                projectProperties.TargetFrameworkVersion,
                projectProperties.TargetFrameworkProfile,
                projectFiles,
                references,
                projectReferences);
        }

        private static CustomProjectParserResult ParseNetcoreProjectFile(string config, XDocument document, IFile projFile)
        {
            var targetFramework = document.Descendants(ProjectXElement.TargetFramework).FirstOrDefault()?.Value;
            var outputType = document.Descendants("OutputType").FirstOrDefault()?.Value ?? "Library";
            var defaultOutputPath = projFile.Path.GetDirectory().Combine($"bin/{config}/{targetFramework}/");
            var outputPath = document.GetOutputPath(config) ?? defaultOutputPath;

                var assemblyName = $"{projFile.Path.GetFilenameWithoutExtension()}";

            return new CustomProjectParserResult(config, "AnyCPU", null, null, outputType, outputPath, null,
                assemblyName, targetFramework, null, null, null, null);
        }

        [Obsolete("XProj format has been deprecated")]
        private CustomProjectParserResult ParseXprojFile(FilePath projectPath, string config)
        {
            // Get the project file.
            var file = _fileSystem.GetFile(projectPath);
            if (!file.Exists)
            {
                const string format = "Project file '{0}' does not exist.";
                var message = string.Format(CultureInfo.InvariantCulture, format, projectPath.FullPath);
                throw new CakeException(message);
            }

            if (!file.Path.HasExtension)
            {
                throw new CakeException("Project file type could not be determined by extension.");
            }

            XDocument document;
            using (var stream = file.OpenRead())
            {
                document = XDocument.Load(stream);
            }

            var projectProperties = 
                (from project in document.Elements(ProjectXElement.Project)
                 from propertyGroup in project.Elements(ProjectXElement.PropertyGroup)
                 where propertyGroup.Attribute("Label") != null && 
                       propertyGroup.Attribute("Label").Value.Equals("Globals", StringComparison.InvariantCultureIgnoreCase)  
                 select new
                 {
                     ProjectGuid = propertyGroup
                        .Elements(ProjectXElement.ProjectGuid)
                        .Select(projectGuid => projectGuid.Value)
                        .FirstOrDefault(),
                     RootNameSpace = propertyGroup
                        .Elements(ProjectXElement.RootNamespace)
                        .Select(rootNameSpace => rootNameSpace.Value)
                        .FirstOrDefault(),
                     OutputPath = propertyGroup
                        .Elements(ProjectXElement.OutputPath)
                        .Select(outputPath => file.Path.GetDirectory().Combine(DirectoryPath.FromString(outputPath.Value)))
                        .FirstOrDefault(),
                     TargetFrameworkVersion = propertyGroup
                        .Elements(ProjectXElement.TargetFrameworkVersion)
                        .Select(targetFrameworkVersion => targetFrameworkVersion.Value)
                        .FirstOrDefault(),
                 }).FirstOrDefault();

            return new CustomProjectParserResult(
                config,
                null,                                               // platform
                projectProperties.ProjectGuid,
                null,                                               // projectTypeGuids
                null,                                               // outputType
                projectProperties.OutputPath,                       // outputPath
                projectProperties.RootNameSpace,                    // root namespace
                file.Path.GetFilenameWithoutExtension().ToString(), // assemblyName
                projectProperties.TargetFrameworkVersion,
                null,                                               // targetFrameworkProfile
                null,                                               // files
                null,                                               // references
                null                                                // projectReferences
            );
        }
    }

    /// <summary>
    /// Class which describes the Path to a Visual Studio Project.
    /// </summary>
    public class ProjectPath
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProjectPath"/> class.
        /// </summary>
        /// <param name="path">The path to the project file.</param>
        public ProjectPath(string path)
        {
            Path = path;
        }

        /// <summary>
        /// Gets or sets the path to the Project file.
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// Gets a value indicating whether the Project Path is to an actual file.
        /// </summary>
        public bool IsFile => Path.Contains("*");
    }

    /// <summary>Represents a MSBuild project file.</summary>
    public sealed class CustomProjectFile
    {
        /// <summary>Gets or sets the project file path.</summary>
        /// <value>The project file path.</value>
        public ProjectPath FilePath { get; set; }

        /// <summary>Gets or sets the relative path to the project file.</summary>
        /// <value>The relative path to the project file.</value>
        public string RelativePath { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="T:Cake.Common.Solution.Project.ProjectFile" /> is compiled.
        /// </summary>
        /// <value>
        ///   <c>true</c> if compiled; otherwise, <c>false</c>.
        /// </value>
        public bool Compile { get; set; }
    }

    /// <summary>
    /// Several extension methods when using ProjectPath.
    /// </summary>
    public static class ProjectPathExtensions
    {
        /// <summary>
        /// Combines a base project path with the name of the project file.
        /// </summary>
        /// <param name="basePath">The base path to the location of the Project File.</param>
        /// <param name="path">The path to the actual Project file.</param>
        /// <returns></returns>
        public static ProjectPath CombineWithProjectPath(this DirectoryPath basePath, string path)
        {
            return new ProjectPath(System.IO.Path.Combine(basePath.FullPath, path));
        }
    }
}