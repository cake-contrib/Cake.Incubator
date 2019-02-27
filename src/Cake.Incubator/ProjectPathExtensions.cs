// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/. 

namespace Cake.Incubator.ProjectPathExtensions
{
    using Cake.Core.Annotations;
    using Cake.Core.IO;
    using Cake.Incubator.Project;

    /// <summary>
    /// Several extension methods when using ProjectPath.
    /// </summary>
    [CakeNamespaceImport("Cake.Incubator.ProjectPathExtensions")]
    public static class ProjectPathExtensions
    {
        /// <summary>
        /// Combines a base project path with the name of the project file.
        /// </summary>
        /// <param name="basePath">The base path to the location of the Project File.</param>
        /// <param name="path">The path to the actual Project file.</param>
        /// <returns></returns>
        public static ProjectPath CombineWithProjectPath(this DirectoryPath basePath, string path)
        {
            return new ProjectPath(System.IO.Path.Combine(basePath.FullPath, path));
        }
    }
}