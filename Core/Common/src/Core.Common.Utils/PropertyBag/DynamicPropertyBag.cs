using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

using Core.Common.Utils.Attributes;

namespace Core.Common.Utils.PropertyBag
{
    /// <summary>
    /// Creates a custom type descriptor for an object using reflection as a property bag. Used for Property Grid.
    /// Additionally it scans the object for any dynamic attributes and processes those, eg checks their condition
    /// at runtime and if met, adds them as static attribute.
    /// </summary>
    public class DynamicPropertyBag : ICustomTypeDescriptor
    {
        public DynamicPropertyBag(object propertyObject)
        {
            Properties = new PropertySpecCollection();
            Initialize(propertyObject);
        }

        /// <summary>
        /// Gets the collection of properties contained within this PropertyBag.
        /// </summary>
        public PropertySpecCollection Properties { get; private set; }

        public object WrappedObject { get; private set; }

        public Type GetContentType()
        {
            return WrappedObject.GetType();
        }

        public override string ToString()
        {
            return WrappedObject.ToString();
        }

        /// <summary>
        /// Raises the GetValue event.
        /// </summary>
        /// <param name="e">A PropertySpecEventArgs that contains the event data.</param>
        /// <param name="propertySpec"></param>
        internal void OnGetValue(PropertySpecEventArgs e, PropertySpec propertySpec)
        {
            var attributeList = new List<Attribute>();
            attributeList.AddRange(propertySpec.Attributes.ToList());

            //check all of the attributes: if we find a dynamic one, evaluate it and possibly add/overwrite a static attribute
            foreach (Attribute customAttribute in propertySpec.Attributes)
            {
                if (customAttribute is DynamicReadOnlyAttribute)
                {
                    attributeList.RemoveAll(x => x is ReadOnlyAttribute);

                    if (DynamicReadOnlyAttribute.IsReadOnly(WrappedObject, propertySpec.Name))
                    {
                        //condition is true: the dynamic attribute should be applied (as static attribute)
                        attributeList.Add(new ReadOnlyAttribute(true)); //add static read only attribute
                    }
                }

                if (customAttribute is DynamicVisibleAttribute)
                {
                    attributeList.RemoveAll(x => x is BrowsableAttribute);

                    if (!DynamicVisibleAttribute.IsVisible(WrappedObject, propertySpec.Name))
                    {
                        attributeList.Add(new BrowsableAttribute(false));
                    }
                }
            }

            propertySpec.Attributes = attributeList.ToArray();

            var propertyInfo = WrappedObject.GetType().GetProperty(propertySpec.Name);
            var value = propertyInfo.GetValue(WrappedObject, null);

            var isNestedPropertiesObject = IsNestedExpandablePropertiesObject(propertyInfo);

            // if nested properties object, wrap in DynamicPropertyBag to provide support for things like DynamicReadOnly
            e.Value = isNestedPropertiesObject ? new DynamicPropertyBag(value) : value;
        }

        /// <summary>
        /// Raises the SetValue event.
        /// </summary>
        /// <param name="propertyName"></param>
        /// <param name="propertyValue"></param>
        internal void OnSetValue(string propertyName, object propertyValue)
        {
            WrappedObject.GetType().GetProperty(propertyName).SetValue(WrappedObject, propertyValue, null);
        }

        private void Initialize(object propertyObject)
        {
            WrappedObject = propertyObject;

            foreach (var propertyInfo in propertyObject.GetType().GetProperties().OrderBy(x => x.MetadataToken).ToArray())
            {
                var propertySpec = GetProperySpecForProperty(propertyInfo);
                Properties.Add(propertySpec);
            }
        }

        private static PropertySpec GetProperySpecForProperty(PropertyInfo propertyInfo)
        {
            var propertySpec = new PropertySpec(propertyInfo.Name, propertyInfo.PropertyType);

            var attributeList = new List<Attribute>();
            foreach (object attrib in propertyInfo.GetCustomAttributes(true))
            {
                if (attrib is Attribute)
                {
                    attributeList.Add(attrib as Attribute);
                }
            }

            if (propertyInfo.GetSetMethod() == null)
            {
                attributeList.Add(new ReadOnlyAttribute(true));
            }
            propertySpec.Attributes = attributeList.ToArray();

            return propertySpec;
        }

