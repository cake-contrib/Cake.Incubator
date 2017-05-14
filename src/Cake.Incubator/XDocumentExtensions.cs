// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/. 

namespace Cake.Incubator
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Xml.Linq;
    using Cake.Common.Solution.Project;
    using Cake.Core.IO;

    /// <summary>
    /// Several extension methods when using XDocument.
    /// </summary>
    public static class XDocumentExtensions
    {
        /// <summary>
        /// Gets the first output path value for a specific config from an xml document
        /// </summary>
        /// <param name="document">The xml document</param>
        /// <param name="config">the configuration</param>
        /// <param name="platform">the platform</param>
        /// <returns>the output path</returns>
        internal static string GetOutputPath(this XDocument document, string config, string platform = "AnyCPU")
        {
            return document.Descendants("OutputPath")
                .FirstOrDefault(x =>
                {
                    return x.Parent != null && x.Parent.Attribute("Condition")
                               .Value
                               .EndsWith($"=='{config}|{platform}'", StringComparison.OrdinalIgnoreCase);
                })?.Value;
        }

        /// <summary>
        /// Gets the first platform target value for a specific config from an xml document
        /// </summary>
        /// <param name="document">The xml document</param>
        /// <param name="config">the configuration</param>
        /// <param name="platform">the platform</param>
        /// <returns>the platform target</returns>
        internal static string GetPlatformTarget(this XDocument document, string config, string platform = "AnyCPU")
        {
            return document.Descendants("PlatformTarget")
                .FirstOrDefault(x => x.Parent.Attribute("Condition")
                    .Value
                    .EndsWith($"=='{config}|{platform}'", StringComparison.OrdinalIgnoreCase))?.Value;
        }

        /// <summary>
        /// Gets the first targetframework value from an xml document
        /// </summary>
        /// <param name="document">The xml document</param>
        /// <returns>the target framework</returns>
        internal static string GetTargetFramework(this XDocument document)
        {
            return document.GetFirstElementValue(ProjectXElement.TargetFramework);
        }

        /// <summary>
        /// Checks if an xml document for the dot net sdk attribute
        /// </summary>
        /// <param name="document">The xml document</param>
        /// <returns>True if attribute was found</returns>
        internal static bool IsDotNetSdk(this XDocument document)
        {
            return document.GetSdk() != null;
        }

        internal static string GetSdk(this XDocument document)
        {
            return document.Root?.Attribute("Sdk", true)?.Value;
        }

        internal static string GetVersion(this XDocument document)
        {
            // prefix and suffix take precidence, fallback is version if both are not specified
            var prefix = document.GetFirstElementValue(ProjectXElement.VersionPrefix);
            var suffix = document.GetFirstElementValue(ProjectXElement.VersionSuffix);
            var version = document.GetFirstElementValue(ProjectXElement.Version);

            if (prefix.IsNullOrEmpty() || suffix.IsNullOrEmpty()) return version;

            return char.IsDigit(suffix[0]) ? $"{prefix}.{suffix}" : $"{prefix}-{suffix}";
        }

        internal static string GetFirstElementValue(this XDocument document, string elementName)
        {
            return document.Descendants(elementName).FirstOrDefault()?.Value;
        }

        internal static ICollection<DotNetCliToolReference> GetDotNetCliToolReferences(this XDocument document)
        {
            return document.Descendants(ProjectXElement.DotNetCliToolReference).Select(x =>
                new DotNetCliToolReference
                {
                    Name = x.GetAttributeValue("Include"),
                    Version = x.GetAttributeValue("Version")
                }).ToArray();
        }

        internal static ICollection<PackageReference> GetPackageReferences(this XDocument document)
        {
            // NOTE: Conflicting docs: 
            // https://docs.microsoft.com/en-us/nuget/consume-packages/package-references-in-project-files
            // https://docs.microsoft.com/en-us/dotnet/articles/core/tools/project-json-to-csproj#type-build
            /*
             * Conditions can apply at itemgroup OR package reference level
             *<ItemGroup Condition="'$(TargetFramework)'=='net451'">
              <PackageReference Include="System.Collections.Immutable" Version="1.3.1" />
            </ItemGroup>

            <ItemGroup Condition="'$(TargetFramework)'=='netstandard1.5'">
              <PackageReference Include="Newtonsoft.Json" Version="9.0.1" />
            </ItemGroup> 

            <PackageReference Include="Newtonsoft.Json" Version="9.0.1" Condition="'$(TargetFramework)' == 'net452'" />
             */
            return document.Descendants(ProjectXElement.PackageReference).Select(
                x =>
                {
                    var privateAssets = x.Element(ProjectXElement.PrivateAssets)?.Value.Split(';');
                    var includeAssets = x.Element(ProjectXElement.IncludeAssets)?.Value.Split(';');
                    var excludeAssets = x.Element(ProjectXElement.ExcludeAssets)?.Value.Split(';');
                    var condition = x.GetAttributeValue("Condition") ?? x.Parent.GetAttributeValue("Condition");
                    return new PackageReference
                    {
                        Name = x.GetAttributeValue("Include"),
                        Version = x.GetAttributeValue("Version"),
                        PrivateAssets = x.GetAttributeValue(ProjectXElement.PrivateAssets)?.Split(';') ?? privateAssets,
                        IncludeAssets = x.GetAttributeValue(ProjectXElement.IncludeAssets)?.Split(';') ?? includeAssets,
                        ExcludeAssets = x.GetAttributeValue(ProjectXElement.ExcludeAssets)?.Split(';') ?? excludeAssets,
                        TargetFramework = condition.HasTargetFrameworkCondition()
                            ? condition.GetConditionTargetFramework()
                            : null
                    };
                }).ToArray();
        }

        internal static ICollection<ProjectReference> GetProjectReferences(this XDocument document, DirectoryPath rootPath)
        {
            return document.Descendants(ProjectXElement.ProjectReference).Select(x =>
            {
                var value = x.GetAttributeValue("Include");
                return new ProjectReference
                {
                    Name = value,
                    RelativePath = value,
                    FilePath = rootPath.CombineWithFilePath(value),

                };
            }).ToArray();
        }

        internal static ICollection<BuildTarget> GetTargets(this XDocument document)
        {
            /*
             <Target Name="MyPreCompileTarget" BeforeTargets="Build">
                <Exec Command="generateCode.cmd" />
            </Target>

            <Target Name="MyPostCompileTarget" AfterTargets="Publish">
                <Exec Command="obfuscate.cmd" />
                <Exec Command="removeTempFiles.cmd" />
            </Target>
                */
            return document.Descendants(ProjectXElement.Target).Select(x =>
                new BuildTarget
                {
                    Name = x.GetAttributeValue("Name"),
                    BeforeTargets = x.GetAttributeValue("BeforeTargets")?.Split(';') ?? new string[0],
                    AfterTargets = x.GetAttributeValue("AfterTargets")?.Split(';') ?? new string[0],
                    DependsOn = x.GetAttributeValue("DependsOn")?.Split(';') ?? new string[0],
                    Executables = x.Elements("Exec").Select(exec =>
                        new BuildTargetExecutable
                        {
                            Command = exec.GetAttributeValue("Command")
                        }).ToArray()
                }).ToArray();
        }
    }
}