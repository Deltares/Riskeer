using System;
using System.ComponentModel;
using System.Globalization;
using Core.Common.Gui.Properties;

namespace Core.Common.Gui
{
    /// <summary>
    /// <see cref="ArrayConverter"/> with modified conversion to string and shows as array
    /// starting with index 1 instead of 0.
    /// </summary>
    public class ExpandableArrayConverter : ArrayConverter
    {
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            var array = value as Array;
            if (destinationType == typeof(string) && array != null)
            {
                return string.Format(Resources.ExpandableArrayConverter_ConvertTo_Count_0_, array.GetLength(0));
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
                for (int index = 0; index < length; ++index)
                {
                    properties[index] = new ArrayPropertyDescriptor(type, elementType, index);
                }
            }
            return new PropertyDescriptorCollection(properties);
        }

        #region Nested Type: ArrayPropertyDescriptor

        /// <summary>
        /// Array element property descriptor used by <see cref="ExpandableArrayConverter"/>.
        /// Properties are named based on their index + 1.
        /// </summary>
        private class ArrayPropertyDescriptor : SimplePropertyDescriptor
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

            public override object GetValue(object instance)
            {
                var array = (Array)instance;
                return array.GetValue(index);
            }

            public override void SetValue(object instance, object value)
            {
                var array = (Array)instance;
                array.SetValue(value, index);
                // This class is based on the System.ComponentModel.ArrayConverter.ArrayPropertyDescriptor,
                // and there the SetValue also called OnValueChanged. Copying that behavior here as well.
                OnValueChanged(array, EventArgs.Empty);
            }
        }

        #endregion
    }
}