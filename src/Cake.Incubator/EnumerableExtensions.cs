// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/. 

namespace Cake.Incubator
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Cake.Core.Annotations;

    /// <summary>
    /// Several extension methods when using IEnumerable.
    /// </summary>
    [CakeAliasCategory("Collection Helpers")]
    public static class EnumerableExtensions
    {
        /// <summary>
        /// Performs an action on a collection of items
        /// </summary>
        /// <typeparam name="T">The item type</typeparam>
        /// <param name="source">the collection</param>
        /// <param name="action">the action to perform</param>
        /// <example>
        /// Replace the following
        /// <code>
        /// foreach(var item in items) 
        /// {
        ///   Debug.WriteLine(item);
        /// }
        /// </code>
        /// with
        /// <code>
        /// items.Each(item =&gt; Debug,WriteLine(item));
        /// </code>
        /// </example>
        public static void Each<T>(this IEnumerable<T> source, Action<T> action)
        {
            source.ThrowIfNull(nameof(source));
            action.ThrowIfNull(nameof(action));
            foreach (var item in source)
            {
                action(item);
            }
        }

        /// <summary>
        /// Checks whether specified IEnumerable is null or contains no elements
        /// </summary>
        /// <typeparam name="T">the item type</typeparam>
        /// <param name="source">the collection</param>
        /// <returns>true if element null or empty, else false</returns>
        /// <example>
        /// Replace
        /// <code>collection == null || !collection.Any()</code>
        /// with
        /// <code>collection.IsNullOrEmpty()</code>
        /// </example>
        public static bool IsNullOrEmpty<T>(this IEnumerable<T> source) => source == null || !source.Any();

        /// <summary>
        /// Checks whether specified IList is null or contains no elements
        /// </summary>
        /// <typeparam name="T">the item type</typeparam>
        /// <param name="source">the collection</param>
        /// <returns>true if element null or empty, else false</returns>
        /// <example>
        /// Replace
        /// <code>collection == null || collection.Count == 0</code>
        /// with
        /// <code>collection.IsNullOrEmpty()</code>
        /// </example>
        public static bool IsNullOrEmpty<T>(this IList<T> source) => source == null || source.Count == 0;

        /// <summary>
        /// Checks whether specified array is null or contains no elements
        /// </summary>
        /// <typeparam name="T">the item type</typeparam>
        /// <param name="source">the array</param>
        /// <returns>true if element null or empty, else false</returns>
        /// <example>
        /// Replace
        /// <code>collection == null || collection.Length == 0</code>
        /// with
        /// <code>collection.IsNullOrEmpty()</code>
        /// </example>
        public static bool IsNullOrEmpty<T>(this T[] source) => source == null || source.Length == 0;

        /// <summary>
        /// Select a distinct instance of an object from a collection of objects.
        /// </summary>
        /// <typeparam name="TSource">The type of the object to select.</typeparam>
        /// <typeparam name="TKey">The type of the key to search for.</typeparam>
        /// <param name="source">The collection of objects to select from.</param>
        /// <param name="getKey">The key that is being searched for.</param>
        /// <returns></returns>
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
