// Copyright (C) Stichting Deltares 2018. All rights reserved.
//
// This file is part of Ringtoets.
//
// Ringtoets is free software: you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with this program. If not, see <http://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of
// Stichting Deltares and remain full property of Stichting Deltares at all times.
// All rights reserved.

using System;
using System.ComponentModel;
using System.Globalization;
using Core.Common.Gui.Properties;

namespace Core.Common.Gui.Converters
{
    /// <summary>
    /// Defines a converter for arrays with elements. Properties generated for the array elements uses one
    /// of the element's properties as a name and one of the element's properties as a value.
    /// </summary>
    public class KeyValueExpandableArrayConverter : ArrayConverter
    {
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            var array = value as Array;
            if (destinationType == typeof(string) && array != null)
            {
                return string.Format(CultureInfo.CurrentCulture,
                                     Resources.ExpandableArrayConverter_ConvertTo_Count_0_,
                                     array.GetLength(0));
            }

            return base.ConvertTo(context, culture, value, destinationType);
        }

        public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes)
        {
            var keyValueAttribute = context?.PropertyDescriptor?.Attributes?[typeof(KeyValueElementAttribute)] as KeyValueElementAttribute;

            if (keyValueAttribute == null)
            {
                throw new ArgumentException($"The {typeof(KeyValueExpandableArrayConverter).Name} can only be used on properties that have the " +
                                            $"{typeof(KeyValueElementAttribute).Name} defined.");
            }

            PropertyDescriptor[] properties = null;
            var array = value as Array;
            if (array != null)
            {
                Type type = array.GetType();
                Type elementType = type.GetElementType();

                int length = array.GetLength(0);
                properties = new PropertyDescriptor[length];

                for (var index = 0; index < length; ++index)
                {
                    object source = array.GetValue(index);
                    properties[index] = new ArrayPropertyDescriptor(elementType, keyValueAttribute.GetName(source), keyValueAttribute.GetValue(source));
                }
            }

            return new PropertyDescriptorCollection(properties);
        }

        /// <summary>
        /// Array element property descriptor used by <see cref="ExpandableArrayConverter"/>.
        /// Properties are named based on the first item in the provided tuple and the value is
        /// based on the second item.
        /// </summary>
        private class ArrayPropertyDescriptor : SimplePropertyDescriptor
        {
            private readonly object value;

            /// <summary>
            /// Creates a new instance of <see cref="ArrayPropertyDescriptor"/>.
            /// </summary>
            /// <param name="elementType">The type of elements of the array.</param>
            /// <param name="name">The name of the property.</param>
            /// <param name="value">The value of the property.</param>
            public ArrayPropertyDescriptor(Type elementType, string name, object value)
                : base(elementType, name, value.GetType())
            {
                this.value = value;
            }

            public override bool IsReadOnly
            {
                get
                {
                    return true;
                }
            }

            public override object GetValue(object component)
            {
                return value;
            }

            public override void SetValue(object component, object valueToSet)
            {
                throw new NotSupportedException();
            }
        }
    }
}