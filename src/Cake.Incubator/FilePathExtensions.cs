// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/. 

namespace Cake.Incubator
{
    using Cake.Core.Annotations;
    using Cake.Core.IO;

    /// <summary>
    /// Contains extension methods for working with <see cref="FilePath"/>'s
    /// </summary>
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
    }
}