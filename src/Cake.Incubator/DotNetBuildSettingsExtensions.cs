// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/. 

namespace Cake.Incubator.DotNetBuildExtensions
{
    using System.Collections.Generic;
    using Cake.Common.Tools.DotNetCore.MSBuild;
    using Cake.Core.Annotations;
    using Cake.Incubator.AssertExtensions;
    using Cake.Incubator.EnumerableExtensions;

    /// <summary>
    /// Several extension methods when using DotNetBuildSettings.
    /// </summary>
    [CakeNamespaceImport("Cake.Incubator.DotNetBuildExtensions")]
    public static class DotNetBuildSettingsExtensions
    {
        /// <summary>
        /// Adds multiple .NET build targets to the configuration.
        /// </summary>
        /// <param name="settings">The settings.</param>
        /// <param name="targets">The .NET build targets.</param>
        /// <returns>The same <see cref="DotNetCoreMSBuildSettings"/> instance so that multiple calls can be chained.</returns>
        /// <example>
        /// Add many targets to the build settings
        /// <code>
        /// var settings = new DotNetCoreMSBuildSettings().WithTargets(new[] { "Clean", "Build", "Publish" });
        /// </code>
        /// </example>
        public static DotNetCoreMSBuildSettings WithTargets(this DotNetCoreMSBuildSettings settings, IEnumerable<string> targets)
        {
            settings.ThrowIfNull(nameof(settings));
            targets.Each(target => settings.Targets.Add(target));
            return settings;
        }
    }
}