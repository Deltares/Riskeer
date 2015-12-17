using System;
using System.ComponentModel;

namespace Core.Common.Utils.PropertyBag
{
    public class PropertySpecDescriptor : PropertyDescriptor
    {
        private readonly DynamicPropertyBag bag;
        private readonly PropertySpec item;

        public PropertySpecDescriptor(PropertySpec item, DynamicPropertyBag bag, string name, Attribute[] attrs)
            :
                base(name, attrs)
        {
            this.bag = bag;
            this.item = item;
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
                return (Attributes.Matches(ReadOnlyAttribute.Yes));
            }
        }

        public override bool IsBrowsable
        {
            get
            {
                ReEvaluateAttributes();
                return base.IsBrowsable;
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

        public override object GetValue(object component)
        {
            return ReEvaluateAttributes().Value;
        }

        public override void ResetValue(object component)
        {

        }

        public override void SetValue(object component, object value)
        {
            // Have the property bag raise an event to set the current value
            // of the property.

            PropertySpecEventArgs e = new PropertySpecEventArgs(item, value);
            bag.OnSetValue(e.Property.Name, e.Value);
        }

        public override bool ShouldSerializeValue(object component)
        {
            return false;
        }

        private PropertySpecEventArgs ReEvaluateAttributes()
        {
            // Have the property bag raise an event to get the current value
            // of the property and evaluate the dynamic attributes
            var e = new PropertySpecEventArgs(item, null);
            bag.OnGetValue(e, e.Property);
            AttributeArray = e.Property.Attributes;
            return e;
        }
    }
}