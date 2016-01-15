using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

using Core.Common.Gui.Attributes;

namespace Core.Common.Gui.PropertyBag
{
    /// <summary>
    /// A <see cref="PropertyDescriptor"/> that works for properties captured in a <see cref="PropertySpec"/>
    /// and handles Dynamic attributes.
    /// </summary>
    /// <remarks>The following dynamic attributes are supported:
    /// <list type="bullet">
    /// <item><see cref="DynamicReadOnlyAttribute"/></item>
    /// <item><see cref="DynamicVisibleAttribute"/></item>
    /// </list></remarks>
    public class PropertySpecDescriptor : PropertyDescriptor
    {
        private readonly PropertySpec item;
        private readonly object instance;

        /// <summary>
        /// Initializes a new instance of the <see cref="PropertySpecDescriptor"/> class
        /// for a given <see cref="PropertySpec"/>.
        /// </summary>
        /// <param name="propertySpec">The property spec.</param>
        /// <param name="instance">The instance which has the property captured in <paramref name="propertySpec"/>.</param>
        public PropertySpecDescriptor(PropertySpec propertySpec, object instance)
            : base(propertySpec.Name, propertySpec.Attributes)
        {
            item = propertySpec;
            this.instance = instance;
        }

        public override Type ComponentType
        {
            get
            {
                return item.GetType();
            }
        }

        public override bool IsReadOnly
        {
            get
            {
                if (Attributes.Matches(new DynamicReadOnlyAttribute()))
                {
                    return DynamicReadOnlyAttribute.IsReadOnly(instance, item.Name);
                }
                return Attributes.Matches(ReadOnlyAttribute.Yes);
            }
        }

        public override bool IsBrowsable
        {
            get
            {
                if (Attributes.Matches(new DynamicVisibleAttribute()))
                {
                    return DynamicVisibleAttribute.IsVisible(instance, item.Name);
                }
                return !Attributes.Matches(BrowsableAttribute.No);
            }
        }

        public override Type PropertyType
        {
            get
            {
                return Type.GetType(item.TypeName);
            }
        }

        public override bool CanResetValue(object component)
        {
            return false;
        }

        public override void ResetValue(object component)
        {

        }

        public override object GetValue(object component)
        {
            var propertyValue = item.GetValue(component);
            if (item.IsNonCustomExpandableObjectProperty())
            {
                return new DynamicPropertyBag(propertyValue);
            }
            return propertyValue;
        }

        public override void SetValue(object component, object value)
        {
            item.SetValue(component, value);
        }

        public override bool ShouldSerializeValue(object component)
        {
            return false;
        }
    }
}