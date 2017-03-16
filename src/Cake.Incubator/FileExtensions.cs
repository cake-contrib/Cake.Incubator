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
    /// Contains aliases for working with files
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
        /// Returns files in the same directory that have the same file name but different extensions
        /// </summary>
        /// <param name="context">the cake context</param>
        /// <param name="files">the files to return matches for</param>
        /// <returns>a list of matching files</returns>
        /// <example>
        /// Locates files with the same name in the same directory, but different extensions.
        /// The .pdb to your .dll as it were.
        /// <code>
        /// // /output/file.dll
        /// // /output/file.xml
        /// // /output/file.pdb
        /// // /output/another.dll
        ///
        /// IEnumerable&lt;FilePath&gt; matchingFiles = GetMatchingFiles(new FilePath("/output/file.dll"));
        ///
        /// matchingFiles[0]; // /output/file.xml
        /// matchingFiles[1]; // /output/file.pdb
        /// </code>
        /// </example>
        [CakeMethodAlias]
        [CakeAliasCategory("General")]
        public static IEnumerable<IFile> GetMatchingFiles(this ICakeContext context, IEnumerable<FilePath> files)
        {
            return files.SelectMany(
                x =>
                    context.FileSystem.GetDirectory(x.GetDirectory())
                        .GetFiles($"{x.GetFilenameWithoutExtension()}.*", SearchScope.Current)
                        .Where(f => !f.Path.Equals(x)));
        }

        /// <summary>
        /// Gets FliePaths using glob patterns
        /// </summary>
        /// <param name="context">the cake context</param>
        /// <param name="patterns">the glob patterns</param>
        /// <returns>the files matching the glob patterns</returns>
        /// <example>
        /// Locates files with the same name in the same directory, but different extensions.
        /// The .pdb to your .dll as it were.
        /// <code>
        /// // /output/file.dll
        /// // /output/file.xml
        /// // /output/file.pdb
        /// // /output/another.dll
        ///
        /// IEnumerable&lt;FilePath&gt; matchingFiles = GetMatchingFiles(new FilePath("/output/file.dll"));
        ///
        /// matchingFiles[0]; // /output/file.xml
        /// matchingFiles[1]; // /output/file.pdb
        /// </code>
        /// </example>
        [CakeMethodAlias]
        [CakeAliasCategory("General")]
        public static FilePathCollection GetFiles(this ICakeContext context, params string[] patterns)
        {
            return
                patterns.Aggregate(
                    new FilePathCollection(new PathComparer(context.Environment)),
                    (current, pattern) =>
                        current + context.Globber.GetFiles(pattern));
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
        /// The project or solution's <see cref="FilePath"/> and the build configuration will return the output file/s (dll or exe) for the project and return as an <see cref="IEnumerable{FilePath}"/>
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
        /// The Solution's <see cref="FilePath"/> and the build configuration will return the output files (dll or exe) for the projects and return as an <see cref="IEnumerable{FilePath}"/>
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
        /// The project's <see cref="FilePath"/> and the build configuration will return the output file (dll or exe) for the project and return as a <see cref="FilePath"/>
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
        /// Moves a file
        /// </summary>
        /// <param name="context">the cake context</param>
        /// <param name="source">the source file</param>
        /// <param name="destination">the destination file path</param>
        [CakeMethodAlias]
        [Obsolete("Use Cake.Common.IO.MoveFile instead")]
        public static void Move(this ICakeContext context, FilePath source, FilePath destination)
        {
            source.ThrowIfNull(nameof(source));
            destination.ThrowIfNull(nameof(destination));

            if (!context.FileSystem.Exist(source)) throw new CakeException($"{source.FullPath} does not exist");

            var file = context.FileSystem.GetFile(source);
            file.Move(destination);
        }

        [CakeMethodAlias]
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