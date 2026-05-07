// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

namespace Cake.Incubator.Project
{
    /// <summary>
    /// Represents a single &lt;PackageReference&gt; element parsed out of a
    /// project file by <see cref="ProjectParserExtensions.ParseProject(Cake.Core.ICakeContext,Cake.Core.IO.FilePath,string)"/>.
    /// </summary>
    /// <remarks>
    /// Renamed from <c>PackageReference</c> to <c>ParsedPackageReference</c> in v11.0.0
    /// to disambiguate from <c>Cake.Core.Packaging.PackageReference</c>. The two types
    /// have unrelated purposes (this one is a csproj-parsing DTO; the Cake.Core type
    /// describes a runtime tool-installation reference) but the unqualified short
    /// name collided whenever both namespaces were brought into the same compilation
    /// unit — most notably inside Cake.Sdk's source-generated helpers, which
    /// prevented <c>cake.cs</c> consumers from compiling against this addin at all.
    /// See cake-contrib/Cake.Incubator#268 for the rename background.
    /// </remarks>
    public class ParsedPackageReference
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
