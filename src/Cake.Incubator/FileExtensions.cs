// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/. 

namespace Cake.Incubator
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Xml.Linq;
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
        // ReSharper disable once UnusedMember.Global
        public static IEnumerable<FilePath> Filter(this FilePathCollection filePathCollection, params string[] fileNames)
        {
            return
                fileNames
                    .Select(fileName => filePathCollection.SingleOrDefault(x => x.HasFileName(fileName)))
                    .Where(match => match != null)
                    .ToList();
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
        // ReSharper disable once UnusedMember.Global
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
        // ReSharper disable once UnusedMember.Global
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

        /// <summary>
        /// Loads an xml file
        /// </summary>
        /// <param name="xmlFile">the xml file path</param>
        /// <returns>An XDocument</returns>
        /// <example>Load an xml document
        /// <code>
        /// XDocument doc = new File("./proj.csproj").LoadXml();
        /// </code>
        /// </example>
        public static XDocument LoadXml(this IFile xmlFile)
        {
            XDocument document;
            using (var stream = xmlFile.OpenRead())
            {
                document = XDocument.Load(stream);
            }
            return document;
        }
    }
}