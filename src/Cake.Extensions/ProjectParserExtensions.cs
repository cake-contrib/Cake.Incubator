namespace Cake.Extensions
{
    using System;
    using Cake.Common.Solution.Project;
    using Cake.Core;
    using Cake.Core.Annotations;
    using Cake.Core.IO;

    public static class ProjectParserExtensions
    {
        public static bool IsLibrary(this ProjectParserResult projectParserResult)
        {
            return projectParserResult.OutputType.Equals("Library", StringComparison.InvariantCultureIgnoreCase);
        }

        public static string GetExtension(this ProjectParserResult projectParserResult)
        {
            return projectParserResult.IsLibrary()
                ? ".dll"
                : ".exe";
        }

        public static FilePath GetAssemblyFilePath(this ProjectParserResult projectParserResult)
        {
            return
                projectParserResult.OutputPath.CombineWithFilePath(projectParserResult.AssemblyName +
                                                                   projectParserResult.GetExtension());
        }

        [CakeMethodAlias]
        public static ProjectParserResult ParseProject(this ICakeContext context, FilePath project, string configuration)
        {
            project.ThrowIfNull(nameof(project));
            configuration.ThrowIfNullOrEmpty(nameof(configuration));

            // get config specific
            var parser = new CustomProjectParser(context.FileSystem, context.Environment);
            return parser.Parse(project, configuration);
        }
    }
}