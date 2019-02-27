// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/. 

namespace Cake.Incubator.SolutionParserExtensions
{
    using System.Collections.Generic;
    using System.Linq;
    using Cake.Common.Solution;
    using Cake.Core.Annotations;
    using Cake.Core.IO;
    using Cake.Incubator.AssertExtensions;
    using Cake.Incubator.Project;
    using Cake.Incubator.StringExtensions;

    /// <summary>
    /// Several extension methods when using SolutionParser.
    /// </summary>
    [CakeNamespaceImport("Cake.Incubator.SolutionParserExtensions")]
    public static class SolutionParserExtensions
    {
        /// <summary>
        /// Checks if a SolutionProject is of type SolutionFolder
        /// </summary>
        /// <param name="project">the solutionproject</param>
        /// <returns>true if the project is a solution folder</returns>
        /// <example>
        /// Identifies a SolutionProject as a solution folder type
        /// <code>
        /// // test.sln { proj1.csproj, solutionFolder }
        /// var projects = ParseSolution(new FilePath("test.sln")).Projects;
        /// 
        /// projects[0].IsSolutionFolder(); // false
        /// projects[1].IsSolutionFolder(); // true
        /// </code>
        /// </example>
        public static bool IsSolutionFolder(this SolutionProject project)
        {
            return project.Type.EqualsIgnoreCase(ProjectTypes.SolutionFolder);
        }

        /// <summary>
        /// Checks the SolutionProject type
        /// </summary>
        /// <param name="project">The solutionproject</param>
        /// <param name="projectType">The type to check</param>
        /// <returns>true if the project type matches</returns>
        public static bool IsType(this SolutionProject project, ProjectType projectType)
        {
            return project.Type.EqualsIgnoreCase(Types[projectType]);
        }

        /// <summary>
        /// Gets the SolutionProjects, excluding any SolutionFolders
        /// </summary>
        /// <param name="projects">The SolutionProject collection</param>
        /// <returns>The SolutionProjects</returns>
        /// <example>
        /// Gets an absolute assembly path for a project
        /// <code>
        /// SolutionProjectResult result = ParseSolution(new FilePath("test.sln"));
        /// result.GetProjects();
        /// </code>
        /// </example>
        public static IEnumerable<SolutionProject> GetProjects(this SolutionParserResult projects)
        {
            return projects.Projects.Where(x => !IsSolutionFolder(x));
        }

        /// <summary>
        /// Gets the output assembly path for a SolutionProject
        /// </summary>
        /// <param name="solutionProject">The solutionproject</param>
        /// <param name="project">The parsed project</param>
        /// <returns>The SolutionProject output assembly path</returns>
        /// <example>
        /// Gets an absolute assembly path for a project
        /// <code>
        /// var projects = ParseSolution(new FilePath("test.sln")).GetProjects();
        /// project[0].GetAssemblyFilePath(); 
        /// </code>
        /// </example>
        public static FilePath GetAssemblyFilePath(this SolutionProject solutionProject, CustomProjectParserResult project)
        {
            solutionProject.ThrowIfNull(nameof(solutionProject));
            project.ThrowIfNull(nameof(project));

            var assemblyFilePath = project.GetAssemblyFilePath();
            return solutionProject.Path.GetDirectory().CombineWithFilePath(assemblyFilePath);
        }

