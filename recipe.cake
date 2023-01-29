#load nuget:?package=Cake.Recipe&version=3.0.1

Environment.SetVariableNames();

BuildParameters.SetParameters(context: Context,
                            buildSystem: BuildSystem,
                            sourceDirectoryPath: "./src",
                            title: "Cake.Incubator",
                            repositoryOwner: "cake-contrib",
                            repositoryName: "Cake.Incubator",
                            shouldRunCodecov: false,
                            shouldGenerateDocumentation: false, // until wyam oin recipe is fixed
                            appVeyorAccountName: "cakecontrib",
                            shouldRunDotNetCorePack: true,
                            preferredBuildProviderType: BuildProviderType.GitHubActions);

BuildParameters.PrintParameters(Context);

ToolSettings.SetToolPreprocessorDirectives(
                            gitVersionTool: "#tool nuget:?package=GitVersion.CommandLine&version=5.8.1",
                            gitVersionGlobalTool: "#tool dotnet:?package=GitVersion.Tool&version=5.8.1",
                            reSharperTools: "#tool nuget:?package=JetBrains.ReSharper.CommandLineTools&version=2021.2.2"
);

ToolSettings.SetToolSettings(context: Context,
                            testCoverageFilter: "+[*]* -[xunit.*]* -[Cake.Core]* -[Cake.Testing]* -[*.Tests]* -[FakeItEasy]* -[FluentAssertions]* -[FluentAssertions.Core]*",
                            testCoverageExcludeByAttribute: "*.ExcludeFromCodeCoverage*",
                            testCoverageExcludeByFile: "*/*Designer.cs;*/*.g.cs;*/*.g.i.cs");

Build.RunDotNetCore();
