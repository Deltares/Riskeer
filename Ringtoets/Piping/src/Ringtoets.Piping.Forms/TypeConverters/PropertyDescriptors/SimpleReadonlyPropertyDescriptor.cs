using System;
using System.ComponentModel;

namespace Ringtoets.Piping.Forms.TypeConverters.PropertyDescriptors
{
    /// <summary>
    /// This class defines a simple readonly property item that isn't an actual property 
    /// of an object, but a standalone piece of data. Because the piece of data does not 
    /// belong to some component, the data is readonly and cannot be set or changed.
    /// </summary>
    public class SimpleReadonlyPropertyDescriptorItem : PropertyDescriptor
    {
        private readonly object valueObject;

        /// <summary>
        /// Initializes a new instance of the <see cref="SimpleReadonlyPropertyDescriptorItem"/> class.
        /// </summary>
        /// <param name="displayName">The display name of the data.</param>
        /// <param name="description">The descriptive text associated with the data.</param>
        /// <param name="id">The name of the 'property', allowing it searched for in collections.</param>
        /// <param name="value">The value that should be shown in the value field.</param>
        public SimpleReadonlyPropertyDescriptorItem(string displayName, string description, string id, object value) :
            base(id, new Attribute[]
            {
                new DisplayNameAttribute(displayName),
                new DescriptionAttribute(description),
                new ReadOnlyAttribute(true)
            })
        {
            valueObject = value;
        }

        public override Type ComponentType
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public override bool IsReadOnly
        {
            get
            {
                return true;
            }
        }

        public override Type PropertyType
        {
            get
            {
                return valueObject.GetType();
            }
        }

        public override bool CanResetValue(object component)
        {
            return false;
        }

        public override object GetValue(object component)
        {
            return valueObject;
        }

        public override void ResetValue(object component)
        {
            throw new NotImplementedException();
        }

        public override void SetValue(object component, object value)
        {
            throw new NotImplementedException();
        }

        public override bool ShouldSerializeValue(object component)
        {
            return false;
        }
    }
}