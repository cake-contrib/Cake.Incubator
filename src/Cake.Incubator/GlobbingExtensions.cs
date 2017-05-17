// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/. 

namespace Cake.Incubator
{
    using System.Collections.Generic;
    using System.Linq;
    using Cake.Core;
    using Cake.Core.Annotations;
    using Cake.Core.IO;

    /// <summary>
    /// Contains functionality related to file system globbing. 
    /// </summary>
    [CakeAliasCategory("Globbing")]
    public static class GlobbingExtensions
    {
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
        [CakeAliasCategory("Files")]
        public static IEnumerable<IFile> GetMatchingFiles(this ICakeContext context, IEnumerable<FilePath> files)
        {
            return files.SelectMany(
                x =>
                    context.FileSystem.GetDirectory(x.GetDirectory())
                        .GetFiles($"{x.GetFilenameWithoutExtension()}.*", SearchScope.Current)
                        .Where(f => !f.Path.Equals(x)));
        }

        /// <summary>
        /// Gets FilePaths using glob patterns
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
        [CakeAliasCategory("Files")]
        public static FilePathCollection GetFiles(this ICakeContext context, params string[] patterns)
        {
            return
                patterns.Aggregate(
                    new FilePathCollection(new PathComparer(context.Environment)),
                    (current, pattern) =>
                        current + context.Globber.GetFiles(pattern));
        }
    }
}
