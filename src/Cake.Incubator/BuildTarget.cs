// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/. 

namespace Cake.Incubator
{
    using System.Collections.Generic;

    /// <summary>
    /// A project build target
    /// </summary>
    public class BuildTarget
    {
        // TODO: Add support for include all types of targets inside a build target

        /// <summary>
        /// The build target name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Targets that will run before this one
        /// </summary>
        public string[] BeforeTargets { get; set; }

        /// <summary>
        /// Targets that will run after this one
        /// </summary>
        public string[] AfterTargets { get; set; }

        /// <summary>
        /// Targets that will run before this one
        /// </summary>
        public string[] DependsOn { get; set; }

        /// <summary>
        /// Any executable targets <see cref="BuildTargetExecutable"/>
        /// </summary>
        /// <value>The build target executables <see cref="BuildTargetExecutable"/></value>
        public ICollection<BuildTargetExecutable> Executables { get; set; }
    }
}