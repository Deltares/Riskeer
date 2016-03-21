using System;
using System.ComponentModel;

namespace Ringtoets.Piping.Forms.TypeConverters.PropertyDescriptors
{
    /// <summary>
    /// Decorates a <see cref="PropertyDescriptor"/> to also set the property of the containing
    /// object with the updated component.
    /// </summary>
    public class ContainingPropertyUpdateDescriptorDecorator : PropertyDescriptor
    {
        private readonly object source;
        private readonly PropertyDescriptor sourceContainingProperty;
        private readonly PropertyDescriptor originalPropertyDescriptor;

        /// <summary>
        /// Creates a new <see cref="ContainingPropertyUpdateDescriptorDecorator"/>
        /// </summary>
        /// <param name="propertyDescription">The original property description.</param>
        /// <param name="source">The source object, which contains the property described by <paramref name="propertyDescription"/>.</param>
        /// <param name="containingProperty">The property which contains the </param>
        public ContainingPropertyUpdateDescriptorDecorator(PropertyDescriptor propertyDescription, object source, PropertyDescriptor containingProperty) : base(propertyDescription)
        {
            this.source = source;
            sourceContainingProperty = containingProperty;
            originalPropertyDescriptor = propertyDescription;
        }

        public override void SetValue(object component, object value)
        {
            originalPropertyDescriptor.SetValue(component, value);
            if (source != null && sourceContainingProperty != null)
            {
                sourceContainingProperty.SetValue(source, component);
            }
        }

        #region Members delegated to wrapped descriptor

        public override bool CanResetValue(object component)
        {
            return originalPropertyDescriptor.CanResetValue(component);
        }

        public override object GetValue(object component)
        {
            return originalPropertyDescriptor.GetValue(component);
        }

        public override void ResetValue(object component)
        {
            originalPropertyDescriptor.ResetValue(component);
        }

        public override bool ShouldSerializeValue(object component)
        {
            return originalPropertyDescriptor.ShouldSerializeValue(component);
        }

        public override Type ComponentType
        {
            get
            {
                return originalPropertyDescriptor.ComponentType;
            }
        }

        public override bool IsReadOnly
        {
            get
            {
                return originalPropertyDescriptor.IsReadOnly;
            }
        }

        public override Type PropertyType
        {
            get
            {
                return originalPropertyDescriptor.PropertyType;
            }
        }

        public override string Description
        {
            get
            {
                return originalPropertyDescriptor.Description;
            }
        }

        public override string DisplayName
        {
            get
            {
                return originalPropertyDescriptor.DisplayName;
            }
        }
        
        public override AttributeCollection Attributes
        {
            get
            {
                return originalPropertyDescriptor.Attributes;
            }
        }

        public override string Category
        {
            get
            {
                return originalPropertyDescriptor.Category;
            }
        }

        public override TypeConverter Converter
        {
            get
            {
                return originalPropertyDescriptor.Converter;
            }
        }

        public override bool DesignTimeOnly
        {
            get
            {
                return originalPropertyDescriptor.DesignTimeOnly;
            }
        }

        public override bool IsBrowsable
        {
            get
            {
                return originalPropertyDescriptor.IsBrowsable;
            }
        }

        public override bool IsLocalizable
        {
            get
            {
                return originalPropertyDescriptor.IsLocalizable;
            }
        }

        public override string Name
        {
            get
            {
                return originalPropertyDescriptor.Name;
            }
        }

        public override bool SupportsChangeEvents
        {
            get
            {
                return originalPropertyDescriptor.SupportsChangeEvents;
            }
        }

        #endregion
    }
}