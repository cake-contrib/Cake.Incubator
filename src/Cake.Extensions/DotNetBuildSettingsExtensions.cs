// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/. 
namespace Cake.Extensions
{
    using System.Collections.Generic;
    using Cake.Common.Tools;

    public static class DotNetBuildSettingsExtensions
    {
        /// <summary>
        /// Adds .NET build targets to the configuration.
        /// </summary>
        /// <param name="settings">The settings.</param>
        /// <param name="targets">The .NET build targets.</param>
        /// <returns>The same <see cref="DotNetBuildSettings"/> instance so that multiple calls can be chained.</returns>
        public static DotNetBuildSettings WithTargets(this DotNetBuildSettings settings, IEnumerable<string> targets)
        {
            settings.ThrowIfNull(nameof(settings));
            targets.Each(target => settings.Targets.Add(target));
            return settings;
        }
    }
}