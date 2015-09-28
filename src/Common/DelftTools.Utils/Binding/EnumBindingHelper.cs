using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using DelftTools.Utils.Reflection;

namespace DelftTools.Utils.Binding
{
    
    /// <summary>
    /// Class to get a list Key,Value of an enum where the keys is the enum value and the value the description
    /// Copied from:
    /// http://geekswithblogs.net/sdorman/archive/2007/08/02/Data-Binding-an-Enum-with-Descriptions.aspx
    /// </summary>
    /// <example>
    /// ComboBox combo = new ComboBox();
    /// combo.DataSource = EnumBindingHelper.ToList<SimpleEnum>();
    /// combo.DisplayMember = "Value";
    /// combo.ValueMember = "Key";
    /// </example>
    public class EnumBindingHelper
    {
        /// <summary>
        /// Gets the <see cref="DescriptionAttribute"/> of an <see cref="Enum"/> type value.
        /// </summary>
        /// <param name="value">The <see cref="Enum"/> type value.</param>
        /// <returns>A string containing the text of the <see cref="DescriptionAttribute"/>.</returns>
        private static string GetDescription(Enum value)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }

            var descriptionAttribute = TypeUtils.GetFieldAttribute<DescriptionAttribute>(value.GetType(), value.ToString());
            return descriptionAttribute != null ? descriptionAttribute.Description : value.ToString();
        }

        /// <summary>
        /// Converts the <see cref="Enum"/> type to an <see cref="IList{T}"/> compatible object.
        /// </summary>
        /// <returns>An <see cref="IList{T}"/> containing the enumerated type value and description.</returns>
        public static IList<KeyValuePair<TEnumType,string>> ToList<TEnumType>() where TEnumType:struct
        {
            Type type = typeof(TEnumType);
            if (!type.IsEnum)
            {
                throw new InvalidOperationException("Type is not an enum type.");
            }

            return Enum.GetValues(type).Cast<TEnumType>()
                .ToDictionary(e => e, e => GetDescription(e as Enum))
                .ToList();
        }
    }
}