        /// <summary>
        /// Determines if the property represents nested object properties, by checking for an ExpandableObjectConverter type converter.
        /// </summary>
        /// <param name="propertyInfo"></param>
        /// <returns></returns>
        private bool IsNestedExpandablePropertiesObject(PropertyInfo propertyInfo)
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

        #region ICustomTypeDescriptor explicit interface definitions

        #region Implementations delegated to System.ComponentModel.TypeDescriptor

        public AttributeCollection GetAttributes()
        {
            return TypeDescriptor.GetAttributes(this, true);
        }

        public string GetClassName()
        {
            return TypeDescriptor.GetClassName(this, true);
        }

        public string GetComponentName()
        {
            return TypeDescriptor.GetComponentName(this, true);
        }

        public System.ComponentModel.TypeConverter GetConverter()
        {
            return TypeDescriptor.GetConverter(this, true);
        }

        public EventDescriptor GetDefaultEvent()
        {
            return TypeDescriptor.GetDefaultEvent(this, true);
        }

        public object GetEditor(Type editorBaseType)
        {
            return TypeDescriptor.GetEditor(this, editorBaseType, true);
        }

        public EventDescriptorCollection GetEvents()
        {
            return TypeDescriptor.GetEvents(this, true);
        }

        public EventDescriptorCollection GetEvents(Attribute[] attributes)
        {
            return TypeDescriptor.GetEvents(this, attributes, true);
        }

        public PropertyDescriptorCollection GetProperties()
        {
            return GetProperties(new Attribute[0]);
        }

        #endregion

        public PropertyDescriptor GetDefaultProperty()
        {
            return Properties.Count > 0 ? new PropertySpecDescriptor(Properties[0], this, Properties[0].Name, null) : null;
        }

        public PropertyDescriptorCollection GetProperties(Attribute[] attributes)
        {
            // Rather than passing this function on to the default TypeDescriptor,
            // which would return the actual properties of PropertyBag, I construct
            // a list here that contains property descriptors for the elements of the
            // Properties list in the bag.
            var props = new List<PropertySpecDescriptor>();
            var propsToOrder = new List<Tuple<int, PropertySpecDescriptor>>();

            foreach (PropertySpec property in Properties)
            {
                var attrs = new ArrayList();

                // Additionally, append the custom attributes associated with the
                // PropertySpec, if any.
                if (property.Attributes != null)
                {
                    attrs.AddRange(property.Attributes);
                }

                Attribute[] attrArray = (Attribute[])attrs.ToArray(typeof(Attribute));

                // Create a new property descriptor for the property item, and add
                // it to the list.
                var pd = new PropertySpecDescriptor(property, this, property.Name, attrArray);

                var propertyOrderAttribute = property.Attributes != null ? property.Attributes.OfType<PropertyOrderAttribute>().FirstOrDefault() : null;
                if (propertyOrderAttribute != null)
                {
                    propsToOrder.Add(new Tuple<int, PropertySpecDescriptor>(propertyOrderAttribute.Order, pd));
                }
                else
                {
                    props.Add(pd);
                }
            }

            var orderedProperties = propsToOrder.OrderBy(p => p.Item1).Select(p => p.Item2).ToList();

            // Convert the list of PropertyDescriptors to a collection that the
            // ICustomTypeDescriptor can use, and return it.
            var browsableAttribute = attributes.OfType<BrowsableAttribute>().FirstOrDefault();

            var propertySpecDescriptors = (browsableAttribute != null)
                                              ? orderedProperties.Concat(props).Where(p => p.IsBrowsable == browsableAttribute.Browsable)
                                              : orderedProperties.Concat(props);

            return new PropertyDescriptorCollection(propertySpecDescriptors.ToArray());
        }

        public object GetPropertyOwner(PropertyDescriptor pd)
        {
            return this;
        }

        #endregion
    }
}