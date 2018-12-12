// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/. 

namespace Cake.Incubator
{
    using System.Collections.Generic;
    using System.Collections.Specialized;
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
        /// dotnet pack: A list of packages authors, matching the profile names on nuget.org. 
        /// These are displayed in the NuGet Gallery on nuget.org and are used to cross-reference packages by the same authors.
        /// </summary>
        public string[] Authors { get; set; }

        /// <summary>
        /// dotnet pack: Specifies the folder where to place the output assemblies. 
        /// The output assemblies (and other output files) are copied into their respective framework folders.
        /// </summary>
        public string BuildOutputTargetFolder { get; set; }

        /// <summary>
        /// dotnet pack: This property specifies the default location of where all the content files should go if PackagePath is not specified for them. 
        /// The default value is "content;contentFiles".
        /// </summary>
        public string[] ContentTargetFolders { get; set; }

        /// <summary>
        /// Copyright details for the package.
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
        /// dotnet pack: A long description of the package for UI display.
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
        /// dotnet pack: This Boolean value indicates whether the package should create an additional symbols package when the project is packed. 
        /// This package will have a .symbols.nupkg extension and will copy the PDB files along with the DLL and other output files.
        /// </summary>
        public bool IncludeSymbols { get; set; }

        /// <summary>
        /// dotnet pack: This Boolean value indicates whether the pack process should create a source package. 
        /// The source package contains the library's source code as well as PDB files. 
        /// Source files are put under the src/ProjectName directory in the resulting package file.
        /// </summary>
        public bool IncludeSource { get; set; }

        /// <summary>
        /// dotnet pack: This Boolean values specifies whether the build output assemblies should be packed into the .nupkg file or not.
        /// </summary>
        public bool IncludeBuildOutput { get; set; }

        /// <summary>
        /// dotnet pack: This Boolean value specifies whether any items that have a type of Content will be included in the resulting package automatically. The default is true.
        /// </summary>
        public bool IncludeContentInPack { get; set; } = true;

        /// <summary>
        /// dotnet pack: A Boolean value that specifies whether the project can be packed. The default value is true.
        /// </summary>
        public bool IsPackable { get; set; } = true;

        /// <summary>
        /// dotnet pack: Specifies whether all output files are copied to the tools folder instead of the lib folder. 
        /// Note that this is different from a DotNetCliTool which is specified by setting the PackageType in the .csproj file.
        /// </summary>
        public bool IsTool { get; set; }

        /// <summary>
        /// True if this is a web project
        /// </summary>
        public bool IsWeb { get; set; }

        /// <summary>
        /// Assembly language version (ISO-1, ISO-2, [C#]2-7)
        /// </summary>
        public string LangVersion { get; set; }

        /// <summary>
        /// dotnet pack: Specifies the minimum version of the NuGet client that can install this package, enforced by nuget.exe and the Visual Studio Package Manager.
        /// </summary>
        public string MinClientVersion { get; set; }

        /// <summary>
        /// The netstandard package target version if specified
        /// </summary>
        public string NetStandardImplicitPackageVersion { get; set; }

        /// <summary>
        /// The assembly neutral language
        /// </summary>
        public string NeutralLanguage { get; set; }

        /// <summary>
        /// dotnet pack: Specifies that pack should not run package analysis after building the package.
        /// </summary>
        public bool NoPackageAnalysis { get; set; }

        /// <summary>
        /// The pragma warnings to ignore during compilation
        /// </summary>
        public string[] NoWarn { get; set; }

        /// <summary>
        /// dotnet pack: Base path for the .nuspec file.
        /// </summary>
        public string NuspecBasePath { get; set; }

        /// <summary>
        /// dotnet pack: Relative or absolute path to the .nuspec file being used for packing
        /// </summary>
        public string NuspecFile { get; set; }

        /// <summary>
        /// dotnet pack: list of key=value pairs.
        /// </summary>
        public NameValueCollection NuspecProperties { get; set; }

        /// <summary>
        /// The optimize code flag
        /// </summary>
        public bool Optimize { get; set; }

        /// <summary>
        /// dotnet pack: A URL for a 64x64 image with transparent background to use as the icon for the package in UI display.
        /// </summary>
        public string PackageIconUrl { get; set; }

        /// <summary>
        /// dotnet pack: The package id
        /// </summary>
        public string PackageId { get; set; }

        /// <summary>
        /// dotnet pack: An URL to the license that is applicable to the package.
        /// </summary>
        public string PackageLicenseUrl { get; set; }

        /// <summary>
        /// dotnet pack: Determines the output path in which the packed package will be dropped. Default is the OutputPath
        /// </summary>
        public string PackageOutputPath { get; set; }

        /// <summary>
        /// dotnet pack: A URL for the package's home page, often shown in UI displays as well as nuget.org.
        /// </summary>
        public string PackageProjectUrl { get; set; }

        /// <summary>
        /// The project package references. A collection of <see cref="PackageReference"/>
        /// </summary>
        public ICollection<PackageReference> PackageReferences { get; set; }

        /// <summary>
        /// dotnet pack: A Boolean value that specifies whether the client must prompt the consumer to accept the package license before installing the package. 
        /// The default is false.
        /// </summary>
        public bool PackageRequireLicenseAcceptance { get; set; }

        /// <summary>
        /// dotnet pack: Release notes for the package.
        /// </summary>
        public string PackageReleaseNotes { get; set; }

        /// <summary>
        /// dotnet pack: A list of tags that designates the package.
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
        /// dotnet pack: Specifies the type of the repository. Default is "git"
        /// </summary>
        public string RepositoryType { get; set; }

        /// <summary>
        /// dotnet pack: Specifies the URL for the repository where the source code for the package resides and/or from which it's being built.
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
        /// dotnet pack: A human-friendly title of the package, typically used in UI displays as on nuget.org and the Package Manager in Visual Studio. 
        /// If not specified, the package ID is used instead.
        /// </summary>
        public string Title { get; set; }

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

        /// <summary>
        /// True if it should pack it as global tool
        /// </summary>
        public bool PackAsTool { get; set; }
    }
}