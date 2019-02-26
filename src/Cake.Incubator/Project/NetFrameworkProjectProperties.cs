// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/. 

namespace Cake.Incubator.Project
{
    using System;
    using Cake.Core.IO;

    /// <summary>
    /// Represents the project properties for a .Net Framework project file
    /// </summary>
    internal class NetFrameworkProjectProperties
    {
        public string Configuration { get; set; }
        public string Platform { get; set; }
        public string ProjectGuid { get; set; }
        public string[] ProjectTypeGuids { get; set; }
        public string OutputType { get; set; }
        public DirectoryPath OutputPath { get; set; }
        public string RootNameSpace { get; set; }
        public string AssemblyName { get; set; }
        [Obsolete("Use TargetFrameworkVersions insead")]
        public string TargetFrameworkVersion { get; set; }
        public string[] TargetFrameworkVersions { get; set; }
        public string TargetFrameworkProfile { get; set; }
    }
}