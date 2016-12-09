// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/. 

namespace Cake.Incubator
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public static class EnumerableExtensions
    {
        /// <summary>
        /// Performs an action on a collection of items
        /// </summary>
        /// <typeparam name="T">The item type</typeparam>
        /// <param name="source">the collection</param>
        /// <param name="action">the action to perform</param>
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
        public static bool IsNullOrEmpty<T>(this IEnumerable<T> source) => source == null || !source.Any();

        /// <summary>
        /// Checks whether specified IList is null or contains no elements
        /// </summary>
        /// <typeparam name="T">the item type</typeparam>
        /// <param name="source">the collection</param>
        /// <returns>true if element null or empty, else false</returns>
        public static bool IsNullOrEmpty<T>(this IList<T> source) => source == null || source.Count == 0;

        /// <summary>
        /// Checks whether specified array is null or contains no elements
        /// </summary>
        /// <typeparam name="T">the item type</typeparam>
        /// <param name="source">the array</param>
        /// <returns>true if element null or empty, else false</returns>
        public static bool IsNullOrEmpty<T>(this T[] source) => source == null || source.Length == 0;
    }
}
