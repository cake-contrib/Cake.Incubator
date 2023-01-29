// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/. 

namespace Cake.Incubator.XDocumentExtensions
{
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Linq;
    using System.Xml.Linq;
    using Cake.Common.Solution.Project;
    using Cake.Core.IO;
    using Cake.Incubator.EnumerableExtensions;
    using Cake.Incubator.Project;
    using Cake.Incubator.StringExtensions;
    using Cake.Incubator.XElementExtensions;

    /// <summary>
    /// Several extension methods when using XDocument.
    /// </summary>
    internal static class XDocumentExtensions
    {
        /// <summary>
        /// Gets the first output path value for a specific config from an xml document
        /// </summary>
        /// <param name="document">The xml document</param>
        /// <param name="config">the configuration</param>
        /// <param name="rootDirectoryPath">the root directory for any relative assembly paths</param>
        /// <param name="platform">the platform</param>
        /// <param name="targetFrameworks">the target frameworks expected (affects output paths)</param>
        /// <returns>the output path</returns>
        internal static DirectoryPath[] GetOutputPaths(this XDocument document, string config, string[] targetFrameworks, DirectoryPath rootDirectoryPath, string platform = "AnyCPU")
        {
            var outputPathOverride = document.Descendants("OutputPath")
                .FirstOrDefault(x =>
                {
                    var condition = x.Parent?.Attribute("Condition")?.Value;
                    if (AssertExtensions.AssertExtensions.IsNullOrEmpty(condition) || !condition.HasConfigPlatformCondition()) return false;
                    return condition.GetConditionalConfigPlatform().EqualsIgnoreCase($"{config}|{platform}");
                })?.Value;

            var shouldAppendTargetFramework = !targetFrameworks.IsNullOrEmpty() && (document.GetFirstElementValue(ProjectXElement.AppendTargetFrameworkToOutputPath) ?? "true") == "true";

            // specific output path is specified in project, overrides convention
            if (!string.IsNullOrWhiteSpace(outputPathOverride))
            {
                return shouldAppendTargetFramework
                    ? targetFrameworks.Select(x => rootDirectoryPath.Combine(outputPathOverride).Combine(x)).ToArray()
                    : new[] {rootDirectoryPath.Combine(outputPathOverride)};
            }

            // use conventions, skip null or empty props
            var template = "bin/";
            if (!AssertExtensions.AssertExtensions.IsNullOrEmpty(platform) && !platform.EqualsIgnoreCase("AnyCPU")) template += $"{platform}/";
            if (!AssertExtensions.AssertExtensions.IsNullOrEmpty(config)) template += $"{config}/";

            return shouldAppendTargetFramework
                ? targetFrameworks.Select(x => rootDirectoryPath.Combine(template).Combine(x)).ToArray()
                : new[] { rootDirectoryPath.Combine(template) };
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

            if (AssertExtensions.AssertExtensions.IsNullOrEmpty(prefix) || AssertExtensions.AssertExtensions.IsNullOrEmpty(suffix)) return version ?? "1.0.0";

            return char.IsDigit(suffix[0]) ? $"{prefix}.{suffix}" : $"{prefix}-{suffix}";
        }

