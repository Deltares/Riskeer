using System;
using System.ComponentModel;

namespace Core.Common.Utils
{
    /// <summary>
    /// Type conversion ie string conversion, works with nullable types.
    /// </summary>
    /// <remarks>http://www.dogaoztuzun.com/post/C-Generic-Type-Conversion.aspx</remarks>
    public static class TypeConverter
    {
        /// <summary>
        /// Converts value to given type using the current type's TypeConverter
        /// </summary>
        /// <param name="t">The target type to convert to</param>
        /// <param name="value">The value object to convert from</param>
        /// <returns>The original value, converted to the specified type</returns>
        public static object ConvertValueToTargetType(Type t, object value)
        {
            System.ComponentModel.TypeConverter tc = TypeDescriptor.GetConverter(t);
            return tc.ConvertFrom(value);
        }
    }
}