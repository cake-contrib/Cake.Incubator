// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/. 

namespace Cake.Incubator.Tests
{
    public class ProjectFileHelpers
    {
        public static string GetNetCoreProjectWithElement(string element, string value)
        {
            return $"<Project sdk=\"Microsoft.NET.Sdk\"><{element}>{value}</{element}></Project>";
        }

        public static string GetNetCoreProjectWithString(string content)
        {
            return $"<Project sdk=\"Microsoft.NET.Sdk\">{content}</Project>";
        }

        public static string GetNetCoreWebProjectWithString(string content)
        {
            return $"<Project sdk=\"Microsoft.NET.Sdk.Web\">{content}</Project>";
        }
    }
}