        /// <summary>
        /// gets the first matching element value, if a config is passed, it will only match an element with the specified config and platform condition.
        /// the platform defaults to AnyCPU
        /// </summary>
        /// <param name="document">the document</param>
        /// <param name="elementName">the element name to match</param>
        /// <param name="config">the configuration to match</param>
        /// <param name="platform">the platform to match, default is AnyCPU</param>
        /// <returns>the matching element value if found</returns>
        internal static string GetFirstElementValue(this XDocument document, XName elementName, string config = null, string platform = "AnyCPU")
        {
            var elements = document.Descendants(elementName);
            if (!elements.Any()) return null;

            // if no config specified, return first value without config condition
            if (AssertExtensions.AssertExtensions.IsNullOrEmpty(config)) return elements.FirstOrDefault(x => !x.WithConfigCondition())?.Value;

            // next will look to match the config|platform condition of an element or it's parent, if that fails, 
            // will fallback to grab the first matching value without a condition on the element or it's parent.
            return elements.FirstOrDefault(x => x.WithConfigCondition(config, platform))
                       ?.Value ?? elements.FirstOrDefault(x => !x.WithConfigCondition())?.Value;
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

        internal static XName GetXNameWithNamespace(this XNamespace ns, string elementName)
        {
            var nsName = ns?.NamespaceName;
            return nsName == null ? XName.Get(elementName) : XName.Get(elementName, nsName);
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

            // if we are querying a pre-2017 csproj file, we need the namespace in the xname queries
            var ns = document.Root?.Name.Namespace;
            var packageReferenceXName = ns.GetXNameWithNamespace(ProjectXElement.PackageReference);
            var privateAssetsXName = ns.GetXNameWithNamespace(ProjectXElement.PrivateAssets);
            var includeAssetsXName = ns.GetXNameWithNamespace(ProjectXElement.IncludeAssets);
            var excludeAssetsXName = ns.GetXNameWithNamespace(ProjectXElement.ExcludeAssets);
            var includeXName = ns.GetXNameWithNamespace("Include");
            var versionXName = ns.GetXNameWithNamespace("Version");
            
            // get default/empty reference
            var global = document
                .Descendants(packageReferenceXName)
                .LastOrDefault(x => x.Element(includeXName)?.Value == null || x.GetAttributeValue("Include") == null);
            
            string[] globalPrivate = null;
            string[] globalInclude = null;    
            string[] globalExclude = null;
            
            if (global != null)
            {
                globalPrivate = global.GetAttributeValue(ProjectXElement.PrivateAssets)?.SplitIgnoreEmpty(';') ??
                                global.Element(privateAssetsXName)?.Value.SplitIgnoreEmpty(';');
                globalInclude = global.GetAttributeValue(ProjectXElement.IncludeAssets)?.SplitIgnoreEmpty(';') ??
                                global.Element(includeAssetsXName)?.Value.SplitIgnoreEmpty(';');
                globalExclude = global.GetAttributeValue(ProjectXElement.ExcludeAssets)?.SplitIgnoreEmpty(';') ??
                                global.Element(excludeAssetsXName)?.Value.SplitIgnoreEmpty(';');
            }

            return document.Descendants(packageReferenceXName).Select(
                x =>
                {
                    var privateAssets = x.Element(privateAssetsXName)?.Value.SplitIgnoreEmpty(';') ?? globalPrivate;
                    var includeAssets = x.Element(includeAssetsXName)?.Value.SplitIgnoreEmpty(';') ?? globalInclude;
                    var excludeAssets = x.Element(excludeAssetsXName)?.Value.SplitIgnoreEmpty(';') ?? globalExclude;
                    var condition = x.GetAttributeValue("Condition") ?? x.Parent.GetAttributeValue("Condition");
                    return new PackageReference
                    {
                        Name = x.GetAttributeValue("Include") ?? x.Element(includeXName)?.Value,
                        Version = x.GetAttributeValue("Version") ?? x.Element(versionXName)?.Value,
                        PrivateAssets = x.GetAttributeValue(ProjectXElement.PrivateAssets)?.SplitIgnoreEmpty(';') ?? privateAssets,
                        IncludeAssets = x.GetAttributeValue(ProjectXElement.IncludeAssets)?.SplitIgnoreEmpty(';') ?? includeAssets,
                        ExcludeAssets = x.GetAttributeValue(ProjectXElement.ExcludeAssets)?.SplitIgnoreEmpty(';') ?? excludeAssets,
                        TargetFramework = condition.HasTargetFrameworkCondition()
                            ? condition.GetConditionTargetFramework()
                            : null
                    };
                }).ToArray();
        }
        
        internal static ICollection<ProjectAssemblyReference> GetAssemblyReferences(this XDocument document, DirectoryPath rootPath)
        {
            /*
                <Reference Include="Cake.Common, Version=0.22.0.0, Culture=neutral, PublicKeyToken=null">
                  <HintPath>..\.nuget\packages\cake.common\0.22.0\lib\netstandard1.6\Cake.Common.dll</HintPath>
                </Reference>
             */

            // if we are querying a pre-2017 csproj file, we need the namespace in the xname queries
            var ns = document.Root?.Name.Namespace;
            var referenceXName = ns.GetXNameWithNamespace(ProjectXElement.Reference);
            var includeXName = ns.GetXNameWithNamespace(ProjectXElement.Include);
            var hintPathXName = ns.GetXNameWithNamespace(ProjectXElement.HintPath);
            var nameXName = ns.GetXNameWithNamespace(ProjectXElement.Name);
            var fusionXName = ns.GetXNameWithNamespace(ProjectXElement.FusionName);
            var specificVersionXName = ns.GetXNameWithNamespace(ProjectXElement.SpecificVersion);
            var aliasesXName = ns.GetXNameWithNamespace(ProjectXElement.Aliases);
            var privateXName = ns.GetXNameWithNamespace(ProjectXElement.Private);
            
            return document.Descendants(referenceXName).Select(
                x =>
                {
                    var condition = x.GetAttributeValue("Condition") ?? x.Parent.GetAttributeValue("Condition");
                    var include = x.GetAttributeValue(ProjectXElement.Include) ?? x.Element(includeXName)?.Value;
                    var hintPath = x.GetAttributeValue(ProjectXElement.HintPath) ?? x.Element(hintPathXName)?.Value;
                    var name = x.GetAttributeValue(ProjectXElement.Name) ?? x.Element(nameXName)?.Value;
                    var fusionName = x.GetAttributeValue(ProjectXElement.FusionName) ?? x.Element(fusionXName)?.Value;
                    var specificVersion = x.GetAttributeValue(ProjectXElement.SpecificVersion) ??
                                          x.Element(specificVersionXName)?.Value;
                    var aliases = x.GetAttributeValue(ProjectXElement.Aliases) ?? x.Element(aliasesXName)?.Value;
                    var privateFlag = x.GetAttributeValue(ProjectXElement.Aliases) ?? x.Element(privateXName)?.Value;
                    
                    // TODO Can't set the target framework condition without changing the return type, no where to put it.
                    var targetFramework = condition.HasTargetFrameworkCondition()
                        ? condition.GetConditionTargetFramework()
                        : null;

                    return new ProjectAssemblyReference
                    {
                        Include = include,
                        HintPath = string.IsNullOrWhiteSpace(hintPath) ? null : hintPath.GetAbsolutePath(rootPath),
                        Name = name ?? include?.Split(',')?.FirstOrDefault(),
                        FusionName = fusionName,
                        SpecificVersion = specificVersion == null ? (bool?) null : bool.Parse(specificVersion),
                        Aliases = aliases,
                        Private = privateFlag == null ? (bool?) null : bool.Parse(privateFlag),
                    };
                }).Distinct(x => x.Name).ToArray();
        }

        internal static ICollection<ProjectReference> GetProjectReferences(this XDocument document, DirectoryPath rootPath)
        {
            return document.Descendants(ProjectXElement.ProjectReference).Select(x =>
            {
                var value = new FilePath(x.GetAttributeValue("Include"));
                return new ProjectReference
                {
                    Name = value.GetFilenameWithoutExtension().ToString(),
                    RelativePath = value.IsRelative ? value.ToString() : value.GetRelativePath(rootPath).ToString(),
                    FilePath = value.IsRelative ? value.MakeAbsolute(rootPath) : value.ToString(),
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
                    BeforeTargets = x.GetAttributeValue("BeforeTargets")?.SplitIgnoreEmpty(';'),
                    AfterTargets = x.GetAttributeValue("AfterTargets")?.SplitIgnoreEmpty(';'),
                    DependsOn = x.GetAttributeValue("DependsOn")?.SplitIgnoreEmpty(';'),
                    Executables = x.Elements("Exec").Select(exec =>
                        new BuildTargetExecutable
                        {
                            Command = exec.GetAttributeValue("Command")
                        }).ToArray()
                }).ToArray();
        }

        internal static NameValueCollection GetNuspecProps(this XDocument document)
        {
            var nuspecProps = document.GetFirstElementValue(ProjectXElement.NuspecProperties).SplitIgnoreEmpty(';');
            var nuspecProperties = new NameValueCollection(nuspecProps.Length);
            foreach (var prop in nuspecProps)
            {
                var pair = prop.Split('=');
                nuspecProperties.Add(pair[0], pair[1]);
            }
            return nuspecProperties;
        }
    }
}