namespace Cake.Incubator.Test
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Helper methods for settings file
    /// </summary>
    public static class DotNetCoreXUnitSettingsExtensions
    {
        /// <summary>
        /// Adds a trait to the settings, to include in test execution.
        /// </summary>
        /// <param name="settings">The settings.</param>
        /// <param name="name">The trait name.</param>
        /// <param name="values">The trait values.</param>
        /// <returns>The same <see cref="DotNetCoreXUnitSettings"/> instance so that multiple calls can be chained.</returns>
        public static DotNetCoreXUnitSettings IncludeTrait(this DotNetCoreXUnitSettings settings, string name, params string[] values)
        {
            settings.ThrowIfNull(nameof(settings));
            name.ThrowIfNull(nameof(name));
            values.ThrowIfNull(nameof(values));

            if (values.Any(v => v == null))
            {
                throw new ArgumentException("values may not contain a null value.", nameof(values));
            }

            if (!settings.TraitsToInclude.ContainsKey(name))
            {
                settings.TraitsToInclude.Add(name, new List<string>());
            }

            foreach (var value in values.Where(v => v != null))
            {
                settings.TraitsToInclude[name].Add(value);
            }

            return settings;
        }

        /// <summary>
        /// Adds a trait to the settings, to exclude in test execution.
        /// </summary>
        /// <param name="settings">The settings.</param>
        /// <param name="name">The trait name.</param>
        /// <param name="values">The trait values.</param>
        /// <returns>The same <see cref="DotNetCoreXUnitSettings"/> instance so that multiple calls can be chained.</returns>
        public static DotNetCoreXUnitSettings ExcludeTrait(this DotNetCoreXUnitSettings settings, string name, params string[] values)
        {
            settings.ThrowIfNull(nameof(settings));
            name.ThrowIfNull(nameof(name));
            values.ThrowIfNull(nameof(values));

            if (values.Any(v => v == null))
            {
                throw new ArgumentException("values may not contain a null value.", nameof(values));
            }

            if (!settings.TraitsToExclude.ContainsKey(name))
            {
                settings.TraitsToExclude.Add(name, new List<string>());
            }

            foreach (var value in values.Where(v => v != null))
            {
                settings.TraitsToExclude[name].Add(value);
            }

            return settings;
        }

        /// <summary>
        /// Adds a namespace to the settings, to include in test execution.  Namespace should be fully qualified; i.e., MyNameSpace.MySubNamespace
        /// </summary>
        /// <param name="settings">The settings.</param>
        /// <param name="namespaceToInclude">The namespace to include.</param>
        /// <returns>The same <see cref="DotNetCoreXUnitSettings"/> instance so that multiple calls can be chained.</returns>
        public static DotNetCoreXUnitSettings IncludeNamespace(this DotNetCoreXUnitSettings settings, string namespaceToInclude)
        {
            settings.ThrowIfNull(nameof(settings));
            if (string.IsNullOrEmpty(namespaceToInclude))
            {
                throw new ArgumentException("Value cannot be null or empty.", nameof(namespaceToInclude));
            }

            settings.NamespacesToRun.Add(namespaceToInclude);

            return settings;
        }

        /// <summary>
        /// Adds a class name to the settings, to include in test execution. Class name should be fully qualified; i.e., MyNameSpace.MyClassName
        /// </summary>
        /// <param name="settings">The settings.</param>
        /// <param name="classNameToInclude">The class name to include.</param>
        /// <returns>The same <see cref="DotNetCoreXUnitSettings"/> instance so that multiple calls can be chained.</returns>
        public static DotNetCoreXUnitSettings IncludeClass(this DotNetCoreXUnitSettings settings, string classNameToInclude)
        {
            settings.ThrowIfNull(nameof(settings));
            if (string.IsNullOrEmpty(classNameToInclude))
            {
                throw new ArgumentException("Value cannot be null or empty.", nameof(classNameToInclude));
            }

            settings.ClassesToRun.Add(classNameToInclude);

            return settings;
        }

        /// <summary>
        /// Adds a method name to the settings, to include in test execution. Method name should be fully qualified; i.e., MyNameSpace.MyClassName.MyMethod
        /// </summary>
        /// <param name="settings">The settings.</param>
        /// <param name="methodNameToInclude">The method name to include.</param>
        /// <returns>The same <see cref="DotNetCoreXUnitSettings"/> instance so that multiple calls can be chained.</returns>
        public static DotNetCoreXUnitSettings IncludeMethod(this DotNetCoreXUnitSettings settings, string methodNameToInclude)
        {
            settings.ThrowIfNull(nameof(settings));
            if (string.IsNullOrEmpty(methodNameToInclude))
            {
                throw new ArgumentException("Value cannot be null or empty.", nameof(methodNameToInclude));
            }

            settings.MethodsToRun.Add(methodNameToInclude);

            return settings;
        }
    }
}