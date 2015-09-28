using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace DeltaShell.Plugins.CommonTools.Gui.Property
{
    internal class AttributePropertyDescriptor<T> : PropertyDescriptor
    {
        public AttributePropertyDescriptor(string name, Attribute[] attrs) : base(name, attrs)
        {
        }

        protected AttributePropertyDescriptor(MemberDescriptor descr)
            : base(descr)
        {
        }

        protected AttributePropertyDescriptor(MemberDescriptor descr, Attribute[] attrs)
            : base(descr, attrs)
        {
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
            var prop = GetAttributeProperties(component);
            if (prop != null)
            {
                return prop.Value;
            }
            return null;
        }

        private AttributeProperties<T> GetAttributeProperties(object component)
        {
            var props = component as AttributeProperties<T>[];
            return props != null ? props.FirstOrDefault(p => p.Key == Name) : null;
        }

        public override void SetValue(object component, object value)
        {
            var prop = GetAttributeProperties(component);
            if (prop != null)
            {
                prop.Value = value as string;
            }
        }

        public override bool ShouldSerializeValue(object component)
        {
            return false;
        }

        public override Type ComponentType
        {
            get { return typeof (AttributeProperties<T>[]); }
        }

        public override bool IsReadOnly
        {
            get { return false; }
        }

        public override Type PropertyType
        {
            get { return typeof (string); }
        }
    }

    public class AttributeArrayConverter<T> : ArrayConverter
    {
        public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destType)
        {
            if (destType == typeof(string) && value is AttributeProperties<T>[])
            {
                var attributes = (AttributeProperties<T>[]) value;
                return String.Format("({0} attributes)", attributes.Length);
            }

            return base.ConvertTo(context, culture, value, destType);
        }

        public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes)
        {
            var attributeProperties = value as IEnumerable<AttributeProperties<T>>;

            if (attributeProperties == null)
                return PropertyDescriptorCollection.Empty;

            var collection =
                new PropertyDescriptorCollection(
                    attributeProperties.Select(
                        attributeProperty => new AttributePropertyDescriptor<T>(attributeProperty.Key, attributes))
                                       .Cast<PropertyDescriptor>()
                                       .ToArray());
            
            return collection;
        }
    }
}