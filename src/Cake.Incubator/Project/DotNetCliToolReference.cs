// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/. 

namespace Cake.Incubator.Project
{
    /// <summary>
    /// A dotnet cli tool referenceS
    /// </summary>
    public class DotNetCliToolReference
    {
        /// <summary>
        /// The dotnet cli tool name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The tool version
        /// </summary>
        public string Version { get; set; }
    }
}