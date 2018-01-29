namespace Cake.Incubator.Test
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Common.Tools.DotNetCore;
    using Common.Tools.XUnit;
    using Core;
    using Core.Diagnostics;
    using Core.IO;
    using Core.Tooling;

    /// <summary>.NET Core xunit project tester.</summary>
    public class DotNetCoreXUnitTester : DotNetCoreTool<DotNetCoreXUnitSettings>
    {
        private readonly ICakeEnvironment environment;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Cake.Incubator.DotNetCoreXUnitTester" /> class.
        /// </summary>
        /// <param name="fileSystem">The file system.</param>
        /// <param name="environment">The environment.</param>
        /// <param name="processRunner">The process runner.</param>
        /// <param name="tools">The tool locator.</param>
        public DotNetCoreXUnitTester(IFileSystem fileSystem, Core.ICakeEnvironment environment, IProcessRunner processRunner, IToolLocator tools) : base(fileSystem, environment, processRunner, tools)
        {
            this.environment = environment;
        }

        public void Test(FilePath[] projectFilePaths, DotNetCoreXUnitSettings dotNetCoreXUnitSettings)
        {
            dotNetCoreXUnitSettings.ThrowIfNull(nameof(dotNetCoreXUnitSettings));
            if (string.IsNullOrWhiteSpace(dotNetCoreXUnitSettings.OutputDirectory?.FullPath))
            {
                if (dotNetCoreXUnitSettings.NetFrameworkOptions.HtmlReport)
                    throw new CakeException("Cannot generate HTML report when no output directory has been set.");
                if (dotNetCoreXUnitSettings.NetFrameworkOptions.NUnitReport)
                    throw new CakeException("Cannot generate HTML report when no output directory has been set.");
                if (dotNetCoreXUnitSettings.XmlReport || dotNetCoreXUnitSettings.NetFrameworkOptions.XmlReportV1)
                    throw new CakeException("Cannot generate XML report when no output directory has been set.");
            }
            this.RunCommand(dotNetCoreXUnitSettings, GetArguments(projectFilePaths, dotNetCoreXUnitSettings));
        }

        private ProcessArgumentBuilder GetArguments(IEnumerable<FilePath> projectFilePaths,
            DotNetCoreXUnitSettings settings)
        {
            var builder = this.CreateArgumentBuilder(settings);

            builder.Append("xunit");

            projectFilePaths.Each(x => builder.AppendQuoted(x.MakeAbsolute(this.environment).FullPath));

            if (settings.Debug) builder.Append("-debug");
            if (settings.Diagnostics) builder.Append("-diagnotics");
            if (settings.InternalDiagnostics) builder.Append("-internaldiagnostics");
            if (settings.FailSkippedTests) builder.Append("-failskipped");
            if (settings.NoAutoReporters) builder.Append("-noautoreporters");
            if (settings.NoBuild) builder.Append("-nobuild");
            if (settings.NoColor) builder.Append("-nocolor");
            if (settings.NoLogo) builder.Append("-nologo");
            if (settings.Serialize) builder.Append("-serialize");
            if (settings.StopOnFail) builder.Append("-stoponfail");
            if (settings.UseMSBuild) builder.Append("-usemsbuild");
            if (settings.MSBuildVerbosity != Verbosity.Quiet)
                builder.Append($"-msbuildverbosity {settings.MSBuildVerbosity}");
            if (settings.Wait) builder.Append("-wait");
            if (!settings.NetFrameworkOptions.ShadowCopy) builder.Append("-noshadow");
            if (settings.NetFrameworkOptions.NoAppDomain) builder.Append("-noappdomain");
            if (settings.NetFrameworkOptions.UseX86) builder.Append("-x86");
            if (settings.Parallelism != ParallelismOption.None) builder.Append($"-parallel {settings.Parallelism.ToString().ToLowerInvariant()}");
            if (settings.MaxThreads.HasValue)
            {
                builder.Append(settings.MaxThreads.Value == 0
                    ? "-maxthreads unlimited"
                    : $"-maxthreads {settings.MaxThreads.Value}");
            }

            foreach (var data in settings.TraitsToInclude.SelectMany(pair => pair.Value.Select(v => new
            {
                Name = pair.Key,
                Value = v
            })))
            {
                builder.Append($"-trait \"{data.Name}={data.Value}\"");
            }

            foreach (var data in settings.TraitsToExclude.SelectMany(pair => pair.Value.Select(v => new
            {
                Name = pair.Key,
                Value = v
            })))
            {
                builder.Append($"-notrait \"{data.Name}={data.Value}\"");
            }

            if (!settings.MethodsToRun.IsNullOrEmpty())
                settings.MethodsToRun.Each(x =>
                {
                    builder.Append("-class");
                    builder.AppendQuoted(x);
                });

            if (!settings.ClassesToRun.IsNullOrEmpty())
                settings.ClassesToRun.Each(x =>
                {
                    builder.Append("-class");
                    builder.AppendQuoted(x);
                });

            if (!settings.NamespacesToRun.IsNullOrEmpty())
                settings.NamespacesToRun.Each(x =>
                {
                    builder.Append("-class");
                    builder.AppendQuoted(x);
                });


            if (!settings.TargetFramework.IsNullOrEmpty()) builder.Append($"-framework {settings.Configuration}");
            if (!settings.Configuration.IsNullOrEmpty()) builder.Append($"-configuration {settings.Configuration}");
            if (!settings.NetCoreOptions.NetCoreFrameworkVersion.IsNullOrEmpty()) builder.Append($"-fxversion {settings.NetCoreOptions.NetCoreFrameworkVersion}");

            // reporting
            var outputDir = settings.OutputDirectory.MakeAbsolute(environment);
            var reportName = settings.ReportName;

            if (settings.XmlReport)
            {
                builder.Append("-xml");
                builder.AppendQuoted($"{outputDir}{reportName}.xml");
            }

            if (settings.NetFrameworkOptions.NUnitReport)
            {
                builder.Append("-nunit");
                builder.AppendQuoted($"{outputDir}{reportName}.nunit.xml");
            }

            if (settings.NetFrameworkOptions.XmlReportV1)
            {
                builder.Append("-xmlv1");
                builder.AppendQuoted($"{outputDir}{reportName}.xunitV1.xml");
            }

            if (settings.NetFrameworkOptions.HtmlReport)
            {
                builder.Append("-html");
                builder.AppendQuoted($"{outputDir}{reportName}.html");
            }

            if (settings.Reporter != XUnitReporter.None)
            {
                switch (settings.Reporter)
                {
                    case XUnitReporter.AppVeyor:
                        builder.Append("-appveyor");
                        break;
                    case XUnitReporter.Json:
                        builder.Append("-json");
                        break;
                    case XUnitReporter.Quiet:
                        builder.Append("-quiet");
                        break;
                    case XUnitReporter.Teamcity:
                        builder.Append("-teamcity");
                        break;
                    case XUnitReporter.Verbose:
                        builder.Append("-verbose");
                        break;
                }
                
            }

            return builder;
        }
    }
}