// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/. 
namespace Cake.Incubator
{
    using System;
    using System.Linq;
    using System.Xml.Linq;

    public static class XDocumentExtensions
    {
        /// <summary>
        /// Gets the first output path value for a specific config from an xml document
        /// </summary>
        /// <param name="document">The xml document</param>
        /// <param name="config">the configuration</param>
        /// <param name="platform">the platform</param>
        /// <returns>the output path</returns>
        public static string GetOutputPath(this XDocument document, string config, string platform = "AnyCPU")
        {
            return document.Descendants("OutputPath")
                .FirstOrDefault(x => x.Parent.Attribute("Condition")
                    .Value
                    .EndsWith($"=='{config}|{platform}'", StringComparison.OrdinalIgnoreCase))?.Value;
        }

        /// <summary>
        /// Gets the first platform target value for a specific config from an xml document
        /// </summary>
        /// <param name="document">The xml document</param>
        /// <param name="config">the configuration</param>
        /// <param name="platform">the platform</param>
        /// <returns>the platform target</returns>
        public static string GetPlatformTarget(this XDocument document, string config, string platform = "AnyCPU")
        {
            return document.Descendants("PlatformTarget")
                .FirstOrDefault(x => x.Parent.Attribute("Condition")
                    .Value
                    .EndsWith($"=='{config}|{platform}'", StringComparison.OrdinalIgnoreCase))?.Value;
        }

        /// <summary>
        /// Gets the first targetframework value from an xml document
        /// </summary>
        /// <param name="document">The xml document</param>
        /// <returns>the target framework</returns>
        public static string GetTargetFramework(this XDocument document)
        {
            return document.Descendants(ProjectXElement.TargetFramework).FirstOrDefault()?.Value;
        }

        /// <summary>
        /// Checks if an xml document for the dot net sdk attribute
        /// </summary>
        /// <param name="document">The xml document</param>
        /// <returns>True if attribute was found</returns>
        public static bool IsDotNetSdk(this XDocument document)
        {
            return document.Root?.Attribute("Sdk")?.Value != null;
        }
    }
}