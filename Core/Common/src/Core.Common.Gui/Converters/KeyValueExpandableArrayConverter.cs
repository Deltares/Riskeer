// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
    public class KeyValueExpandableArrayConverter : ExpandableArrayConverter
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
            PropertyDescriptor[] properties = null;
            var array = value as Array;
            if (array != null)
            {
                Type type = array.GetType();
                Type elementType = type.GetElementType();

                if (!typeof(KeyValueExpandableArrayElement).IsAssignableFrom(elementType))
                {
                    throw new ArgumentException($"Require elements in the array of type {typeof(KeyValueExpandableArrayElement).Name}.");
                }

                int length = array.GetLength(0);
                properties = new PropertyDescriptor[length];

                for (var index = 0; index < length; ++index)
                {
                    var keyValueExpandableArrayElement = array.GetValue(index) as KeyValueExpandableArrayElement;

                    if (keyValueExpandableArrayElement == null)
                    {
                        throw new ArgumentException($"Require elements in the array to be not null.");
                    }
                    properties[index] = new ArrayPropertyDescriptor(keyValueExpandableArrayElement, type, elementType);
                }
            }
            return new PropertyDescriptorCollection(properties);
        }

        /// <summary>
        /// Array element property descriptor used by <see cref="ExpandableArrayConverter"/>.
        /// Properties are named based the first item in the provided tuple and the value is
        /// based on the second item.
        /// </summary>
        protected class ArrayPropertyDescriptor : SimplePropertyDescriptor
        {
            private readonly object value;

            public ArrayPropertyDescriptor(KeyValueExpandableArrayElement element, Type componentType, Type propertyType)
                : base(componentType, Convert.ToString(element.Name), propertyType)
            {
                value = element.Value;
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

            public override void SetValue(object component, object value)
            {
                throw new NotImplementedException();
            }
        }
    }

    public class KeyValueExpandableArrayElement
    {
        public KeyValueExpandableArrayElement(string name, object value)
        {
            Name = name;
            Value = value;
        }

        public string Name { get; }

        public object Value { get;}
    }
}