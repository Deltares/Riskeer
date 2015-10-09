using System;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Reflection;

namespace DelftTools.Utils
{
    /// <summary>
    /// EnumConverter supporting System.ComponentModel.DescriptionAttribute
    /// </summary>
    public class EnumDescriptionAttributeTypeConverter : EnumConverter
    {
        protected Type type;

        public EnumDescriptionAttributeTypeConverter(Type type) : base(type.GetType())
        {
            this.type = type;
        }

        /// <summary>
        /// Gets Enum Value's Description Attribute
        /// </summary>
        /// <param name="value">The value you want the description attribute for</param>
        /// <returns>The description, if any, else it's .ToString()</returns>
        public static string GetEnumDescription(Enum value)
        {
            FieldInfo fi = value.GetType().GetField(value.ToString());
            var attributes =
                (DescriptionAttribute[]) fi.GetCustomAttributes(
                    typeof(DescriptionAttribute), false);
            return (attributes.Length > 0) ? attributes[0].Description : value.ToString();
        }

        /// <summary>
        /// Gets the description for certaing named value in an Enumeration
        /// </summary>
        /// <param name="value">The type of the Enumeration</param>
        /// <param name="name">The name of the Enumeration value</param>
        /// <returns>The description, if any, else the passed name</returns>
        public static string GetEnumDescription(Type value, string name)
        {
            FieldInfo fi = value.GetField(name);
            var attributes =
                (DescriptionAttribute[]) fi.GetCustomAttributes(
                    typeof(DescriptionAttribute), false);
            return (attributes.Length > 0) ? attributes[0].Description : name;
        }

        /// <summary>
        /// Gets the display name of the enum value
        /// </summary>
        /// <param name="value">The value you want the display name for</param>
        /// <returns>The display name, if any, else it's .ToString()</returns>
        public static string GetEnumDisplayName(Enum value)
        {
            FieldInfo fi = value.GetType().GetField(value.ToString());
            var attributes =
                (DisplayNameAttribute[]) fi.GetCustomAttributes(
                    typeof(DisplayNameAttribute), false);
            return (attributes.Length > 0) ? attributes[0].DisplayName : value.ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="enumType"></param>
        /// <param name="displayNameString"></param>
        /// <returns>Null if no match was found in <paramref name="enumType"/> with <paramref name="displayNameString"/>; Value otherwise.</returns>
        public static object GetEnumValueFromDisplayName(Type enumType, string displayNameString)
        {
            return Enum.GetValues(enumType)
                       .Cast<Enum>()
                       .FirstOrDefault(value => GetEnumDisplayName(value) == displayNameString);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value,
                                         Type destinationType)
        {
            if (value is Enum && destinationType == typeof(string))
            {
                return GetEnumDescription((Enum) value);
            }
            if (value is string && destinationType == typeof(string))
            {
                return GetEnumDescription(type, (string) value);
            }
            return base.ConvertTo(context, culture, value, destinationType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value is string)
            {
                return GetEnumValue(type, (string) value);
            }
            if (value is Enum)
            {
                return GetEnumDescription((Enum) value);
            }
            return base.ConvertFrom(context, culture, value);
        }

        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            return new StandardValuesCollection(Enum.GetValues(type));
        }

        /// <summary>
        /// Gets the value of an Enum, based on it's Description Attribute or named value
        /// </summary>
        /// <param name="value">The Enum type</param>
        /// <param name="description">The description or name of the element</param>
        /// <returns>The value, or the passed in description, if it was not found</returns>
        private static object GetEnumValue(Type value, string description)
        {
            FieldInfo[] fis = value.GetFields();
            foreach (FieldInfo fi in fis)
            {
                var attributes =
                    (DescriptionAttribute[]) fi.GetCustomAttributes(
                        typeof(DescriptionAttribute), false);
                if (attributes.Length > 0)
                {
                    if (attributes[0].Description == description)
                    {
                        return fi.GetValue(fi.Name);
                    }
                }
                if (fi.Name == description)
                {
                    return fi.GetValue(fi.Name);
                }
            }
            return description;
        }
    }
}