using System;
using System.ComponentModel;

namespace DelftTools.Utils
{
    ///<summary>
    /// Type conversion ie string conversion, works with nullable types.
    ///</summary>
    /// <remarks>http://www.dogaoztuzun.com/post/C-Generic-Type-Conversion.aspx</remarks>
    public class TypeConverter
    {
        /// <summary>
        /// Convert (and cast) value to given type using the current type's TypeConverter
        /// </summary>
        /// <typeparam name="T">The target type to convert to</typeparam>
        /// <param name="value">The value object to convert from</param>
        /// <returns>The value object, converted and casted into the target type</returns>
        public static T ConvertValueToTargetType<T>(object value)
        {
            return (T)ConvertValueToTargetType(typeof(T), value);
        }

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




        /// <summary>
        /// Convert the object value using a specified TypeConveter, circumventing the Type's TypeConverter
        /// </summary>
        /// <param name="value">The original object to convert from</param>
        /// <param name="tc">The type converter that will be used</param>
        /// <returns></returns>
        public static object ConvertValueToTargetType(object value, System.ComponentModel.TypeConverter tc)
        {
            //System.ComponentModel.TypeConverter tc = TypeDescriptor.GetConverter(t);
            return tc.ConvertFrom(value);
        }


        /// <summary>
        /// Overrides the original TypeConverter with the specified TypeConverter
        /// <remarks>Usefull for registering TypeConverters to Types that are closed such as library types</remarks>
        /// </summary>
        /// <typeparam name="T">The type to attach the specified converter to</typeparam>
        /// <typeparam name="TC">The converter that will handle all type conversions to and from the type</typeparam>
        public static void RegisterTypeConverter<T, TC>() where TC : System.ComponentModel.TypeConverter
        {
            TypeConverterAttribute tca = new TypeConverterAttribute(typeof (TC));
            if (!TypeDescriptor.GetAttributes(typeof(T)).Contains(tca))
            {
                TypeDescriptor.AddAttributes(typeof(T), tca);
            }
        }

    }
}