// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/. 
namespace Cake.Incubator
{
    using System.Linq;

    public static class FunctionalExtensions
    {
        /// <summary>
        /// Checks if the source is contained in a list
        /// </summary>
        /// <typeparam name="T">The source and list type</typeparam>
        /// <param name="source">The source item</param>
        /// <param name="list">The list of items to check</param>
        /// <returns>True if found, false if not</returns>
        public static bool IsIn<T>(this T source, params T[] list)
        {
            source.ThrowIfNull(nameof(source));
            return list.Contains(source);
        }
    }
}