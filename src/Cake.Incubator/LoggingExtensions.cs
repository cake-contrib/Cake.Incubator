﻿// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/. 
namespace Cake.Incubator.LoggingExtensions
{
    using System;
    using System.Collections;
    using System.ComponentModel;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using Cake.Core.Annotations;

    /// <summary>
    /// Several extension methods when using Logging.
    /// </summary>
    [CakeNamespaceImport("Cake.Incubator.LoggingExtensions")]
    public static class LoggingExtensions
    {
        /// <summary>
        /// Get a basic string representation of specified object.
        /// </summary>
        /// <typeparam name="T">Type of object</typeparam>
        /// <param name="obj">Object to generate string representation of</param>
        /// <returns>String representation of object in format in format `Prop: PropValue\r\nArrayProp: ArrayVal1, ArrayVal2`</returns>
        /// <example>
        /// Generates a string representation of objects public properties and values.
        /// <code>
        /// var person = new Person { Name = "Bob", Age = 24, Food = new[] { "Lasagne", "Pizza"} };
        /// var data = person.Dump();
        ///
        /// // output:
        /// "Name: Bob\r\nAge: 24\r\nFood: Lasagne, Pizza";
        /// </code>
        /// 
        /// Useful in for logging objects, e.g.
        /// <code>
        /// var gitVersionResults = GitVersion(new GitVersionSettings());
        /// Information("GitResults -&gt; {0}", gitVersionResults.Dump());
        ///
        /// // output:
        /// GitResults -&gt; Major: 0
        /// Minor: 1
        /// Patch: 0
        /// PreReleaseTag: dev-19.1
        /// PreReleaseTagWithDash: -dev-19.1
        /// PreReleaseLabel: dev-19
        /// PreReleaseNumber: 1
        /// BuildMetaData: 26
        /// ..snip..
        /// </code>
        /// </example>
        public static string Dump<T>(this T obj)
        {
            if (obj == null) return null;
            var dump = new StringBuilder();
            
            if (obj is IEnumerable objEnumerable)
            {
                var first = true;
                foreach (var o in objEnumerable)
                {
                    if (first)
                    {
                        dump.AppendLine("{");
                        first = false;
                    }
                    else
                    {
                        dump.AppendLine(",");
                        dump.AppendLine("{");
                    }

                    dump.Append(ObjectToString(o));
                    dump.Append("}");
                }
            }
            else dump.Append(ObjectToString(obj));

            return dump.ToString();
        }

        private static string ObjectToString<T>(T obj) 
        {
            var dump = new StringBuilder();

            foreach (PropertyDescriptor descriptor in TypeDescriptor.GetProperties(obj))
            {
                var propertyType = descriptor.PropertyType;
                var value = descriptor.GetValue(obj);

                var enumerableType = propertyType.GetInterface("IEnumerable");

                if (enumerableType != null && propertyType != typeof(string))
                {
                    ProcessEnumerable(value, dump, descriptor);
                    continue;
                }

                dump.AppendLine($"\t{descriptor.Name}:\t{value}");
            }

            return dump.ToString();
        }

        private static void ProcessEnumerable(object value, StringBuilder sb, MemberDescriptor descriptor)
        {
            // Is a collection, iterate and spit out value for each
            if (!(value is IEnumerable enumerable)) return;
            
            var first = true;
            sb.Append($"\t{descriptor.Name}:\t[ ");
            foreach (var val in enumerable)
            {
                if (val == null) continue;
                var printVal = IsSimpleType(val.GetType()) ? $"\"{val}\"" : val.Dump().Replace(Environment.NewLine, Environment.NewLine+"\t");
                if (first)
                {
                    sb.Append(printVal);
                    first = false;
                }
                else
                    sb.Append($", {printVal}");
            }

            sb.AppendLine(" ]");
       }
        
        private static bool IsSimpleType(Type type)
        {
            return
                type.IsPrimitive ||
                new Type[]
                {
                    typeof(Enum),
                    typeof(String),
                    typeof(Decimal),
                    typeof(DateTime),
                    typeof(DateTimeOffset),
                    typeof(TimeSpan),
                    typeof(Guid)
                }.Contains(type) ||
                Convert.GetTypeCode(type) != TypeCode.Object ||
                (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>) &&
                 IsSimpleType(type.GetGenericArguments()[0]));
        }
    }
}
