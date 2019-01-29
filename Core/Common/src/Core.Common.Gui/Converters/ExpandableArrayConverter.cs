// Copyright (C) Stichting Deltares 2019. All rights reserved.
//
// This file is part of Riskeer.
//
// Riskeer is free software: you can redistribute it and/or modify
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
using Core.Common.Gui.PropertyBag;

namespace Core.Common.Gui.Converters
{
    /// <summary>
    /// <see cref="ArrayConverter"/> with modified conversion to string and shows an array
    /// starting with index 1 instead of 0.
    /// </summary>
    public class ExpandableArrayConverter : ArrayConverter
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
                int length = array.GetLength(0);
                properties = new PropertyDescriptor[length];

                Type type = array.GetType();
                Type elementType = type.GetElementType();
                for (var index = 0; index < length; ++index)
                {
                    properties[index] = CreateElementPropertyDescriptor(type, elementType, index);
                }
            }

            return new PropertyDescriptorCollection(properties);
        }

        /// <summary>
        /// Creates a new instance of type <see cref="PropertyDescriptor"/>.
        /// </summary>
        /// <param name="type">Type of the array.</param>
        /// <param name="elementType">Type of the elements in <paramref name="type"/>.</param>
        /// <param name="index">Index of the element corresponding with this property descriptor.</param>
        /// <returns>New instance of <see cref="PropertyDescriptor"/>.</returns>
        /// <seealso cref="ArrayPropertyDescriptor"/>
        protected virtual PropertyDescriptor CreateElementPropertyDescriptor(Type type, Type elementType, int index)
        {
            return new ArrayPropertyDescriptor(type, elementType, index);
        }

        #region Nested Type: ArrayPropertyDescriptor

        /// <summary>
        /// Array element property descriptor used by <see cref="ExpandableArrayConverter"/>.
        /// Properties are named based on their index + 1.
        /// </summary>
        protected class ArrayPropertyDescriptor : SimplePropertyDescriptor
        {
            private readonly int index;

            /// <summary>
            /// Initializes a new instance of the <see cref="ArrayPropertyDescriptor"/> class.
            /// </summary>
            /// <param name="arrayType">Type of the array.</param>
            /// <param name="elementType">Type of the elements in <paramref name="arrayType"/>.</param>
            /// <param name="elementIndex">Index of the element corresponding with this property descriptor.</param>
            public ArrayPropertyDescriptor(Type arrayType, Type elementType, int elementIndex)
                : base(arrayType, "[" + (elementIndex + 1) + "]", elementType, null)
            {
                index = elementIndex;
            }

            public override object GetValue(object component)
            {
                var array = (Array) component;
                return new DynamicPropertyBag(array.GetValue(index));
            }

            public override void SetValue(object component, object value)
            {
                var array = (Array) component;
                array.SetValue(value, index);
                // This class is based on the System.ComponentModel.ArrayConverter.ArrayPropertyDescriptor,
                // and there the SetValue also called OnValueChanged. Copying that behavior here as well.
                OnValueChanged(array, EventArgs.Empty);
            }
        }

        #endregion
    }
}