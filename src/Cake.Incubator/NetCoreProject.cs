// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/. 

namespace Cake.Incubator
{
    using System.Collections.Generic;
    using Cake.Common.Solution.Project;

    /// <summary>
    /// Describes a netcore project
    /// </summary>
    public class NetCoreProject
    {
        /// <summary>
        /// True if the assembly allows unsafe blocks
        /// </summary>
        public bool AllowUnsafeBlocks { get; set; }

        /// <summary>
        /// The application icon
        /// </summary>
        public string ApplicationIcon { get; set; }

        /// <summary>
        /// The assembly title, defaults to the assemblyname
        /// </summary>
        public string AssemblyTitle { get; set; }

        /// <summary>
        /// The signing key file path
        /// </summary>
        public string AssemblyOriginatorKeyFile { get; set; }

        /// <summary>
        /// The Assembly Version
        /// </summary>
        public string AssemblyVersion { get; set; }

        /// <summary>
        /// The assembly or package authors
        /// </summary>
        public string[] Authors { get; set; }

        /// <summary>
        /// The assembly or package copyright
        /// </summary>
        public string Copyright { get; set; }

        /// <summary>
        /// The assembly or package company
        /// </summary>
        public string Company { get; set; }

        /// <summary>
        /// True if compilation will output debug symbols
        /// </summary>
        public bool DebugSymbols { get; set; }

        /// <summary>
        /// The debug type (portable, embedded, full)
        /// </summary>
        public string DebugType { get; set; }

        /// <summary>
        /// Build pre-processor directives
        /// </summary>
        public string[] DefineConstants { get; set; }

        /// <summary>
        /// When delay signed, the project will not run or be debuggable
        /// </summary>
        public bool DelaySign { get; set; }

        /// <summary>
        /// Project and package description
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// The documentation file path
        /// </summary>
        public string DocumentationFile { get; set; }

        /// <summary>
        /// The dotnet CLI tool references, a collection of <see cref="DotNetCliToolReference"/>
        /// </summary>
        public ICollection<DotNetCliToolReference> DotNetCliToolReferences { get; set; }

        /// <summary>
        /// The File Version
        /// </summary>
        public string FileVersion { get; set; }

        /// <summary>
        /// True if assembly will generate xml documentation
        /// </summary>
        public bool GenerateDocumentationFile { get; set; }

        /// <summary>
        /// True if the package will be generated when building
        /// </summary>
        public bool GeneratePackageOnBuild { get; set; }

        /// <summary>
        /// Generate serialization assemblies (Off, On, Auto)
        /// </summary>
        public string GenerateSerializationAssemblies { get; set; }

        /// <summary>
        /// True if this is a web project
        /// </summary>
        public bool IsWeb { get; set; }

        /// <summary>
        /// Assembly language version (ISO-1, ISO-2, [C#]2-7)
        /// </summary>
        public string LangVersion { get; set; }

        /// <summary>
        /// The netstandard package target version if specified
        /// </summary>
        public string NetStandardImplicitPackageVersion { get; set; }

        /// <summary>
        /// The assembly neutral language
        /// </summary>
        public string NeutralLanguage { get; set; }

        /// <summary>
        /// The pragma warnings to ignore during compilation
        /// </summary>
        public string[] NoWarn { get; set; }

        /// <summary>
        /// The optimize code flag
        /// </summary>
        public bool Optimize { get; set; }

        /// <summary>
        /// The package icon url
        /// </summary>
        public string PackageIconUrl { get; set; }

        /// <summary>
        /// The package id
        /// </summary>
        public string PackageId { get; set; }

        /// <summary>
        /// The package licence url
        /// </summary>
        public string PackageLicenseUrl { get; set; }

        /// <summary>
        /// The package project url
        /// </summary>
        public string PackageProjectUrl { get; set; }

        /// <summary>
        /// The project package references. A collection of <see cref="PackageReference"/>
        /// </summary>
        public ICollection<PackageReference> PackageReferences { get; set; }

        /// <summary>
        /// True if installing the package requires accepting the licence
        /// </summary>
        public bool PackageRequireLicenseAcceptance { get; set; }

        /// <summary>
        /// The package release notes
        /// </summary>
        public string PackageReleaseNotes { get; set; }

        /// <summary>
        /// The package tags
        /// </summary>
        public string[] PackageTags { get; set; }

        /// <summary>
        /// The fallback targets to use when importing packages
        /// </summary>
        public string[] PackageTargetFallbacks { get; set; }

        /// <summary>
        /// Required to be true for the compilation of razor views
        /// </summary>
        public bool PreserveCompilationContext { get; set; }

        /// <summary>
        /// The product name
        /// </summary>
        public string Product { get; set; }

        /// <summary>
        /// The references to other projects. A collection of <see cref="ProjectReference"/>
        /// </summary>
        public ICollection<ProjectReference> ProjectReferences { get; set; }

        /// <summary>
        /// Undocumented flag relating to assembly signing
        /// </summary>
        public bool PublicSign { get; set; }

        /// <summary>
        /// The source control repository type for the project
        /// </summary>
        public string RepositoryType { get; set; }

        /// <summary>
        /// The source control repository url
        /// </summary>
        public string RepositoryUrl { get; set; }

        /// <summary>
        /// The runtime framework version
        /// </summary>
        public string RuntimeFrameworkVersion { get; set; }

        /// <summary>
        /// The runtime identifiers
        /// </summary>
        public string[] RuntimeIdentifiers { get; set; }

        /// <summary>
        /// Optional runtime options to override the default settings <see cref="RuntimeOptions"/>
        /// </summary>
        public RuntimeOptions RuntimeOptions { get; set; }

        /// <summary>
        /// The net core sdk
        /// </summary>
        public string Sdk { get; set; }

        /// <summary>
        /// True if the assembly is signed during compilation
        /// </summary>
        public bool SignAssembly { get; set; }

        /// <summary>
        /// The projects build targets. A collection of <see cref="BuildTarget"/>
        /// </summary>
        public ICollection<BuildTarget> Targets { get; set; }

        /// <summary>
        /// The projects target frameworks
        /// </summary>
        public string[] TargetFrameworks { get; set; }

        /// <summary>
        /// The warnings to specifically treat as errors
        /// </summary>
        public string[] TreatSpecificWarningsAsErrors { get; set; }

        /// <summary>
        /// True if wanrings will be treated as errors during compilation
        /// </summary>
        public bool TreatWarningsAsErrors { get; set; }

        /// <summary>
        /// The project version
        /// </summary>
        public string Version { get; set; }

        /// <summary>
        /// Compiler warning level
        /// </summary>
        public string WarningLevel { get; set; }
    }
}