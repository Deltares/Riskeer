using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

using Core.Common.Gui.Attributes;

namespace Core.Common.Gui.PropertyBag
{
    public class PropertySpecDescriptor : PropertyDescriptor
    {
        private readonly PropertySpec item;
        private readonly object instance;

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
            item.SetValue(component, value);
        }

        public override bool ShouldSerializeValue(object component)
        {
            return false;
        }

        private PropertySpecEventArgs ReEvaluateAttributes()
        {
            UpdateDynamicAttributes();

            // Have the property bag raise an event to get the current value of the property:
            var e = new PropertySpecEventArgs(item, null);
            OnGetValue(e, e.Property);
            AttributeArray = e.Property.Attributes; // TODO: Override AttributeArray to reroute to 'item.Attributes'
            return e;
        }

        private void UpdateDynamicAttributes()
        {
            var attributeList = new List<Attribute>();
            attributeList.AddRange(item.Attributes.ToList());

            //check all of the attributes: if we find a dynamic one, evaluate it and possibly add/overwrite a static attribute
            foreach (Attribute customAttribute in item.Attributes)
            {
                if (customAttribute is DynamicReadOnlyAttribute)
                {
                    attributeList.RemoveAll(x => x is ReadOnlyAttribute);

                    if (DynamicReadOnlyAttribute.IsReadOnly(instance, item.Name))
                    {
                        //condition is true: the dynamic attribute should be applied (as static attribute)
                        attributeList.Add(new ReadOnlyAttribute(true)); //add static read only attribute
                    }
                }

                if (customAttribute is DynamicVisibleAttribute)
                {
                    attributeList.RemoveAll(x => x is BrowsableAttribute);

                    if (!DynamicVisibleAttribute.IsVisible(instance, item.Name))
                    {
                        attributeList.Add(new BrowsableAttribute(false));
                    }
                }
            }

            item.Attributes = attributeList.ToArray();
        }

        /// <summary>
        /// Raises the GetValue event.
        /// </summary>
        /// <param name="e">A PropertySpecEventArgs that contains the event data.</param>
        /// <param name="propertySpec"></param>
        private void OnGetValue(PropertySpecEventArgs e, PropertySpec propertySpec)
        {
            var propertyInfo = instance.GetType().GetProperty(propertySpec.Name);
            var value = propertyInfo.GetValue(instance, null);

            var isNestedPropertiesObject = IsNestedExpandablePropertiesObject(propertyInfo);

            // if nested properties object, wrap in DynamicPropertyBag to provide support for things like DynamicReadOnly
            e.Value = isNestedPropertiesObject ? new DynamicPropertyBag(value) : value;
        }

        private bool IsNestedExpandablePropertiesObject(System.Reflection.PropertyInfo propertyInfo)
        {
            try
            {
                var typeConverterAttributes = propertyInfo.GetCustomAttributes(typeof(TypeConverterAttribute), false);
                foreach (TypeConverterAttribute typeConverterAttribute in typeConverterAttributes)
                {
                    var typeString = typeConverterAttribute.ConverterTypeName;
                    var type = Type.GetType(typeString);
                    if (type != null)
                    {
                        if (typeof(ExpandableObjectConverter) == type)
                        {
                            return true;
                        }
                    }
                }
            }
            catch (Exception)
            {
                //gulp
            }
            return false;
        }
    }
}