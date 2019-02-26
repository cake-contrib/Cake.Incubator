// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/. 

namespace Cake.Incubator.Project
{
    /// <summary>
    /// A project package reference
    /// </summary>
    public class PackageReference
    {
        /// <summary>
        /// The package name
        /// </summary>
        public string Name { get; set; }
        
        /// <summary>
        /// The package version
        /// </summary>
        public string Version { get; set; }

        /// <summary>
        /// returns any package specific target framework
        /// </summary>
        public string TargetFramework { get; set; }

        /// <summary>
        /// These assets will be consumed but won't flow to the parent project
        /// </summary>
        /// <remarks>For example, build-time only dependencies</remarks>
        public string[] PrivateAssets { get; set; }

        /// <summary>
        /// These assets will be consumed
        /// </summary>
        public string[] IncludeAssets { get; set; }

        /// <summary>
        /// These assets will not be consumed
        /// </summary>
        public string[] ExcludeAssets { get; set; }
    }
}