// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/. 

namespace Cake.Extensions
{
    using System;
    using System.Globalization;
    using System.Linq;
    using System.Xml.Linq;
    using Cake.Common.Solution.Project;
    using Cake.Core;
    using Cake.Core.IO;

    /// <summary>
    /// The MSBuild project file parser.
    /// customised from original: https://github.com/cake-build/cake/blob/main/src/Cake.Common/Solution/Project/ProjectParser.cs
    /// </summary>
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

            Console.WriteLine($"Extension is {extension}");

            switch (extension)
            {                
                case ".xproj":
                    return ParseXprojFile(projectPath, config);                
                default:
                    return ParseCsProjFile(projectPath, config);
            }
        }

        public CustomProjectParserResult ParseCsProjFile(FilePath projectPath, string config)
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
             let configuration = config
             let platform = propertyGroup
                 .Elements(ProjectXElement.Platform)
                 .Select(cfg => cfg.Value)
                 .FirstOrDefault()
             let configPropertyGroups = project.Elements(ProjectXElement.PropertyGroup)
                 .Where(x => x.Elements(ProjectXElement.OutputPath).Any() && x.Attribute("Condition") != null)
                 .Where(x => x.Attribute("Condition").Value.ToLowerInvariant().Contains($"== '{config}|{platform}'".ToLowerInvariant()))
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
                     .Select(outputPath => file.Path.GetDirectory().Combine(DirectoryPath.FromString(outputPath.Value)))
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

            var rootPath = projectPath.GetDirectory();

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
            (from project in document.Elements(ProjectXElement.Project)
             from itemGroup in project.Elements(ProjectXElement.ItemGroup)
             from element in itemGroup.Elements()
             where element.Name == ProjectXElement.Reference
             from include in element.Attributes("Include")
             let includeValue = include.Value
             let hintPathElement = element.Element(ProjectXElement.HintPath)
             let nameElement = element.Element(ProjectXElement.Name)
             let fusionNameElement = element.Element(ProjectXElement.FusionName)
             let specificVersionElement = element.Element(ProjectXElement.SpecificVersion)
             let aliasesElement = element.Element(ProjectXElement.Aliases)
             let privateElement = element.Element(ProjectXElement.Private)
             select new ProjectAssemblyReference
             {
                 Include = includeValue,
                 HintPath = string.IsNullOrEmpty(hintPathElement?.Value)
                     ? null : rootPath.CombineWithFilePath(hintPathElement.Value),
                 Name = nameElement?.Value,
                 FusionName = fusionNameElement?.Value,
                 SpecificVersion = specificVersionElement == null ? (bool?)null : bool.Parse(specificVersionElement.Value),
                 Aliases = aliasesElement?.Value,
                 Private = privateElement == null ? (bool?)null : bool.Parse(privateElement.Value)
             }).ToArray();

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
                     ? null : rootPath.CombineWithFilePath(packageElement.Value)
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

        public CustomProjectParserResult ParseXprojFile(FilePath projectPath, string config)
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

    public class ProjectPath
    {
        public ProjectPath(string path)
        {
            Path = path;
        }

        public string Path { get; set; }

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

    public static class ProjectPathExtensions
    {
        public static ProjectPath CombineWithProjectPath(this DirectoryPath basePath, string path)
        {
            return new ProjectPath(System.IO.Path.Combine(basePath.FullPath, path));
        }
    }
}