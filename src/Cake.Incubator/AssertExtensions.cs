// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/. 

namespace Cake.Incubator
{
    using System;

    public static class AssertExtensions
    {
        public static T ThrowIfNull<T>(this T obj, string varName)
        {
            if (obj == null)
                throw new ArgumentNullException(varName ?? "object");

            return obj;
        }

        public static T ThrowIfNull<T>(this T obj, string varName, string message)
        {
            if (obj == null)
                throw new ArgumentNullException(varName ?? "object", message);

            return obj;
        }

        public static string ThrowIfNullOrEmpty(this string strValue, string varName)
        {
            if (string.IsNullOrEmpty(strValue))
                throw new ArgumentNullException(varName ?? "string");

            return strValue;
        }
    }
}