        internal static readonly IReadOnlyDictionary<ProjectType, string> Types = new Dictionary<ProjectType, string>
        {
            { ProjectType.AspNetMvc1, ProjectTypes.AspNetMvc1 },
            { ProjectType.AspNetMvc2, ProjectTypes.AspNetMvc2 },
            { ProjectType.AspNetMvc3, ProjectTypes.AspNetMvc3 },
            { ProjectType.AspNetMvc4, ProjectTypes.AspNetMvc4 },
            { ProjectType.AspNetMvc5, ProjectTypes.AspNetMvc5 },
            { ProjectType.CPlusplus, ProjectTypes.CPlusplus },
            { ProjectType.CSharp, ProjectTypes.CSharp },
            { ProjectType.Database, ProjectTypes.Database },
            { ProjectType.DatabaseOther, ProjectTypes.DatabaseOther },
            { ProjectType.DeploymentCab, ProjectTypes.DeploymentCab },
            { ProjectType.DeploymentMergeModule, ProjectTypes.DeploymentMergeModule },
            { ProjectType.DeploymentSetup, ProjectTypes.DeploymentSetup },
            { ProjectType.DeploymentSmartDeviceCab, ProjectTypes.DeploymentSmartDeviceCab },
            { ProjectType.DotNetCore, ProjectTypes.DotNetCore },
            { ProjectType.DistributedSystem, ProjectTypes.DistributedSystem },
            { ProjectType.Dynamics2012AxCsharpInAot, ProjectTypes.Dynamics2012AxCsharpInAot },
            { ProjectType.FSharp, ProjectTypes.FSharp },
            { ProjectType.JSharp, ProjectTypes.JSharp },
            { ProjectType.Legacy2003SmartDeviceCSharp, ProjectTypes.Legacy2003SmartDeviceCSharp },
            { ProjectType.Legacy2003SmartDeviceVbNet, ProjectTypes.Legacy2003SmartDeviceVbNet },
            { ProjectType.ModelViewControllerV2Mvc2, ProjectTypes.ModelViewControllerV2Mvc2 },
            { ProjectType.ModelViewControllerV3Mvc3, ProjectTypes.ModelViewControllerV3Mvc3 },
            { ProjectType.ModelViewControllerV4Mvc4, ProjectTypes.ModelViewControllerV4Mvc4 },
            { ProjectType.ModelViewControllerV5Mvc5, ProjectTypes.ModelViewControllerV5Mvc5 },
            { ProjectType.MonoForAndroid, ProjectTypes.MonoForAndroid },
            { ProjectType.Monotouch, ProjectTypes.Monotouch },
            { ProjectType.MonotouchBinding, ProjectTypes.MonotouchBinding },
            { ProjectType.PortableClassLibrary, ProjectTypes.PortableClassLibrary },
            { ProjectType.ProjectFolders, ProjectTypes.ProjectFolders },
            { ProjectType.ServiceFabricApplication, ProjectTypes.ServiceFabricApplication },
            { ProjectType.SharepointCSharp, ProjectTypes.SharepointCSharp },
            { ProjectType.SharepointVbNet, ProjectTypes.SharepointVbNet },
            { ProjectType.SharepointWorkflow, ProjectTypes.SharepointWorkflow },
            { ProjectType.Silverlight, ProjectTypes.Silverlight },
            { ProjectType.SmartDeviceCSharp, ProjectTypes.SmartDeviceCSharp },
            { ProjectType.SmartDeviceVbNet, ProjectTypes.SmartDeviceVbNet },
            { ProjectType.SolutionFolder, ProjectTypes.SolutionFolder },
            { ProjectType.Test, ProjectTypes.Test },
            { ProjectType.VbNet, ProjectTypes.VbNet },
            { ProjectType.VisualDatabaseTools, ProjectTypes.VisualDatabaseTools },
            { ProjectType.VisualStudioToolsForApplicationsVsta, ProjectTypes.VisualStudioToolsForApplicationsVsta },
            { ProjectType.VisualStudioToolsForOfficeVsto, ProjectTypes.VisualStudioToolsForOfficeVsto },
            { ProjectType.WebApplication, ProjectTypes.WebApplication },
            { ProjectType.WebSite, ProjectTypes.WebSite },
            { ProjectType.WindowsCSharp, ProjectTypes.WindowsCSharp },
            { ProjectType.WindowsCommunicationFoundation, ProjectTypes.WindowsCommunicationFoundation },
            { ProjectType.WindowsPhone881AppCSharp, ProjectTypes.WindowsPhone881AppCSharp },
            { ProjectType.WindowsPhone881AppVbNet, ProjectTypes.WindowsPhone881AppVbNet },
            { ProjectType.WindowsPhone881BlankHubWebviewApp, ProjectTypes.WindowsPhone881BlankHubWebviewApp },
            { ProjectType.WindowsPresentationFoundation, ProjectTypes.WindowsPresentationFoundation },
            { ProjectType.WindowsStoreMetroAppsComponents, ProjectTypes.WindowsStoreMetroAppsComponents },
            { ProjectType.WindowsVbNet, ProjectTypes.WindowsVbNet },
            { ProjectType.WindowsVisualCPlusplus, ProjectTypes.WindowsVisualCPlusplus },
            { ProjectType.WorkflowCSharp, ProjectTypes.WorkflowCSharp },
            { ProjectType.WorkflowFoundation, ProjectTypes.WorkflowFoundation },
            { ProjectType.WorkflowVbNet, ProjectTypes.WorkflowVbNet },
            { ProjectType.XamarinAndroid, ProjectTypes.XamarinAndroid },
            { ProjectType.XamarinIos, ProjectTypes.XamarinIos },
            { ProjectType.XnaWindows, ProjectTypes.XnaWindows },
            { ProjectType.XnaXbox, ProjectTypes.XnaXbox },
            { ProjectType.XnaZune, ProjectTypes.XnaZune }
        };
    }
}
