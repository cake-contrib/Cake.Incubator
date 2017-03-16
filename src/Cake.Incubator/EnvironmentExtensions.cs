// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/. 

namespace Cake.Incubator
{
    using System.ComponentModel;
    using System.Globalization;
    using Common;
    using Core;
    using Core.Annotations;

    /// <summary>
    /// Contains functionality related to the environment.
    /// </summary>
    [CakeAliasCategory("Environment")]
    public static class EnvironmentExtensions
    {
        /// <summary>
        /// Retrieves the value of the environment variable or throws a <see cref="CakeException"/> if the argument is missing
        /// </summary>
        /// <typeparam name="T">The type of the environment variable</typeparam>
        /// <param name="context">The context.</param>
        /// <param name="variable">The environment variable name.</param>
        /// <exception cref="CakeException">Environment variable value is null.</exception>
        /// <example>
        /// Returns the build number integer
        /// <code>
        /// var buildNumber = EnvironmentVariable&lt;int&gt;("BUILD_NUMBER");
        /// </code>
        /// </example>
        [CakeMethodAlias]
        [CakeAliasCategory("Environment Variables")]
        public static T EnvironmentVariable<T>(this ICakeContext context, string variable)
        {
            var value = context.EnvironmentVariable(variable);

            if (string.IsNullOrEmpty(value))
            {
                const string format = "Environment variable '{0}' was not found.";
                var message = string.Format(CultureInfo.InvariantCulture, format, variable);
                throw new CakeException(message);
            }

            return Convert<T>(value);
        }

        /// <summary>
        /// Retrieves the value of the environment variable or returns the default value specified if missing
        /// </summary>
        /// <typeparam name="T">The type of the environment variable</typeparam>
        /// <param name="context">The context.</param>
        /// <param name="variable">The environment variable name.</param>
        /// <param name="defaultValue">The value to return if the environment variable is missing.</param>
        /// <returns>The value of the argument if it exist; otherwise <paramref name="defaultValue"/>.</returns>
        /// <example>
        /// Returns the build number integer, defaulting to 1 if unspecified
        /// <code>
        /// var buildNumber = EnvironmentVariable&lt;int&gt;("BUILD_NUMBER", 1);
        /// </code>
        /// </example>
        [CakeMethodAlias]
        [CakeAliasCategory("Environment Variables")]
        public static T EnvironmentVariable<T>(this ICakeContext context, string variable, T defaultValue)
        {
            var value = context.EnvironmentVariable(variable);

            return string.IsNullOrEmpty(value) ? defaultValue : Convert<T>(value);
        }

        private static T Convert<T>(string value)
        {
            var converter = TypeDescriptor.GetConverter(typeof(T));
            return (T)converter.ConvertFromInvariantString(value);
        }
    }
}
