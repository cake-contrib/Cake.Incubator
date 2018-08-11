// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/. 

namespace Cake.Incubator.Tests
{
    using System;
    using System.IO;
    using System.Linq;
    using Castle.Components.DictionaryAdapter;

    public static class ProjectFileHelpers
    {
        public static string GetNetCoreProjectWithElement(string element, string value, string attribute = null)
        {
            return $"<Project sdk=\"Microsoft.NET.Sdk\"><{element} {attribute}>{value}</{element}></Project>";
        }

        public static string GetNetCoreProjectWithString(string content)
        {
            return $"<Project sdk=\"Microsoft.NET.Sdk\">{content}</Project>";
        }

        public static string GetNetCoreWebProjectWithString(string content)
        {
            return $"<Project sdk=\"Microsoft.NET.Sdk.Web\">{content}</Project>";
        }

        public static string GetNetCoreProjectElementWithConfig(string element, string value, string config, string platform)
        {
            return GetNetCoreProjectWithElement(element, value, $@"Condition=""'$(Configuration)|$(Platform)'=='{config}|{platform}'""");
        }

        public static string GetNetCoreProjectElementWithParentConfig(string element, string value, string config, string platform)
        {
            return GetNetCoreProjectWithString($@"<PropertyGroup Condition=""'$(Configuration)|$(Platform)'=='{config}|{platform}'""><{element}>{value}</{element}></PropertyGroup>");
        }

        public static string SafeLoad(this string fileName)
        {
            // issue with embedded resources from dotnet build vs dotnet msbuild
            // https://github.com/Microsoft/msbuild/issues/2221
            var resourceName = $"Cake.Incubator.Tests.sampleprojects.{fileName}.xml";
            using (var stream = typeof(ProjectFileHelpers).Assembly.GetManifestResourceStream(resourceName))
            {
                if (stream == null)
                {
                    throw new Exception($"Resource {resourceName} not found.  Valid resources are: {string.Join(", ", typeof(ProjectFileHelpers).Assembly.GetManifestResourceNames())}.");
                }
                using (var reader = new StreamReader(stream))
                {
                    return reader.ReadToEnd();
                }
            }
        }
    }
}