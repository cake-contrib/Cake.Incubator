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
        internal static string GetTargetFrameworkProfile(this XElement propertyGroup, XNamespace ns)
        {
            return propertyGroup
                .Elements(ns + ProjectXElement.TargetFrameworkProfile)
                .Select(targetFrameworkProfile => targetFrameworkProfile.Value)
                .FirstOrDefault();
        }

        internal static string GetTargetFrameworkVersion(this XElement propertyGroup, XNamespace ns)
        {
            return propertyGroup
                .Elements(ns + ProjectXElement.TargetFrameworkVersion)
                .Select(targetFrameworkVersion => targetFrameworkVersion.Value)
                .FirstOrDefault();
        }

        internal static string GetAssemblyName(this XElement propertyGroup, XNamespace ns)
        {
            return propertyGroup
                .Elements(ns + ProjectXElement.AssemblyName)
                .Select(assemblyName => assemblyName.Value)
                .FirstOrDefault();
        }

        internal static string GetRootNamespace(this XElement propertyGroup, XNamespace ns)
        {
            return propertyGroup
                .Elements(ns + ProjectXElement.RootNamespace)
                .Select(rootNameSpace => rootNameSpace.Value)
                .FirstOrDefault();
        }

        internal static DirectoryPath GetOutputPath(this IEnumerable<XElement> configPropertyGroups, XNamespace ns, DirectoryPath rootPath)
        {
            return configPropertyGroups
                .Elements(ns + ProjectXElement.OutputPath)
                .Select(outputPath => rootPath.Combine(DirectoryPath.FromString(outputPath.Value)))
                .FirstOrDefault();
        }

        internal static string GetOutputType(this XElement propertyGroup, XNamespace ns)
        {
            return propertyGroup
                .Elements(ns + ProjectXElement.OutputType)
                .Select(outputType => outputType.Value)
                .FirstOrDefault();
        }

        internal static string GetProjectType(this XElement propertyGroup, XNamespace ns)
        {
            return propertyGroup
                .Elements(ns + ProjectXElement.ProjectTypeGuids)
                .Select(projectTypeGuid => projectTypeGuid.Value)
                .FirstOrDefault();
        }

        internal static string GetProjectGuid(this XElement propertyGroup, XNamespace ns)
        {
            return propertyGroup
                .Elements(ns + ProjectXElement.ProjectGuid)
                .Select(projectGuid => projectGuid.Value)
                .FirstOrDefault();
        }
    }
}