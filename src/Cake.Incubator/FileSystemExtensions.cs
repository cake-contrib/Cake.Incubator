// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/. 

namespace Cake.Incubator.FileSystemExtensions
{
    using System.Globalization;
    using Cake.Core;
    using Cake.Core.IO;

    /// <summary>
    /// Contains extension methods for working with cakes <see cref="IFileSystem"/>
    /// </summary>
    internal static class FileSystemExtensions
    {
        /// <summary>
        /// Loads a visual studio project file
        /// </summary>
        /// <param name="fs">the filesystem</param>
        /// <param name="projectPath">the path of the project file</param>
        /// <returns>the project file</returns>
        /// <exception cref="CakeException">Throws if the file does not exist or is not a recognised visual studio project file</exception>
        internal static IFile GetProjectFile(this IFileSystem fs, FilePath projectPath)
        {
            var file = fs.GetFile(projectPath);
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
            return file;
        }
    }
}