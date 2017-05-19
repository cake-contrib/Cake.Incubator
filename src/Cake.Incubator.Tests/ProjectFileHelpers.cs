// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/. 

namespace Cake.Incubator.Tests
{
    public class ProjectFileHelpers
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
    }
}