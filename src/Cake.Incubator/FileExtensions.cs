// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/. 

namespace Cake.Incubator
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Cake.Common.Solution;
    using Cake.Core;
    using Cake.Core.Annotations;
    using Cake.Core.IO;

    /// <summary>
    /// Several extension methods when using File.
    /// </summary>
    [CakeAliasCategory("File Operations")]
    public static class FileExtensions
    {
        /// <summary>
        /// Filters FilePathCollection by filenames, in the order specified
        /// </summary>
        /// <param name="filePathCollection">the collection to filter</param>
        /// <param name="fileNames">the file names to filter by</param>
        /// <returns>the filtered list</returns>
        public static IEnumerable<FilePath> Filter(this FilePathCollection filePathCollection, params string[] fileNames)
        {
            return
                fileNames
                    .Select(fileName => filePathCollection.SingleOrDefault(x => x.HasFileName(fileName)))
                    .Where(match => match != null)
                    .ToList();
        }

        /// <summary>
        /// Gets the output assembly paths for solution or project files, for a specific build configuration
        /// </summary>
        /// <param name="context">the cake context</param>
        /// <param name="target">the solution or project file</param>
        /// <param name="configuration">the build configuration</param>
        /// <returns>the list of output assembly paths</returns>
        /// <exception cref="ArgumentException">Throws if the file is not a recognizable solution or project file</exception>
        /// <example>
        /// The project or solution's <see cref="FilePath"/> and the build configuration will 
        /// return the output file/s (dll or exe) for the project and return as an <see cref="IEnumerable{FilePath}"/>
        /// The alias expects a valid `.sln` or a `csproj` file.
        ///
        /// For a solution
        /// <code>
        /// // Solution output dll/exe's FilePath[] for 'Release' configuration
        /// IEnumerable&lt;FilePath&gt; filePaths = GetOutputAssemblies(new FilePath("test.sln"), "Release");
        /// </code>
        /// 
        /// For a project
        /// <code>
        /// // Project output dll/exe as FilePath[] for 'Custom' configuration
        /// IEnumerable&lt;FilePath&gt; filePaths = GetOutputAssemblies(new FilePath("test.csproj"), "Custom");
        /// </code>
        /// </example>
        [CakeMethodAlias]
        [CakeAliasCategory("Projects")]
        public static IEnumerable<FilePath> GetOutputAssemblies(this ICakeContext context, FilePath target,
            string configuration)
        {
            if (!target.IsProject() && !target.IsSolution())
                throw new ArgumentException(
                    $"Cannot get target assemblies, {target.FullPath} is not a project or solution file");

            return target.IsSolution()
                ? context.GetSolutionAssemblies(target, configuration)
                : new[] { context.GetProjectAssembly(target, configuration) };
        }

        /// <summary>
        /// Gets the output assembly paths for a solution file, for a specific build configuration
        /// </summary>
        /// <param name="context">the cake context</param>
        /// <param name="target">the solution file</param>
        /// <param name="configuration">the build configuration</param>
        /// <returns>the list of output assembly paths</returns>
        /// <exception cref="ArgumentException">Throws if the file is not a recognizable solution file</exception>
        /// <example>
        /// The Solution's <see cref="FilePath"/> and the build configuration will return the 
        /// output files (dll or exe) for the projects and return as an <see cref="IEnumerable{FilePath}"/>
        /// The alias expects a valid `.sln` file.
        /// <code>
        /// // Solution project's output dll/exe's for the 'Release' configuration
        /// IEnumerable&lt;FilePath&gt; filePaths = GetOutputAssemblies(new FilePath("test.sln"), "Release");
        /// </code>
        /// </example>
        [CakeMethodAlias]
        [CakeAliasCategory("Projects")]
        public static IEnumerable<FilePath> GetSolutionAssemblies(this ICakeContext context, FilePath target,
            string configuration)
        {
            if (!target.IsSolution())
                throw new ArgumentException(
                    $"Cannot get target assemblies, {target.FullPath} is not a solution file");

            var result = context.ParseSolution(target);
            return
                result.GetProjects()
                    .Select<SolutionProject, FilePath>(x => context.GetProjectAssembly(x.Path, configuration));
        }

        /// <summary>
        /// Gets the output assembly path for a project file, for a specific build configuration
        /// </summary>
        /// <param name="context">the cake context</param>
        /// <param name="target">the project file</param>
        /// <param name="configuration">the build configuration</param>
        /// <returns>the output assembly path</returns>
        /// <exception cref="ArgumentException">Throws if the file is not a recognizable project file</exception>
        /// <example>
        /// The project's <see cref="FilePath"/> and the build configuration will return the 
        /// output file (dll or exe) for the project and return as a <see cref="FilePath"/>
        /// The alias expects a valid project file.
        /// <code>
        /// // Project output dll/exe as FilePath[] for 'Custom' configuration
        /// IEnumerable&lt;FilePath&gt; filePaths = GetOutputAssemblies(new FilePath("test.csproj"), "Custom");
        /// </code>
        /// </example>
        [CakeMethodAlias]
        [CakeAliasCategory("Projects")]
        public static FilePath GetProjectAssembly(this ICakeContext context, FilePath target, string configuration)
        {
            if (!target.IsProject())
                throw new ArgumentException(
                    $"Cannot get target assembly, {target.FullPath} is not a project file");

            return context.ParseProject(target, configuration).GetAssemblyFilePath();
        }

        /// <summary>
        /// Obsolete: Use Cake.Common.IO.MoveFile instead
        /// </summary>
        /// <param name="context">the cake context</param>
        /// <param name="source">the source file</param>
        /// <param name="destination">the destination file path</param>
        [CakeMethodAlias]
        [CakeAliasCategory("Move")]
        [Obsolete("Use Cake.Common.IO.MoveFile instead")]
        public static void Move(this ICakeContext context, FilePath source, FilePath destination)
        {
            source.ThrowIfNull(nameof(source));
            destination.ThrowIfNull(nameof(destination));

            if (!context.FileSystem.Exist(source)) throw new CakeException($"{source.FullPath} does not exist");

            var file = context.FileSystem.GetFile(source);
            file.Move(destination);
        }

        /// <summary>
        /// Obsolete: Use Cake.Common.IO.MoveFiles instead
        /// </summary>
        /// <param name="context"></param>
        /// <param name="source"></param>
        /// <param name="destination"></param>
        [CakeMethodAlias]
        [CakeAliasCategory("Move")]
        [Obsolete("Use Cake.Common.IO.MoveFiles instead")]
        public static void Move(this ICakeContext context, IEnumerable<FilePath> source, DirectoryPath destination)
        {
            var sourceFiles = source.ThrowIfNull(nameof(source));
            destination.ThrowIfNull(nameof(destination));

            sourceFiles.Each(f =>
            {
                if (!context.FileSystem.Exist(f)) throw new CakeException($"{f.FullPath} does not exist");

                var file = context.FileSystem.GetFile(f);
                file.Move(destination.CombineWithFilePath(f.GetFilename()));
            });
        }
    }
}