// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/. 
namespace Cake.Incubator
{
    using System;

    public static class StringExtensions
    {
        /// <summary>
        /// Case-insensitive String.Equals
        /// </summary>
        /// <param name="source">the source string</param>
        /// <param name="value">the string to compare</param>
        /// <returns>true if strings are the same</returns>
        public static bool EqualsIgnoreCase(this string source, string value)
        {
            return source.Equals(value, StringComparison.InvariantCultureIgnoreCase);
        }
    }
}