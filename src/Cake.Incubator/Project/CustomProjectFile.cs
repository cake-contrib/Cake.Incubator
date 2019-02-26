// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/. 

namespace Cake.Incubator.Project
{
    /// <summary>
    /// Represents a MSBuild project file.
    /// </summary>
    public sealed class CustomProjectFile
    {
        /// <summary>Gets or sets the project file path.</summary>
        /// <value>The project file path.</value>
        public ProjectPath FilePath { get; set; }

        /// <summary>Gets or sets the relative path to the project file.</summary>
        /// <value>The relative path to the project file.</value>
        public string RelativePath { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="T:Cake.Common.Solution.Project.ProjectFile" /> is compiled.
        /// </summary>
        /// <value>
        ///   <c>true</c> if compiled; otherwise, <c>false</c>.
        /// </value>
        public bool Compile { get; set; }
    }
}