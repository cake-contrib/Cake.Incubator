namespace Cake.Incubator
{
    using System.Linq;
    using Cake.Common.Tools.DotNetCore;
    using Cake.Common.Tools.DotNetCore.Test;
    using Cake.Common.Tools.XUnit;
    using Cake.Core;
    using Cake.Core.Annotations;
    using Cake.Core.IO;
    using Common.Tools.DotNetCore.Restore;
    using Test;

    /// <summary>
    /// Several extension methods when using DotNetCoreTest.
    /// </summary>
    [CakeAliasCategory("DotNetCore")]
    // ReSharper disable once UnusedMember.Global
    public static class DotNetCoreTestExtensions
    {
        /// <summary>
        /// Runs DotNetCoreTest using the given <see cref="XUnit2Settings"/>
        /// </summary>
        /// <param name="context">The Cake Context</param>
        /// <param name="project">DotNetCore Test Project File Path</param>
        /// <param name="xunitSettings">XUnit2 DotNetCore Test Settings Configurer</param>
        [CakeAliasCategory("Test")]
        [CakeMethodAlias]
        // ReSharper disable once UnusedMember.Global
        public static void DotNetCoreTest(
            this ICakeContext context,
            FilePath project,
            XUnit2Settings xunitSettings)
        {
            DotNetCoreTest(context, new DotNetCoreTestSettings(), project, xunitSettings);
        }


        /// <summary>
        /// Appends <see cref="XUnit2Settings"/> to an <see cref="DotNetCoreTestSettings"/> instance
        /// </summary>
        /// <param name="context">The Cake Context</param>
        /// <param name="settings">DotNetCore Test Settings</param>
        /// <param name="project">DotNetCore Test Project Path</param>
        /// <param name="xunitSettings">XUnit2 DotNetCore Test Settings Configurer</param>
        [CakeMethodAlias]
        [CakeAliasCategory("Test")]
        public static void DotNetCoreTest(
            this ICakeContext context,            
            DotNetCoreTestSettings settings,
            FilePath project,
            XUnit2Settings xunitSettings)
        {
            settings.ArgumentCustomization = args => ProcessArguments(context, args, project, xunitSettings);
            context.DotNetCoreTest(project.FullPath, settings);
        }

        /// <summary>Tests `dotnet xunit` compatible project with settings.</summary>
        /// <param name="context">The context.</param>
        /// <param name="settings">The settings.</param>
        /// <param name="projects">The project path/s.</param>
        /// <example>
        /// <code>
        ///     var settings = new DotNetCoreXUnitSettings
        ///     {
        ///         Configuration = "Release"
        ///     };
        /// 
        ///     DotNetCoreXUnitTest(settings, "./test/TestA/TestA.csproj", "./test/TestB/TestB.csproj");
        /// </code>
        /// </example>
        [CakeMethodAlias]
        [CakeAliasCategory("Test")]
        [CakeNamespaceImport("Cake.Incubator.Test")]
        // ReSharper disable once UnusedMember.Global
        public static void DotNetCoreXUnitTest(this ICakeContext context, DotNetCoreXUnitSettings settings, params FilePath[] projects)
        {
            context.ThrowIfNull(nameof(context));
            var proj = projects.All(x =>
            {
                if (!x.IsProject()) throw new CakeException($"The projct {x.FullPath} passed to DotNetCoreXunitTest was not valid");

                var project = context.ParseProject(x, settings.Configuration);
                if (!project.IsDotNetCliTestProject()) throw new CakeException($"The project {x.FullPath} passed to DotNetCoreXUnitTest does not reference the `Microsoft.NET.Test.Sdk` package");
                if (!project.HasDotNetCliToolReference("dotnet-xunit")) throw new CakeException($"The project {x.FullPath} passed to DotNetCoreXUnitTest does not reference the `dotnet-xunit` cli tool package");
                return true;
            });

            #if NETSTANDARD1_6
            if(settings.RestoreCliTool) 
                context.DotNetCoreRestore(projects.First().MakeAbsolute(context.Environment).GetDirectory().FullPath);
            #elif NET45
                context.DotNetCoreRestore(projects.First().MakeAbsolute(context.Environment).GetDirectory().FullPath);
            #endif

            new DotNetCoreXUnitTester(context.FileSystem, context.Environment, context.ProcessRunner, context.Tools).Test(projects, settings ?? new DotNetCoreXUnitSettings());
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
                var reportFileName = new FilePath(project.GetDirectory().GetDirectoryName());                
                var assemblyFilename = reportFileName.AppendExtension(".xml");
                var outputPath = settings.OutputDirectory.MakeAbsolute(cakeContext.Environment).GetFilePath(assemblyFilename);

                builder.Append("-nunit");
                builder.AppendQuoted(outputPath.FullPath);
            }

            // Generate HTML report?
            if (settings.HtmlReport)
            {
                var reportFileName = new FilePath(project.GetDirectory().GetDirectoryName());
                var assemblyFilename = reportFileName.AppendExtension(".html");
                var outputPath = settings.OutputDirectory.MakeAbsolute(cakeContext.Environment).GetFilePath(assemblyFilename);

                builder.Append("-html");
                builder.AppendQuoted(outputPath.FullPath);
            }

            if (settings.XmlReportV1)
            {
                throw new CakeException("-xmlv1 is not supported in .netcoreapp");
            }

            // Generate XML report?
            if (settings.XmlReport)
            {
                var reportFileName = new FilePath(project.GetDirectory().GetDirectoryName());
                var assemblyFilename = reportFileName.AppendExtension(".xml");
                var outputPath = settings.OutputDirectory.MakeAbsolute(cakeContext.Environment).GetFilePath(assemblyFilename);

                builder.Append("-xml");
                builder.AppendQuoted(outputPath.FullPath);
            }

            // parallelize test execution?
            if (settings.Parallelism != ParallelismOption.None)
            {
                builder.Append("-parallel " + settings.Parallelism.ToString().ToLowerInvariant());
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
                    builder.Append("-maxthreads " + settings.MaxThreads.Value);
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
                builder.Append("-notrait \"{0}={1}\"", trait.Name, trait.Value);
            }

            return builder;
        }
    }
}
