using Cake.Core;
using Cake.Core.IO;
using Cake.Frosting;

namespace Build
{
    public class BuildContext : FrostingContext
    {
        public DirectoryPath WorkDir { get; } = "./BuildArtifacts/temp/test-incubator-frosting";

        public FilePath SampleProject => WorkDir.CombineWithFilePath("Sample.csproj");

        public BuildContext(ICakeContext context)
            : base(context)
        {
        }
    }
}
