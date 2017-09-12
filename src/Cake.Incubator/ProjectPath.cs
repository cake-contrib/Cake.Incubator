// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/. 

namespace Cake.Incubator
{
    /// <summary>
    /// Class which describes the Path to a Visual Studio Project.
    /// </summary>
    public class ProjectPath
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProjectPath"/> class.
        /// </summary>
        /// <param name="path">The path to the project file.</param>
        public ProjectPath(string path)
        {
            Path = path;
        }

        /// <summary>
        /// Gets or sets the path to the Project file.
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// Gets a value indicating whether the Project Path is to an actual file.
        /// </summary>
        // ReSharper disable once UnusedMember.Global
        public bool IsFile => Path.Contains("*");
    }
}