// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/. 

namespace Cake.Incubator
{
    using Cake.Core.IO;

    /// <summary>
    /// Contains extension methods for working with <see cref="FilePath"/>'s
    /// </summary>
    public static class FilePathExtensions
    {
        /// <summary>
        /// Checks if the FilePath is a solution file
        /// </summary>
        /// <param name="filePath">the path to check</param>
        /// <returns>true if sln file</returns>
        /// <example>
        /// Check if the file is a solution
        /// <code>
        /// new FilePath("test.sln").IsSolution(); // true
        /// </code>
        /// </example>
        public static bool IsSolution(this FilePath filePath)
        {
            return filePath.HasExtension && filePath.GetExtension().EqualsIgnoreCase(".sln");
        }

        /// <summary>
        /// Checks if the FilePath is a vbproj/csproj/fsproj file
        /// </summary>
        /// <param name="filePath">the path to check</param>
        /// <returns>true if a visual studio project file was recognised</returns>
        /// <example>
        /// Check if the file is a project
        /// <code>
        /// new FilePath("test.csproj").IsProject(); // true;
        /// new FilePath("test.fsproj").IsProject(); // true;
        /// new FilePath("test.vbproj").IsProject(); // true;
        /// </code>
        /// </example>
        public static bool IsProject(this FilePath filePath)
        {
            return filePath.HasExtension &&
                    (filePath.GetExtension().EqualsIgnoreCase(".vbproj") || filePath.GetExtension().EqualsIgnoreCase(".csproj") || filePath.GetExtension().EqualsIgnoreCase(".fsproj"));
        }

        /// <summary>
        /// Checks if the path has a specific filename and extension
        /// </summary>
        /// <param name="path">the path to check</param>
        /// <param name="fileName">the file name and extension</param>
        /// <returns>true if filename and extension matches</returns>
        /// <example>
        /// Check by the filename (includes extension)
        /// <code>
        /// new FilePath("/folder/testing.cs").HasFileName("testing.cs"); // true
        /// </code>
        /// </example>
        public static bool HasFileName(this FilePath path, string fileName)
        {
            fileName.ThrowIfNullOrEmpty(nameof(fileName));

            return
                path.GetFilenameWithoutExtension()
                    .ToString()
                    .EqualsIgnoreCase(System.IO.Path.GetFileNameWithoutExtension(fileName));
        }
        
        /// <summary>
        /// Checks if the path has a specific file extension (case-insensitive)
        /// </summary>
        /// <param name="path">the path to check</param>
        /// <param name="fileExtension">the file extension name</param>
        /// <returns>true if file extension matches</returns>
        /// <example>
        /// Check the file extension with or without the period ignoring case
        /// <code>
        /// new FilePath("/folder/testing.cs").HasExtension(".cs"); // true
        /// new FilePath("/folder/testing.odd").HasExtension("ODD"); // true
        /// </code>
        /// </example>
        public static bool HasFileExtension(this FilePath path, string fileExtension)
        {    
            fileExtension.ThrowIfNullOrEmpty(nameof(fileExtension));
            return path.HasExtension && path.GetFilename().GetExtension().EndsWithIgnoreCase(fileExtension);
        }
    }
}