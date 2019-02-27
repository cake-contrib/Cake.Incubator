// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/. 

namespace Cake.Incubator.FileExtensions
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Xml.Linq;
    using Cake.Core.Annotations;
    using Cake.Core.IO;
    using Cake.Incubator.FilePathExtensions;

    /// <summary>
    /// Extension methods when using Files.
    /// </summary>
    [CakeNamespaceImport("Cake.Incubator.FileExtensions")]
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
        /// Loads an xml file
        /// </summary>
        /// <param name="xmlFile">the xml file path</param>
        /// <returns>An XDocument</returns>
        /// <example>Load an xml document
        /// <code>
        /// XDocument doc = GetFile("./proj.csproj").LoadXml();
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