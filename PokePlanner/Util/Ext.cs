using System;

namespace PokePlanner.Util
{
    /// <summary>
    /// Support class for extension methods.
    /// </summary>
    public static class Ext
    {
        /// <summary>
        /// Returns the string as an enum value.
        /// </summary>
        public static T ToEnum<T>(this string st)
        {
            if (string.IsNullOrEmpty(st))
            {
                return default(T);
            }

            return (T) Enum.Parse(typeof(T), st, true);
        }
    }
}
