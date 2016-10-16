// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/. 

namespace Cake.Core
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using Cake.Common.Solution;
    using Cake.Common.Solution.Project;
    using Cake.Core.Annotations;
    using Cake.Core.IO;

    public static class FilePathExtensions
    {
        public static bool IsSolution(this FilePath filePath)
        {
            return filePath.GetExtension().Equals(".sln", StringComparison.InvariantCultureIgnoreCase);
        }

        public static bool IsProject(this FilePath filePath)
        {
            return filePath.GetExtension().Equals(".csproj", StringComparison.InvariantCultureIgnoreCase);
        }

        public static bool HasFileName(this FilePath path, string fileName)
        {
            return path.GetFilename().ToString().Equals(fileName, StringComparison.InvariantCultureIgnoreCase);
        }

        /// <summary>
        /// Filters FilePathCollection by filenames, in the order specified
        /// </summary>
        /// <param name="filePathCollection"></param>
        /// <param name="fileNames"></param>
        /// <returns></returns>
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
        /// <param name="context"></param>
        /// <param name="files"></param>
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

        [CakeMethodAlias]
        public static FilePathCollection GetFiles(this ICakeContext context, params string[] patterns)
        {
            return
                patterns.Aggregate(
                    new FilePathCollection(new PathComparer(context.Environment)),
                    (current, pattern) =>
                            current + context.Globber.GetFiles(pattern));
        }

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

        [CakeMethodAlias]
        public static IEnumerable<FilePath> GetSolutionAssemblies(this ICakeContext context, FilePath target, string configuration)
        {
            if (!target.IsSolution())
                throw new ArgumentException(
                    $"Cannot get target assemblies, {target.FullPath} is not a solution file");

            var result = context.ParseSolution(target);
            return result.GetProjects().Select(x => x.GetAssemblyFilePath(context.ParseProject(x.Path, configuration)));
        }

        [CakeMethodAlias]
        public static FilePath GetProjectAssembly(this ICakeContext context, FilePath target, string configuration)
        {
            if(!target.IsProject())
                throw new ArgumentException(
                    $"Cannot get target assembly, {target.FullPath} is not a project file");

            return context.ParseProject(target).GetAssemblyFilePath();
        }
    }
}