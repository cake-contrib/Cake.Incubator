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

    [CakeAliasCategory("FilePath Helpers")]
    public static class FilePathExtensions
    {
        /// <summary>
        /// Checks if the FilePath is a solution file
        /// </summary>
        /// <param name="filePath">the path to check</param>
        /// <returns>true if sln file</returns>
        public static bool IsSolution(this FilePath filePath)
        {
            return filePath.HasExtension && filePath.GetExtension().EqualsIgnoreCase(".sln");
        }

        /// <summary>
        /// Checks if the FilePath is a csproj file
        /// </summary>
        /// <param name="filePath">the path to check</param>
        /// <returns>true if csproj or fsproj file</returns>
        public static bool IsProject(this FilePath filePath)
        {
            return filePath.HasExtension && 
                    (filePath.GetExtension().EqualsIgnoreCase(".csproj") || filePath.GetExtension().EqualsIgnoreCase(".fsproj"));
        }

        /// <summary>
        /// Checks if the path has a specific filename and extension
        /// </summary>
        /// <param name="path">the path to check</param>
        /// <param name="fileName">the file name and extension</param>
        /// <returns>true if filename and extension matches</returns>
        public static bool HasFileName(this FilePath path, string fileName)
        {
            fileName.ThrowIfNullOrEmpty(nameof(fileName));

            return
                path.GetFilenameWithoutExtension()
                    .ToString()
                    .EqualsIgnoreCase(System.IO.Path.GetFileNameWithoutExtension(fileName));
        }

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
        /// Returns files in the same directory that have the same name but different extensions
        /// </summary>
        /// <param name="context">the cake context</param>
        /// <param name="files">the files to return matches for</param>
        /// <returns>a list of matching files</returns>
        [CakeMethodAlias]
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
        [CakeMethodAlias]
        public static FilePathCollection GetFiles(this ICakeContext context, params string[] patterns)
        {
            return
                patterns.Aggregate(
                    new FilePathCollection(new PathComparer(context.Environment)),
                    (current, pattern) =>
                            current + context.Globber.GetFiles(pattern));
        }

        /// <summary>
        /// Gets the output assembly paths for solution or project files for a specific build configuration
        /// </summary>
        /// <param name="context">the cake context</param>
        /// <param name="target">the solution or project file</param>
        /// <param name="configuration">the build configuration</param>
        /// <returns>the list of output assembly paths</returns>
        [CakeMethodAlias]
        public static IEnumerable<FilePath> GetOutputAssemblies(this ICakeContext context, FilePath target, string configuration)
        {
            if (!target.IsProject() && !target.IsSolution())
                throw new ArgumentException(
                        $"Cannot get target assemblies, {target.FullPath} is not a project or solution file");

            return target.IsSolution()
                    ? context.GetSolutionAssemblies(target, configuration)
                    : new[] { context.GetProjectAssembly(target, configuration) };
        }

        /// <summary>
        /// Gets the output assembly paths for a solution file for a specific build configuration
        /// </summary>
        /// <param name="context">the cake context</param>
        /// <param name="target">the solution file</param>
        /// <param name="configuration">the build configuration</param>
        /// <returns>the list of output assembly paths</returns>
        [CakeMethodAlias]
        public static IEnumerable<FilePath> GetSolutionAssemblies(this ICakeContext context, FilePath target, string configuration)
        {
            if (!target.IsSolution())
                throw new ArgumentException(
                    $"Cannot get target assemblies, {target.FullPath} is not a solution file");

            var result = context.ParseSolution(target);
            return result.GetProjects().Select(x => context.GetProjectAssembly(x.Path, configuration));
        }

        /// <summary>
        /// Gets the output assembly path for a project file
        /// </summary>
        /// <param name="context">the cake context</param>
        /// <param name="target">the project file</param>
        /// <param name="configuration">the build configuration</param>
        /// <returns>the output assembly path</returns>
        [CakeMethodAlias]
        public static FilePath GetProjectAssembly(this ICakeContext context, FilePath target, string configuration)
        {
            if(!target.IsProject())
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
        public static void Move(this ICakeContext context, FilePath source, FilePath destination)
        {
            source.ThrowIfNull(nameof(source));
            destination.ThrowIfNull(nameof(destination));

            if(!context.FileSystem.Exist(source)) throw new CakeException($"{source.FullPath} does not exist");

            var file = context.FileSystem.GetFile(source);
            file.Move(destination);
        }

        [CakeMethodAlias]
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