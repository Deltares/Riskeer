using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using DelftTools.Utils.ComponentModel;

namespace DelftTools.Utils.PropertyBag.Dynamic
{
    /// <summary>
    /// Creates a custom type descriptor for an object using reflection as a property bag. Used for Property Grid.
    /// Additionally it scans the object for any dynamic attributes and processes those, eg checks their condition
    /// at runtime and if met, adds them as static attribute.
    /// </summary>
    public class DynamicPropertyBag : PropertyBag
    {
        private object propertyObject;

        public DynamicPropertyBag(object propertyObject, PropertyInfo[] customPropertyInfos=null)
        {
            Initialize(propertyObject, customPropertyInfos ?? propertyObject.GetType().GetProperties());
        }

        protected void Initialize(object propertyObject, PropertyInfo[] properties)
        {
            this.propertyObject = propertyObject;
            foreach (var propertyInfo in properties)
            {
                var propertySpec = GetProperySpecForProperty(propertyInfo);
                Properties.Add(propertySpec);

            }
        }

        private static PropertySpec GetProperySpecForProperty(PropertyInfo propertyInfo)
        {
            var propertySpec = new PropertySpec(propertyInfo.Name, propertyInfo.PropertyType);
                
            var attributeList = new List<Attribute>();
            foreach(object attrib in propertyInfo.GetCustomAttributes(true))
                if (attrib is Attribute)
                    attributeList.Add(attrib as Attribute);

            if (propertyInfo.GetSetMethod()==null) 
            {
                attributeList.Add(new ReadOnlyAttribute(true));
            }
            propertySpec.Attributes = attributeList.ToArray();
            
            return propertySpec;
        }

        protected override void OnGetValue(PropertySpecEventArgs e)
        {
            base.OnGetValue(e);

            var attributeList = new List<Attribute>();
            attributeList.AddRange(e.Property.Attributes.ToList());

            var setToReadOnly = false;
            EditorAttribute editorAttribute = null;
            DynamicEditorAttribute dynamicEditorAttribute = null;

            //check all of the attributes: if we find a dynamic one, evaluate it and possibly add/overwrite a static attribute
            foreach (Attribute customAttribute in e.Property.Attributes)
            {
                if (customAttribute is DynamicReadOnlyAttribute)
                {
                    attributeList.RemoveAll(x => x is ReadOnlyAttribute);

                    if (DynamicReadOnlyAttribute.IsDynamicReadOnly(propertyObject, e.Property.Name))
                    {
                        //condition is true: the dynamic attribute should be applied (as static attribute)
                        attributeList.Add(new ReadOnlyAttribute(true)); //add static read only attribute
                        setToReadOnly = true;
                    }
                }

                if (customAttribute is DynamicVisibleAttribute)
                {
                    attributeList.RemoveAll(x => x is BrowsableAttribute);

                    if (!DynamicVisibleAttribute.IsDynamicVisible(propertyObject, e.Property.Name))
                    {
                        attributeList.Add(new BrowsableAttribute(false));
                    }
                }

                if (customAttribute is EditorAttribute)
                {
                    editorAttribute = customAttribute as EditorAttribute;
                }
                if (customAttribute is DynamicEditorAttribute)
                {
                    dynamicEditorAttribute = customAttribute as DynamicEditorAttribute;
                }
            }

            if (setToReadOnly && editorAttribute != null)
            {
                // translate EditorAttribute into DynamicEditorAttribute (since the property is readonly)
                attributeList.RemoveAll(x => x is EditorAttribute);
                attributeList.Add(new DynamicEditorAttribute(editorAttribute.EditorTypeName, editorAttribute.EditorBaseTypeName));
            }
            if (!setToReadOnly && dynamicEditorAttribute != null)
            {
                // translate DynamicEditorAttribute into regular EditorAttribute (since the property is NOT readonly)
                attributeList.RemoveAll(x => x is DynamicEditorAttribute);
                try
                {
                    attributeList.Add(new EditorAttribute(Type.GetType(dynamicEditorAttribute.EditorType, true), Type.GetType(dynamicEditorAttribute.EditorBaseType, true)));
                }
                catch (Exception)
                {
                    // nothing we can do about it
                }
            }
            
            e.Property.Attributes = attributeList.ToArray();

            var propertyInfo = propertyObject.GetType().GetProperty(e.Property.Name);
            var value = propertyInfo.GetValue(propertyObject, null);

            var isNestedPropertiesObject = IsNestedExpandablePropertiesObject(propertyInfo);

            // if nested properties object, wrap in DynamicPropertyBag to provide support for things like DynamicReadOnly
            e.Value = isNestedPropertiesObject ? new DynamicPropertyBag(value) : value;
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
                var typeConverterAttributes = propertyInfo.GetCustomAttributes(typeof (TypeConverterAttribute), false);
                foreach (TypeConverterAttribute typeConverterAttribute in typeConverterAttributes)
                {
                    var typeString = typeConverterAttribute.ConverterTypeName;
                    var type = Type.GetType(typeString);
                    if (type != null)
                    {
                        if (typeof (ExpandableObjectConverter) == type)
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

        protected override void OnSetValue(PropertySpecEventArgs e)
        {
            base.OnSetValue(e);

            propertyObject.GetType().GetProperty(e.Property.Name).SetValue(propertyObject,e.Value,null);
        }

        public Type GetContentType()
        {
            return propertyObject.GetType();
        }

        public override string ToString()
        {
            return propertyObject.ToString();
        }

        protected DynamicPropertyBag()
        {
        }
    }
}
