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
    public static class XDocumentExtensions
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
        public static DirectoryPath[] GetOutputPaths(this XDocument document, string config, string[] targetFrameworks, DirectoryPath rootDirectoryPath, string platform = "AnyCPU")
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
        public static bool IsDotNetSdk(this XDocument document)
        {
            return document.GetSdk() != null;
        }

        /// <summary>
        /// Returns the SDK identifier from the project's root element, supporting both
        /// the &lt;Project Sdk="..."&gt; attribute form and the &lt;Project&gt;&lt;Sdk Name="..." /&gt;&lt;/Project&gt;
        /// child-element form (per #267 — common in monorepos that hide reusable SDK
        /// configuration in Directory.Build.props).
        /// </summary>
        /// <param name="document">the document</param>
        /// <returns>the SDK identifier or null if no Sdk attribute or element is set</returns>
        public static string GetSdk(this XDocument document)
        {
            var attribute = document.Root?.Attribute("Sdk", true)?.Value;
            if (attribute != null)
            {
                return attribute;
            }

            // <Project>
            //   <Sdk Name="Microsoft.NET.Sdk" />
            // </Project>
            var sdkElement = document.Root?
                .Elements()
                .FirstOrDefault(e => e.Name.LocalName.EqualsIgnoreCase("Sdk"));
            return sdkElement?.GetAttributeValue("Name");
        }

        /// <summary>
        /// Returns the project version, combining VersionPrefix and VersionSuffix when both are
        /// present, falling back to Version, and ultimately "1.0.0" when nothing is set.
        /// </summary>
        /// <param name="document">the document</param>
        /// <returns>the resolved version string</returns>
        public static string GetVersion(this XDocument document)
        {
            // prefix and suffix take precidence, fallback is version if both are not specified
            var prefix = document.GetFirstElementValue(ProjectXElement.VersionPrefix);
            var suffix = document.GetFirstElementValue(ProjectXElement.VersionSuffix);
            var version = document.GetFirstElementValue(ProjectXElement.Version);

            if (AssertExtensions.AssertExtensions.IsNullOrEmpty(prefix) || AssertExtensions.AssertExtensions.IsNullOrEmpty(suffix)) return version ?? "1.0.0";

            return char.IsDigit(suffix[0]) ? $"{prefix}.{suffix}" : $"{prefix}-{suffix}";
        }

        /// <summary>
        /// Gets the first matching element.
        /// If a config is passed, it will only match an element with the specified config and platform condition.
        /// The platform defaults to AnyCPU.
        /// </summary>
        /// <param name="document">the document</param>
        /// <param name="elementName">the element name to match</param>
        /// <param name="config">the configuration to match</param>
        /// <param name="platform">the platform to match, default is AnyCPU</param>
        /// <returns>the matching element if found</returns>
        public static XElement GetFirstElement(this XDocument document, XName elementName, string config = null, string platform = "AnyCPU")
        {
            var elements = document.Descendants(elementName);
            if (!elements.Any()) return null;

            XElement element = null;

            if (AssertExtensions.AssertExtensions.IsNullOrEmpty(config))
            {
                // No config specified. Get the first element without config condition
                element = elements.FirstOrDefault((XElement x) => !x.WithConfigCondition());
            }
            else
            {
                // Config specified. Get the first element with matching config and platform condition, or fallback to element without config condition
                element = elements.FirstOrDefault((XElement x) => x.WithConfigCondition(config, platform)) ?? elements.FirstOrDefault((XElement x) => !x.WithConfigCondition());
            }

            return element;
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
        public static string GetFirstElementValue(this XDocument document, XName elementName, string config = null, string platform = "AnyCPU")
        {
            return document.GetFirstElement(elementName, config, platform)?.Value;
        }

        /// <summary>
        /// Sets the value of the first element with the specified name in the XML document, using optional
        /// configuration and platform filters.
        /// </summary>
        /// <remarks>If no element matching the specified criteria is found, the method returns false and
        /// no changes are made to the document.</remarks>
        /// <param name="document">The XML document to search for the element. Cannot be null.</param>
        /// <param name="elementName">The name of the element whose value is to be set. Cannot be null.</param>
        /// <param name="newValue">The new value to assign to the first matching element.</param>
        /// <param name="config">An optional configuration name used to filter the search for the element. If null, no configuration
        /// filtering is applied.</param>
        /// <param name="platform">An optional platform name used to filter the search for the element. Defaults to "AnyCPU" if not specified.</param>
        /// <returns>true if the value of the first matching element was set; otherwise, false.</returns>
        public static bool SetFirstElementValue(this XDocument document, XName elementName, string newValue, string config = null, string platform = "AnyCPU")
        {
            var element = document.GetFirstElement(elementName, config, platform);
            if (element == null) return false;

            element.SetValue(newValue);
            return true;
        }

        /// <summary>
        /// Removes the first element with the specified name from the XML document, using optional configuration and
        /// platform filters.
        /// </summary>
        /// <remarks>This method only removes the first matching element. If no element matches the
        /// specified name, configuration, and platform, the document remains unchanged.</remarks>
        /// <param name="document">The XML document from which to remove the element. Cannot be null.</param>
        /// <param name="elementName">The name of the element to remove. Cannot be null.</param>
        /// <param name="config">An optional configuration value used to filter the element selection. If null, no configuration filtering is
        /// applied.</param>
        /// <param name="platform">An optional platform value used to filter the element selection. Defaults to "AnyCPU" if not specified.</param>
        /// <returns>true if an element matching the specified criteria was found and removed; otherwise, false.</returns>
        public static bool RemoveElement(this XDocument document, XName elementName, string config = null, string platform = "AnyCPU")
        {
            var element = document.GetFirstElement(elementName, config, platform);
            if (element == null) return false;

            element.Remove();
            return true;
        }

        /// <summary>
        /// Returns all &lt;DotNetCliToolReference&gt; entries declared in the project.
        /// </summary>
        /// <param name="document">the document</param>
        /// <returns>the collection of CLI tool references (empty array if none)</returns>
        public static ICollection<DotNetCliToolReference> GetDotNetCliToolReferences(this XDocument document)
        {
            return document.Descendants(ProjectXElement.DotNetCliToolReference).Select(x =>
                new DotNetCliToolReference
                {
                    Name = x.GetAttributeValue("Include"),
                    Version = x.GetAttributeValue("Version")
                }).ToArray();
        }

        /// <summary>
        /// Combines an XNamespace with an element name into an XName, handling the case where
        /// the namespace is null (pre-VS2017 csproj files use a default namespace; modern
        /// SDK-style projects don't).
        /// </summary>
        /// <param name="ns">the namespace (or null)</param>
        /// <param name="elementName">the element name</param>
        /// <returns>an XName scoped to the namespace when provided</returns>
        public static XName GetXNameWithNamespace(this XNamespace ns, string elementName)
        {
            var nsName = ns?.NamespaceName;
            return nsName == null ? XName.Get(elementName) : XName.Get(elementName, nsName);
        }

        /// <summary>
        /// Returns all &lt;PackageReference&gt; entries declared in the project, resolving
        /// PrivateAssets / IncludeAssets / ExcludeAssets attributes or child elements and any
        /// TargetFramework condition on the element or its parent ItemGroup.
        /// </summary>
        /// <param name="document">the document</param>
        /// <returns>the collection of parsed package references</returns>
        public static ICollection<ParsedPackageReference> GetPackageReferences(this XDocument document)
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
                    return new ParsedPackageReference
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
        
        /// <summary>
        /// Returns all &lt;Reference&gt; entries declared in the project, resolving HintPath
        /// values relative to <paramref name="rootPath"/>.
        /// </summary>
        /// <param name="document">the document</param>
        /// <param name="rootPath">the project root directory used for HintPath resolution</param>
        /// <returns>the collection of assembly references</returns>
        public static ICollection<ProjectAssemblyReference> GetAssemblyReferences(this XDocument document, DirectoryPath rootPath)
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

        /// <summary>
        /// Returns all &lt;ProjectReference&gt; entries, resolving Include paths relative to
        /// <paramref name="rootPath"/>.
        /// </summary>
        /// <param name="document">the document</param>
        /// <param name="rootPath">the project root directory used for path resolution</param>
        /// <returns>the collection of project references</returns>
        public static ICollection<ProjectReference> GetProjectReferences(this XDocument document, DirectoryPath rootPath)
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

        /// <summary>
        /// Returns all &lt;Target&gt; build targets declared in the project (with their
        /// BeforeTargets / AfterTargets attributes and nested &lt;Exec&gt; commands).
        /// </summary>
        /// <param name="document">the document</param>
        /// <returns>the collection of build targets</returns>
        public static ICollection<BuildTarget> GetTargets(this XDocument document)
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

        /// <summary>
        /// Returns the project's nuspec-relevant property values (PackageId, Title, Authors,
        /// Description, Copyright, etc.) keyed by element name.
        /// </summary>
        /// <param name="document">the document</param>
        /// <returns>a name-value collection of nuspec property values</returns>
        public static NameValueCollection GetNuspecProps(this XDocument document)
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