using System;
using System.ComponentModel;

namespace Ringtoets.Piping.Forms.TypeConverters.PropertyDescriptors
{
    public class RoutedPropertyDescriptor : PropertyDescriptor
    {
        private readonly Func<object, object> rerouteToActualPropertyOwner;
        private readonly PropertyDescriptor originalPropertyDescriptor;

        public RoutedPropertyDescriptor(PropertyDescriptor descr, Func<object, object> routerFunction)
            : base(descr)
        {
            originalPropertyDescriptor = descr;
            rerouteToActualPropertyOwner = routerFunction;
        }

        public override bool CanResetValue(object component)
        {
            return originalPropertyDescriptor.CanResetValue(rerouteToActualPropertyOwner(component));
        }

        public override object GetValue(object component)
        {
            return originalPropertyDescriptor.GetValue(rerouteToActualPropertyOwner(component));
        }

        public override void ResetValue(object component)
        {
            originalPropertyDescriptor.ResetValue(rerouteToActualPropertyOwner(component));
        }

        public override void SetValue(object component, object value)
        {
            originalPropertyDescriptor.SetValue(rerouteToActualPropertyOwner(component), value);
        }

        public override bool ShouldSerializeValue(object component)
        {
            return originalPropertyDescriptor.ShouldSerializeValue(rerouteToActualPropertyOwner(component));
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
    }
}