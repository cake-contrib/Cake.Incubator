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

        internal static string GetFirstElementValue(this XElement element, XName elementName)
        {
            return element.Descendants(elementName).FirstOrDefault()?.Value;
        }

        internal static IEnumerable<XElement> GetPropertyGroups(this XElement project, XNamespace ns)
        {
            return project.Elements(ns + ProjectXElement.PropertyGroup);
        }

        internal static IEnumerable<XElement> GetItemGroups(this XElement project, XNamespace ns)
        {
            return project.Elements(ns + ProjectXElement.ItemGroup);
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
        /// Gets the first (in document order) child element with the specified <see cref="XName"/>.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="name">The <see cref="XName"/> to match.</param>
        /// <param name="ignoreCase">If set to <c>true</c> case will be ignored whilst searching for the <see cref="XElement"/>.</param>
        /// <returns>A <see cref="XElement"/> that matches the specified <see cref="XName"/>, or null. </returns>
        internal static XElement Element(this XElement element, XName name, bool ignoreCase)
        {
            var el = element.Element(name);
            if (el != null)
                return el;

            if (!ignoreCase)
                return null;

            var elements = element.Elements().Where(e => e.Name.LocalName.EqualsIgnoreCase(name.ToString()));
            return !elements.Any() ? null : elements.First();
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