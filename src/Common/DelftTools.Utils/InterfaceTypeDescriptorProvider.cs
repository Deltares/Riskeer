using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;

namespace DelftTools.Utils
{

    /// <summary>
    /// Provides properties from base interface for derived interface type
    /// todo: check if it can be removed
    /// </summary>
    public class InterfacePropertiesTypeDescriptionProvider : TypeDescriptionProvider
    {
        private readonly TypeDescriptionProvider baseProvider;
        private PropertyDescriptorCollection propCache;
        private FilterCache filterCache;

        public InterfacePropertiesTypeDescriptionProvider(Type t)
        {
            if(!t.IsInterface){throw new ArgumentException("This provider is meant to be used on interface types only");}
            baseProvider = TypeDescriptor.GetProvider(t);
        }

        public override ICustomTypeDescriptor GetTypeDescriptor(Type objectType, object instance)
        {
            return new InterfaceTypeDescriptor(
                this, baseProvider.GetTypeDescriptor(objectType, instance), objectType);
        }

        private class FilterCache
        {
            public Attribute[] Attributes;
            public PropertyDescriptorCollection FilteredProperties;

            public bool IsValid(Attribute[] other)
            {
                if (other == null || Attributes == null)
                {
                    return false;
                }
                if (Attributes.Length != other.Length)
                {
                    return false;
                }
                for (var i = 0; i < other.Length; i++)
                {
                    if (!Attributes[i].Match(other[i]))
                    {
                        return false;
                    }
                }
                return true;
            }
        }


        private class InterfaceTypeDescriptor : CustomTypeDescriptor
        {
            private readonly Type objectType;
            private readonly InterfacePropertiesTypeDescriptionProvider provider;

            public InterfaceTypeDescriptor(InterfacePropertiesTypeDescriptionProvider provider,
                                           ICustomTypeDescriptor descriptor, Type objectType)
                : base(descriptor)
            {
                if (provider == null) throw new ArgumentNullException("provider");
                if (descriptor == null) throw new ArgumentNullException("descriptor");
                if (objectType == null) throw new ArgumentNullException("objectType");
                this.objectType = objectType;
                this.provider = provider;
            }

            public override PropertyDescriptorCollection GetProperties()
            {
                return GetProperties(null);
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="attributes"></param>
            /// <returns></returns>
            public override PropertyDescriptorCollection GetProperties(Attribute[] attributes)
            {
                // Retrieve cached properties and filtered properties
                var filtering = attributes != null && attributes.Length > 0;
                var cache = provider.filterCache;
                var props = provider.propCache;

                // Use a cached version if we can
                if (filtering && cache != null && cache.IsValid(attributes))
                {
                    return cache.FilteredProperties;
                }
                if (!filtering && props != null)
                {
                    return props;
                }

                var properties1 = new Dictionary<string, PropertyDescriptor>();
                {
                    foreach (PropertyDescriptor prop in base.GetProperties())
                    {
                        properties1[prop.Name] = prop;
                    }
                }
                if (objectType.IsInterface)
                {
                    foreach (var interfaceType in objectType.GetInterfaces())
                    {
                        foreach (PropertyDescriptor prop in TypeDescriptor.GetProperties(interfaceType))
                        {
                            properties1[prop.Name] = prop;
                        }
                    }
                }

                var properties = new ArrayList(properties1.Values);

                //Store the updated properties
                props = new PropertyDescriptorCollection(
                    (PropertyDescriptor[])properties.ToArray(typeof(PropertyDescriptor)), false);
                if (filtering)
                {
                    cache = new FilterCache { FilteredProperties = props, Attributes = attributes };
                    provider.filterCache = cache;
                }
                else provider.propCache = props;

                return props;
            }
        }
    }
}

