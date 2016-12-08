// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/. 
namespace Cake.Extensions
{
    using System.Collections;
    using System.ComponentModel;
    using System.Text;

    public static class LoggingExtensions
    {
        /// <summary>
        /// Get a basic string representation of specified object.
        /// </summary>
        /// <typeparam name="T">Type of object</typeparam>
        /// <param name="obj">Object to generate string representation of</param>
        /// <returns>String representation of object in format in format "Prop: PropValue\r\nArrayProp: ArrayVal1, ArrayVal2"</returns>
        public static string Dump<T>(this T obj)
        {
            if (obj == null) return null;

            var dump = new StringBuilder();
            foreach (PropertyDescriptor descriptor in TypeDescriptor.GetProperties(obj))
            {
                var propertyType = descriptor.PropertyType;
                var value = descriptor.GetValue(obj);
                if (propertyType.GetInterface("IEnumerable") != null && propertyType != typeof(string))
                {
                    ProcessEnumerable(value, dump, descriptor);
                    continue;
                }

                dump.Append($"\r\n{descriptor.Name}: {value}");
            }
            
            return dump.ToString().Remove(0, 2);
        }

        private static void ProcessEnumerable(object value, StringBuilder sb, MemberDescriptor descriptor)
        {
            // Is a collection, iterate and spit out value for each
            var enumerable = value as IEnumerable;
            if (enumerable == null) return;

            var enumValues = new StringBuilder();
            var first = true;
            foreach (var val in enumerable)
            {
                if (first)
                {
                    enumValues.Append(val);
                    first = false;
                }
                else
                    enumValues.Append($", {val}");
            }

            sb.Append($"\r\n{descriptor.Name}: {enumValues}");
        }
    }
}
