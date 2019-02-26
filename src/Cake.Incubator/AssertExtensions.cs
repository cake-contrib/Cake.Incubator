// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/. 

namespace Cake.Incubator.AssertExtensions
{
    using System;
    using System.Collections.Generic;
    using Cake.Core.Annotations;

    /// <summary>
    /// Contains extensions for guard clauses
    /// </summary>
    [CakeAliasCategory("Guard")]
    public static class AssertExtensions
    {
        /// <summary>
        /// Throws a <see cref="ArgumentNullException"/> if the value is null, otherwise returns the value
        /// </summary>
        /// <typeparam name="T">The type to return</typeparam>
        /// <param name="value">The object to check</param>
        /// <param name="varName">The name of the variable</param>
        /// <exception cref="ArgumentNullException">value is null.</exception>
        /// <returns>The non-null value</returns>
        /// <example>
        /// replace the following
        /// <code>
        /// if (myArg == null) 
        /// {
        ///   throw new ArgumentNullException(nameof(myArg));
        /// }
        /// var arg1 = myArg;
        /// </code>
        /// with 
        /// <code>
        /// var arg1 = myArg.ThrowIfNull(nameof(myArg));
        /// </code>
        /// </example>
        [CakeAliasCategory("Guard Clauses")]
        public static T ThrowIfNull<T>(this T value, string varName)
        {
            if (value == null)
                throw new ArgumentNullException(varName ?? "object");

            return value;
        }

        /// <summary>
        /// Throws a <see cref="ArgumentNullException"/> with a specific message if the value is null, otherwise returns the value
        /// </summary>
        /// <typeparam name="T">The type to return</typeparam>
        /// <param name="value">The object to check</param>
        /// <param name="varName">The name of the variable</param>
        /// <param name="message">The exception message</param>
        /// <exception cref="ArgumentNullException">value is null.</exception>
        /// <returns>The non-null value</returns>
        /// <example>
        /// replace the following
        /// <code>
        /// if (myArg == null) 
        /// {
        ///   throw new ArgumentNullException(nameof(myArg), "Oops");
        /// }
        /// var arg1 = myArg;
        /// </code>
        /// with 
        /// <code>
        /// var arg1 = myArg.ThrowIfNull(nameof(myArg), "Oops");
        /// </code>
        /// </example>
        [CakeAliasCategory("Guard Clauses")]
        public static T ThrowIfNull<T>(this T value, string varName, string message)
        {
            if (value == null)
                throw new ArgumentNullException(varName ?? "object", message);

            return value;
        }

        /// <summary>
        /// Throws a <see cref="ArgumentNullException"/> if a string is null or white space, otherwise returns the value
        /// </summary>
        /// <param name="value">The object to check</param>
        /// <param name="varName">The name of the variable</param>
        /// <exception cref="ArgumentNullException">value is null.</exception>
        /// <returns>The non-null value</returns>
        /// <example>
        /// replace the following
        /// <code>
        /// string myArg = "";
        /// if (string.IsNullOrWhiteSpace(myArg)) 
        /// {
        ///   throw new ArgumentNullException(nameof(myArg));
        /// }
        /// var arg1 = myArg;
        /// </code>
        /// with 
        /// <code>
        /// string myArg = "";
        /// var arg1 = myArg.ThrowIfNullOrWhiteSpace(nameof(myArg));
        /// </code>
        /// </example>
        [CakeAliasCategory("Guard Clauses")]
        public static string ThrowIfNullOrWhiteSpace(this string value, string varName)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentNullException(varName ?? "string");

            return value;
        }
        
        /// <summary>
        /// Throws a <see cref="ArgumentNullException"/> if a string is null or empty, otherwise returns the value
        /// </summary>
        /// <param name="value">The object to check</param>
        /// <param name="varName">The name of the variable</param>
        /// <exception cref="ArgumentNullException">value is null.</exception>
        /// <returns>The non-null value</returns>
        /// <example>
        /// replace the following
        /// <code>
        /// string myArg = "";
        /// if (string.IsNullOrEmpty(myArg)) 
        /// {
        ///   throw new ArgumentNullException(nameof(myArg));
        /// }
        /// var arg1 = myArg;
        /// </code>
        /// with 
        /// <code>
        /// string myArg = "";
        /// var arg1 = myArg.ThrowIfNullOrEmpty(nameof(myArg));
        /// </code>
        /// </example>
        [CakeAliasCategory("Guard Clauses")]
        public static string ThrowIfNullOrEmpty(this string value, string varName)
        {
            if (string.IsNullOrEmpty(value))
                throw new ArgumentNullException(varName ?? "string");

            return value;
        }
        
        [CakeAliasCategory("Guard Clauses")]
        public static bool IsNullOrEmpty(this string value)
        {
            return string.IsNullOrEmpty(value);
        }
        
        [CakeAliasCategory("Guard Clauses")]
        public static bool IsNullOrEmpty<T>(this ICollection<T> value)
        {
            return (value is null || value.Count == 0);
        }
    }
}