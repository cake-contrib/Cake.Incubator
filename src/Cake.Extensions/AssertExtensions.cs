namespace Cake.Extensions
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

        public static string ThrowIfNullOrEmpty(this string strValue, string varName)
        {
            if (string.IsNullOrEmpty(strValue))
                throw new ArgumentNullException(varName ?? "string");

            return strValue;
        }
    }
}