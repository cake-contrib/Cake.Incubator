using System.IO;
using Cake.Common.Diagnostics;
using Cake.Common.IO;
using Cake.Frosting;

namespace Build.Tasks
{
    [TaskName("Setup")]
    public sealed class SetupTask : FrostingTask<BuildContext>
    {
        public override void Run(BuildContext context)
        {
            if (context.DirectoryExists(context.WorkDir))
            {
                context.DeleteDirectory(
                    context.WorkDir,
                    new DeleteDirectorySettings { Recursive = true });
            }

            context.EnsureDirectoryExists(context.WorkDir);

            File.WriteAllText(
                context.MakeAbsolute(context.SampleProject).FullPath,
                @"<Project Sdk=""Microsoft.NET.Sdk"">
  <PropertyGroup>
    <TargetFramework>net8.0</TargetFramework>
    <OutputType>Library</OutputType>
    <AssemblyName>SampleAddin</AssemblyName>
    <RootNamespace>Sample</RootNamespace>
  </PropertyGroup>
  <ItemGroup>
    <PackageReference Include=""Cake.Core"" Version=""6.0.0"" />
  </ItemGroup>
</Project>");

            context.Information("Setup complete.");
        }
    }
}
