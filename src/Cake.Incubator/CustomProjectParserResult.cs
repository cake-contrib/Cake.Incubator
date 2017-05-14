namespace Cake.Incubator
{
    using System.Collections.Generic;
    using Cake.Common.Solution.Project;
    using Cake.Core.IO;

    /// <summary>Represents the content in an MSBuild project file.</summary>
    public class CustomProjectParserResult
    {
        /// <summary>Gets the build configuration.</summary>
        /// <value>The build configuration.</value>
        public string Configuration { get; set; }

        /// <summary>Gets the target platform.</summary>
        /// <value>The platform.</value>
        public string Platform { get; set; }

        /// <summary>Gets the unique project identifier.</summary>
        /// <value>The unique project identifier.</value>
        public string ProjectGuid { get; set; }

        /// <summary>Gets the project type identifiers.</summary>
        /// <value>The project type identifiers.</value>
        public string[] ProjectTypeGuids { get; set; }

        /// <summary>
        /// Gets the compiler output type, i.e. <c>Exe/Library</c>.
        /// </summary>
        /// <value>The output type.</value>
        public string OutputType { get; set; }

        /// <summary>Gets the compiler output path.</summary>
        /// <value>The output path.</value>
        public DirectoryPath OutputPath { get; set; }

        /// <summary>Gets the default root namespace.</summary>
        /// <value>The root namespace.</value>
        public string RootNameSpace { get; set; }

        /// <summary>Gets the build target assembly name.</summary>
        /// <value>The assembly name.</value>
        public string AssemblyName { get; set; }

        /// <summary>Gets the compiler target framework version.</summary>
        /// <value>The target framework version.</value>
        public string TargetFrameworkVersion { get; set; }

        /// <summary>Gets the compiler target framework profile.</summary>
        /// <value>The target framework profile.</value>
        public string TargetFrameworkProfile { get; set; }

        /// <summary>Gets the project content files. <see cref="CustomProjectFile"/></summary>
        /// <value>The files. <see cref="CustomProjectFile"/></value>
        public ICollection<CustomProjectFile> Files { get; set; }

        /// <summary>Gets the references. <see cref="ProjectAssemblyReference"/></summary>
        /// <value>The references. <see cref="ProjectAssemblyReference"/></value>
        public ICollection<ProjectAssemblyReference> References { get; set; }

        /// <summary>Gets the references to other projects. <see cref="ProjectReference"/></summary>
        /// <value>The references. <see cref="ProjectReference"/></value>
        public ICollection<ProjectReference> ProjectReferences { get; set; }

        /// <summary>
        /// True if the project is a net core project
        /// </summary>
        public bool IsNetCore { get; set; }

        /// <summary>
        /// True if the project is a net framework project
        /// </summary>
        public bool IsNetFramework { get; set; }

        /// <summary>
        /// Contains properties specific to net core projects. See <see cref="NetCoreProject"/>
        /// </summary>
        public NetCoreProject NetCore { get; set; }
    }
}