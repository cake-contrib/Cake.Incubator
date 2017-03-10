namespace Cake.Incubator
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Xml.Linq;

    public static class XDocumentExtensions
    {
        public static string GetOutputPath(this XDocument document, string config, string platform = "AnyCPU")
        {
            return document.Descendants("OutputPath")
                .FirstOrDefault(x => x.Parent.Attribute("Condition")
                    .Value
                    .EndsWith($"=='{config}|{platform}'", StringComparison.OrdinalIgnoreCase))?.Value;
        }

        public static string GetPlatformTarget(this XDocument document, string config, string platform = "AnyCPU")
        {
            return document.Descendants("PlatformTarget")
                .FirstOrDefault(x => x.Parent.Attribute("Condition")
                    .Value
                    .EndsWith($"=='{config}|{platform}'", StringComparison.OrdinalIgnoreCase))?.Value;
        }

        public static string GetTargetFramework(this XDocument document)
        {
            return document.Descendants(ProjectXElement.TargetFramework).FirstOrDefault()?.Value;
        }

        public static bool IsDotNetSdk(this XDocument document)
        {
            return document.Root?.Attribute("Sdk")?.Value != null;
        }

        public static IEnumerable<TSource> Distinct<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> getKey)
        {
            var dictionary = new HashSet<TKey>();

            foreach (var item in source)
            {
                var key = getKey(item);
                if (dictionary.Add(key))
                {
                    yield return item;
                }
            }
        }
    }
}