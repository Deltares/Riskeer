using System;
using System.ComponentModel;

namespace Ringtoets.Piping.Forms.TypeConverters.PropertyDescriptors
{
    /// <summary>
    /// A decorator <see cref="PropertyDescriptor"/> that forces <see cref="IsReadOnly"/>
    /// to true regardless of the wrapped <see cref="PropertyDescriptor"/>.
    /// </summary>
    public class ReadOnlyPropertyDescriptorDecorator : PropertyDescriptor
    {
        private readonly PropertyDescriptor wrappedPropertyDescriptor;

        /// <summary>
        /// Initializes a new instance of the <see cref="ReadOnlyPropertyDescriptorDecorator"/> class.
        /// </summary>
        /// <param name="propertyDescriptor">The property descriptor to be wrapped.</param>
        public ReadOnlyPropertyDescriptorDecorator(PropertyDescriptor propertyDescriptor) : base(propertyDescriptor)
        {
            wrappedPropertyDescriptor = propertyDescriptor;
        }

        public override bool IsReadOnly
        {
            get
            {
                return true;
            }
        }

        #region Methods and Properties delegates to wrapped property descriptor

        public override bool CanResetValue(object component)
        {
            return wrappedPropertyDescriptor.CanResetValue(component);
        }

        public override object GetValue(object component)
        {
            return wrappedPropertyDescriptor.GetValue(component);
        }

        public override void ResetValue(object component)
        {
            wrappedPropertyDescriptor.ResetValue(component);
        }

        public override void SetValue(object component, object value)
        {
            wrappedPropertyDescriptor.SetValue(component, value);
        }

        public override bool ShouldSerializeValue(object component)
        {
            return wrappedPropertyDescriptor.ShouldSerializeValue(component);
        }

        public override AttributeCollection Attributes
        {
            get
            {
                return wrappedPropertyDescriptor.Attributes;
            }
        }

        public override string Category
        {
            get
            {
                return wrappedPropertyDescriptor.Category;
            }
        }

        public override Type ComponentType
        {
            get
            {
                return wrappedPropertyDescriptor.ComponentType;
            }
        }

        public override TypeConverter Converter
        {
            get
            {
                return wrappedPropertyDescriptor.Converter;
            }
        }

        public override string Description
        {
            get
            {
                return wrappedPropertyDescriptor.Description;
            }
        }

        public override bool DesignTimeOnly
        {
            get
            {
                return wrappedPropertyDescriptor.DesignTimeOnly;
            }
        }

        public override string DisplayName
        {
            get
            {
                return wrappedPropertyDescriptor.DisplayName;
            }
        }

        public override bool IsBrowsable
        {
            get
            {
                return wrappedPropertyDescriptor.IsBrowsable;
            }
        }

        public override bool IsLocalizable
        {
            get
            {
                return wrappedPropertyDescriptor.IsLocalizable;
            }
        }

        public override string Name
        {
            get
            {
                return wrappedPropertyDescriptor.Name;
            }
        }

        public override Type PropertyType
        {
            get
            {
                return wrappedPropertyDescriptor.PropertyType;
            }
        }

        public override bool SupportsChangeEvents
        {
            get
            {
                return wrappedPropertyDescriptor.SupportsChangeEvents;
            }
        }

        #endregion
    }
}