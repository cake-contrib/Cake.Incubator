using System;
using Cake.Common.Diagnostics;
using Cake.Frosting;
using Cake.Incubator.Project;
using Cake.Incubator.StringExtensions;

namespace Build.Tasks
{
    [TaskName("Parse-Project")]
    [IsDependentOn(typeof(SetupTask))]
    public sealed class ParseProjectTask : FrostingTask<BuildContext>
    {
        public override void Run(BuildContext context)
        {
            var result = context.ParseProject(context.SampleProject, "Debug");
            AssertThat(result != null, "ParseProject: returned null");
            AssertThat(result.OutputType.EqualsIgnoreCase("Library"), "OutputType: expected Library, got " + result.OutputType);
            AssertThat(result.AssemblyName == "SampleAddin", "AssemblyName: expected SampleAddin, got " + result.AssemblyName);
            AssertThat(result.RootNameSpace == "Sample", "RootNameSpace: expected Sample, got " + result.RootNameSpace);
            context.Information("ParseProject OK (OutputType={0}, AssemblyName={1}, RootNameSpace={2})",
                result.OutputType, result.AssemblyName, result.RootNameSpace);
        }

        private static void AssertThat(bool condition, string message)
        {
            if (!condition)
            {
                throw new Exception("Assertion failed: " + message);
            }
        }
    }
}
