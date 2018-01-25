#load nuget:https://www.myget.org/F/cake-contrib/api/v2?package=Cake.Recipe&prerelease

Environment.SetVariableNames();

BuildParameters.SetParameters(context: Context, 
                            buildSystem: BuildSystem,
                            sourceDirectoryPath: "./src",
                            title: "Cake.Incubator",
                            repositoryOwner: "cake-contrib",
                            repositoryName: "Cake.Incubator",
							shouldRunCodecov: false,
                            appVeyorAccountName: "cakecontrib");

BuildParameters.PrintParameters(Context);

ToolSettings.SetToolSettings(context: Context,
                            dupFinderExcludePattern: new string[] { 
								BuildParameters.RootDirectoryPath + "/src/Cake.Incubator/**/*.AssemblyInfo.cs",								
                                BuildParameters.RootDirectoryPath + "/src/Cake.Incubator.Tests/*.cs",
                                BuildParameters.RootDirectoryPath + "/src/Cake.Incubator/CustomProjectParser.cs",
                                BuildParameters.RootDirectoryPath + "/src/Cake.Incubator/DotNetCoreTestExtensions.cs" },
                            testCoverageFilter: "+[*]* -[xunit.*]* -[Cake.Core]* -[Cake.Testing]* -[*.Tests]* -[FakeItEasy]* -[FluentAssertions]* -[FluentAssertions.Core]*",
                            testCoverageExcludeByAttribute: "*.ExcludeFromCodeCoverage*",
                            testCoverageExcludeByFile: "*/*Designer.cs;*/*.g.cs;*/*.g.i.cs");

Build.RunDotNetCore();
