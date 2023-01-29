﻿namespace Cake.Incubator.DotNetTestExtensions
{
    using System.Linq;
    using Cake.Common.Tools.DotNet;
    using Cake.Common.Tools.DotNet.Test;
    using Cake.Common.Tools.XUnit;
    using Cake.Core;
    using Cake.Core.Annotations;
    using Cake.Core.IO;

    /// <summary>
    /// Several extension methods when using DotNetTest.
    /// </summary>
    [CakeAliasCategory("DotNet")]
    // ReSharper disable once UnusedMember.Global
    public static class DotNetTestExtensions
    {
        /// <summary>
        /// Runs DotNetTest using the given <see cref="XUnit2Settings"/>
        /// </summary>
        /// <param name="context">The Cake Context</param>
        /// <param name="project">DotNet Test Project File Path</param>
        /// <param name="xunitSettings">XUnit2 DotNet Test Settings Configurer</param>
        [CakeAliasCategory("Test")]
        [CakeMethodAlias]
        // ReSharper disable once UnusedMember.Global
        public static void DotNetTest(
            this ICakeContext context,
            FilePath project,
            XUnit2Settings xunitSettings)
        {
            DotNetTest(context, new DotNetTestSettings(), project, xunitSettings);
        }

        /// <summary>
        /// Appends <see cref="XUnit2Settings"/> to an <see cref="DotNetTestSettings"/> instance
        /// </summary>
        /// <param name="context">The Cake Context</param>
        /// <param name="settings">DotNet Test Settings</param>
        /// <param name="project">DotNet Test Project Path</param>
        /// <param name="xunitSettings">XUnit2 DotNet Test Settings Configurer</param>
        [CakeMethodAlias]
        [CakeAliasCategory("Test")]
        public static void DotNetTest(
            this ICakeContext context,
            DotNetTestSettings settings,
            FilePath project,
            XUnit2Settings xunitSettings)
        {
            settings.ArgumentCustomization = args => ProcessArguments(context, args, project, xunitSettings);
            context.DotNetTest(project.FullPath, settings);
        }

        private static ProcessArgumentBuilder ProcessArguments(
            ICakeContext cakeContext,
            ProcessArgumentBuilder builder,
            FilePath project,
            XUnit2Settings settings)
        {
            // No shadow copy?
            if (!settings.ShadowCopy)
            {
                throw new CakeException("-noshadow is not supported in .netcoreapp");
            }

            // No app domain?
            if (settings.NoAppDomain)
            {
                throw new CakeException("-noappdomain is not supported in .netcoreapp");
            }

            if (settings.OutputDirectory == null
                && (settings.HtmlReport || settings.NUnitReport || settings.XmlReport))
            {
                throw new CakeException("OutputDirectory must not be null");
            }

            // Generate NUnit Style XML report?
            if (settings.NUnitReport)
            {
                AddOutputArgument(builder, cakeContext, project, settings, ".xml", "-nunit");
            }

            // Generate HTML report?
            if (settings.HtmlReport)
            {
                AddOutputArgument(builder, cakeContext, project, settings, ".html", "-html");
            }

            if (settings.XmlReportV1)
            {
                throw new CakeException("-xmlv1 is not supported in .netcoreapp");
            }

            // Generate XML report?
            if (settings.XmlReport)
            {
                AddOutputArgument(builder, cakeContext, project, settings, ".xml", "-xml");
            }

            // parallelize test execution?
            if (settings.Parallelism != null && settings.Parallelism != ParallelismOption.None)
            {
                builder.Append($"-parallel {settings.Parallelism.ToString().ToLowerInvariant()}");
            }

            // max thread count for collection parallelization
            if (settings.MaxThreads.HasValue)
            {
                if (settings.MaxThreads.Value == 0)
                {
                    builder.Append("-maxthreads unlimited");
                }
                else
                {
                    builder.Append($"-maxthreads {settings.MaxThreads.Value}");
                }
            }

            foreach (var trait in settings.TraitsToInclude
                .SelectMany(pair => pair.Value.Select(v => new { Name = pair.Key, Value = v })))
            {
                builder.Append("-trait \"{0}={1}\"", trait.Name, trait.Value);
            }

            foreach (var trait in settings.TraitsToExclude
                .SelectMany(pair => pair.Value.Select(v => new { Name = pair.Key, Value = v })))
            {
                builder.Append($"-notrait \"{trait.Name}={trait.Value}\"");
            }

            return builder;
        }

        private static void AddOutputArgument(
            ProcessArgumentBuilder builder,
            ICakeContext cakeContext,
            FilePath project,
            XUnit2Settings settings,
            string fileExtension,
            string argumentName)
        {
            var reportFileName = new FilePath(project.GetDirectory().GetDirectoryName());
            var assemblyFilename = reportFileName.AppendExtension(fileExtension);
            var outputPath = settings.OutputDirectory.MakeAbsolute(cakeContext.Environment)
                .GetFilePath(assemblyFilename);

            builder.Append(argumentName);
            builder.AppendQuoted(outputPath.FullPath);
        }
    }
}