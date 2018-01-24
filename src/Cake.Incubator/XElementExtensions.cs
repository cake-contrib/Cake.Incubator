// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/. 

namespace Cake.Incubator
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Xml.Linq;
    using Cake.Core.IO;

    internal static class XElementExtensions
    {
        internal static string GetAttributeValue(this XElement element, string attributeName)
        {
            return element.Attribute(attributeName)?.Value;
        }

        /// <summary>
        /// gets the first matching element value, if a config is passed, it will only match an element with the specified config and platform condition.
        /// the platform defaults to AnyCPU
        /// </summary>
        /// <param name="element">the parent element</param>
        /// <param name="elementName">the element name to match</param>
        /// <param name="config">the configuration to match</param>
        /// <param name="platform">the platform to match, default is AnyCPU</param>
        /// <returns>the matching element value if found</returns>
        internal static string GetFirstElementValue(this XElement element, XName elementName, string config = null, string platform = "AnyCPU")
        {
            // if no config specified, return first value without config condition
            if (config.IsNullOrEmpty()) return element.Descendants(elementName).FirstOrDefault(x => !x.WithConfigCondition())?.Value;

            // next will look to match the config|platform condition of an element or it's parent, if that fails, 
            // will fallback to grab the first matching value without a condition on the element or it's parent.
            return element.Descendants(elementName).FirstOrDefault(x => x.WithConfigCondition(config, platform))
                       ?.Value ?? element.Descendants(elementName).FirstOrDefault(x => !x.WithConfigCondition())?.Value;
        }

        /// <summary>
        /// checks the element for a config condition attribute. If not found, also check the parent
        /// </summary>
        /// <param name="element">the element</param>
        /// <param name="config">the optional config value to match</param>
        /// <param name="platform">the optional platform value to match</param>
        /// <returns>true if a matching condition is found</returns>
        internal static bool WithConfigCondition(this XElement element, string config = null, string platform = null)
        {
            var configAttribute = element.Attribute("Condition")?.Value.HasConfigPlatformCondition(config, platform);
            if(!configAttribute.HasValue) configAttribute = element.Parent?.Attribute("Condition")?.Value.HasConfigPlatformCondition(config, platform);
            return configAttribute ?? false;
        }

        internal static IEnumerable<XElement> GetPropertyGroups(this XElement project, XNamespace ns)
        {
            return project.Elements(ns + ProjectXElement.PropertyGroup);
        }

        internal static string GetPlatform(this XElement propertyGroup, XNamespace ns)
        {
            return propertyGroup.GetFirstElementValue(ns + ProjectXElement.Platform);
        }

        internal static string GetTargetFrameworkProfile(this XElement propertyGroup, XNamespace ns)
        {
            return propertyGroup.GetFirstElementValue(ns + ProjectXElement.TargetFrameworkProfile);
        }

        internal static string GetTargetFrameworkVersion(this XElement propertyGroup, XNamespace ns)
        {
            return propertyGroup.GetFirstElementValue(ns + ProjectXElement.TargetFrameworkVersion);
        }
        
        internal static string GetTargetFrameworkVersions(this XElement propertyGroup, XNamespace ns)
        {
            return propertyGroup.GetFirstElementValue(ns + ProjectXElement.TargetFrameworkVersions);
        }

        internal static string GetAssemblyName(this XElement propertyGroup, XNamespace ns)
        {
            return propertyGroup.GetFirstElementValue(ns + ProjectXElement.AssemblyName);
        }

        internal static string GetRootNamespace(this XElement propertyGroup, XNamespace ns)
        {
            return propertyGroup.GetFirstElementValue(ns + ProjectXElement.RootNamespace);
        }

        internal static DirectoryPath GetOutputPath(this IEnumerable<XElement> configPropertyGroups, XNamespace ns,
            DirectoryPath rootPath)
        {
            return configPropertyGroups
                .Elements(ns + ProjectXElement.OutputPath)
                .Select(outputPath => rootPath.Combine(DirectoryPath.FromString(outputPath.Value)))
                .FirstOrDefault();
        }

        internal static string GetOutputType(this XElement propertyGroup, XNamespace ns)
        {
            return propertyGroup.GetFirstElementValue(ns + ProjectXElement.OutputType);
        }

        internal static string GetProjectType(this XElement propertyGroup, XNamespace ns)
        {
            return propertyGroup.GetFirstElementValue(ns + ProjectXElement.ProjectTypeGuids);
        }

        internal static string GetProjectGuid(this XElement propertyGroup, XNamespace ns)
        {
            return propertyGroup.GetFirstElementValue(ns + ProjectXElement.ProjectGuid);
        }

        /// <summary>
        /// Gets the first (in document order) attribute with the specified <see cref="XName"/>.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="name">The <see cref="XName"/> to match.</param>
        /// <param name="ignoreCase">If set to <c>true</c> case will be ignored whilst searching for the <see cref="XAttribute"/>.</param>
        /// <returns>A <see cref="XAttribute"/> that matches the specified <see cref="XName"/>, or null. </returns>
        internal static XAttribute Attribute(this XElement element, XName name, bool ignoreCase)
        {
            var el = element.Attribute(name);
            if (el != null)
                return el;

            if (!ignoreCase)
                return null;

            var attributes = element.Attributes().Where(e => e.Name.LocalName.EqualsIgnoreCase(name.ToString()));
            return !attributes.Any() ? null : attributes.First();
        }
    }
}