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
    /// Several extension methods when using ProjectParser.
    /// </summary>
    [CakeAliasCategory("MSBuild Resource")]
    public static class ProjectParserExtensions
    {
        /// <summary>
        /// Checks if the project is a library
        /// </summary>
        /// <param name="projectParserResult">the parsed project</param>
        /// <returns>true if the project is a library</returns>
        public static bool IsLibrary(this CustomProjectParserResult projectParserResult)
        {
            return projectParserResult.OutputType.Equals("Library", StringComparison.InvariantCultureIgnoreCase);
        }

        /// <summary>
        /// Returns the parsed projects output assembly extension
        /// </summary>
        /// <param name="projectParserResult">the parsed project</param>
        /// <returns>the output assembly's file extension</returns>
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
        /// <param name="projectType">the project type to check</param>
        /// <returns>true if the project type matches</returns>
        public static bool IsType(this CustomProjectParserResult projectParserResult, ProjectType projectType)
        {
            if (projectType.HasFlag(ProjectType.Unspecified))
                return projectParserResult.ProjectTypeGuids == null
                       || projectParserResult.ProjectTypeGuids.Length == 0;

            return projectParserResult.ProjectTypeGuids.Any(x => x.EqualsIgnoreCase(SolutionParserExtensions.Types[projectType]));
        }

        /// <summary>
        /// Parses a csproj file into a strongly typed <see cref="CustomProjectParserResult"/> object
        /// </summary>
        /// <param name="context">the cake context</param>
        /// <param name="project">the project filepath</param>
        /// <param name="configuration">the build configuration</param>
        /// <returns>The parsed project</returns>
        [CakeMethodAlias]
        public static CustomProjectParserResult ParseProject(this ICakeContext context, FilePath project, string configuration)
        {
            project.ThrowIfNull(nameof(project));
            configuration.ThrowIfNullOrEmpty(nameof(configuration));

            if (project.IsRelative)
            {
                project = project.MakeAbsolute(context.Environment);
            }

            var projectFile = context.FileSystem.GetProjectFile(project);
            var result = projectFile.ParseProject(configuration);

            context.Debug("Parsed project file {0}\r\n{1}", project, result.Dump());
            return result;
        }

        /// <summary>
        /// Parses a csproj file into a strongly typed <see cref="CustomProjectParserResult"/> object
        /// </summary>
        /// <returns>The parsed project</returns>
        /// <param name="projectFile">the project file</param>
        /// <param name="configuration">the build configuration</param>
        /// <returns>The parsed project</returns>
        public static CustomProjectParserResult ParseProject(this IFile projectFile, string configuration)
        {
            projectFile.ThrowIfNull(nameof(projectFile));
            configuration.ThrowIfNullOrEmpty(nameof(configuration));

            var document = projectFile.LoadXml();

            return document.IsDotNetSdk()
                ? document.ParseNetcoreProjectFile(projectFile, configuration)
                : document.ParseNetFramework(projectFile, configuration);
        }

        internal static CustomProjectParserResult ParseNetFramework(this XDocument document, IFile projectFile, string config)
        {
            var rootPath = projectFile.Path.GetDirectory();

            var ns = document.Root.Name.Namespace;
            var projectProperties = GetNetFrameworkProjectProperties(document, config, ns, rootPath);

            if (projectProperties == null)
            {
                throw new CakeException("Failed to parse .net framework project properties");
            }

            var projectFiles = GetNetFrameworkMSBuildProjects(document, ns, rootPath);
            var references = GetNetFrameworkReferences(document, ns, rootPath);
            var projectReferences = GetNetFrameworkProjectReferences(document, ns, rootPath);

            return new CustomProjectParserResult(
                projectProperties.Configuration,
                projectProperties.Platform,
                projectProperties.ProjectGuid,
                projectProperties.ProjectTypeGuids,
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

        internal static CustomProjectParserResult ParseNetcoreProjectFile(this XDocument document, IFile projFile, string config)
        {
            var targetFramework = document.Descendants(ProjectXElement.TargetFramework).FirstOrDefault()?.Value;
            var outputType = document.Descendants(ProjectXElement.OutputType).FirstOrDefault()?.Value ?? "Library";
            var defaultOutputPath = projFile.Path.GetDirectory().Combine($"bin/{config}/{targetFramework}/");
            var outputPath = document.GetOutputPath(config) ?? defaultOutputPath;

            var assemblyName = $"{projFile.Path.GetFilenameWithoutExtension()}";

            return new CustomProjectParserResult(config, "AnyCPU", null, null, outputType, outputPath, null,
                assemblyName, targetFramework, null, null, null, null);
        }

        private static NetFrameworkProjectProperties GetNetFrameworkProjectProperties(XDocument document, string config, XNamespace ns, DirectoryPath rootPath)
        {
            return (from project in document.Elements(ns + ProjectXElement.Project)
                from propertyGroup in project.Elements(ns + ProjectXElement.PropertyGroup)
                let configuration = config
                let platform = propertyGroup
                    .Elements(ns + ProjectXElement.Platform)
                    .Select(cfg => cfg.Value)
                    .FirstOrDefault()
                let configPropertyGroups = project.Elements(ns + ProjectXElement.PropertyGroup)
                    .Where(x => x.Elements(ns + ProjectXElement.OutputPath).Any() && x.Attribute("Condition") != null)
                    .Where(
                        x =>
                            x.Attribute("Condition")
                                .Value.ToLowerInvariant()
                                .Contains($"== '{config}|{platform}'".ToLowerInvariant()))
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