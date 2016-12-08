﻿// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/. 

namespace Cake.Extensions
{
    using System;
    using System.Linq;
    using Cake.Core;
    using Cake.Core.Annotations;
    using Cake.Core.IO;
    using Common.Diagnostics;

    public static class ProjectParserExtensions
    {
        /// <summary>
        /// Checks if the project is a library
        /// </summary>
        /// <param name="projectParserResult">the parsed project</param>
        /// <returns>true if the project is a library</returns>
        public static bool IsLibrary(this CustomProjectParserResult projectParserResult)
        {
            return projectParserResult.OutputType.Equals("Library", StringComparison.InvariantCultureIgnoreCase);
        }

        /// <summary>
        /// Returns the parsed projects output assembly extension
        /// </summary>
        /// <param name="projectParserResult">the parsed project</param>
        /// <returns>the output assemblys file extnesion</returns>
        public static string GetExtension(this CustomProjectParserResult projectParserResult)
        {
            return projectParserResult.IsLibrary()
                ? ".dll"
                : ".exe";
        }

        /// <summary>
        /// Gets a parsed projects output assenbly path
        /// </summary>
        /// <param name="projectParserResult">the parsed project</param>
        /// <returns>the output assembly path</returns>
        public static FilePath GetAssemblyFilePath(this CustomProjectParserResult projectParserResult)
        {
            return
                projectParserResult.OutputPath.CombineWithFilePath(projectParserResult.AssemblyName +
                                                                   projectParserResult.GetExtension());
        }

        /// <summary>
        /// Checks the parsed projects type
        /// </summary>
        /// <param name="projectParserResult">the parsed project</param>
        /// <param name="projectType">the project type to check</param>
        /// <returns>true if the project type matches</returns>
        public static bool IsType(this CustomProjectParserResult projectParserResult, ProjectType projectType)
        {
            if (projectType.HasFlag(ProjectType.Undefined))
                return projectParserResult.ProjectTypeGuids == null
                       || projectParserResult.ProjectTypeGuids.Length == 0;

            return projectParserResult.ProjectTypeGuids.Any(x => x.EqualsIgnoreCase(SolutionParserExtensions.Types[projectType]));
        }

        /// <summary>
        /// Parses a csproj file into a strongly typed CustomProjectParserResult object
        /// </summary>
        /// <param name="context">the cake context</param>
        /// <param name="project">the project filepath</param>
        /// <param name="configuration">the buidl configuration</param>
        /// <returns>The parsed project</returns>
        [CakeMethodAlias]
        public static CustomProjectParserResult ParseProject(this ICakeContext context, FilePath project, string configuration)
        {
            project.ThrowIfNull(nameof(project));
            configuration.ThrowIfNullOrEmpty(nameof(configuration));

            // get config specific
            var parser = new CustomProjectParser(context.FileSystem, context.Environment);
            var customProjectParserResult = parser.Parse(project, configuration);
            
            context.Debug("Parsed project file {0}\r\n{1}", project, customProjectParserResult.Dump());
            
            return customProjectParserResult;
        }
    }
}