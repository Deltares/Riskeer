// Copyright (C) Stichting Deltares 2016. All rights reserved.
//
// This file is part of Ringtoets.
//
// Ringtoets is free software: you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with this program. If not, see <http://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of
// Stichting Deltares and remain full property of Stichting Deltares at all times.
// All rights reserved.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Core.Common.Gui.Attributes;
using Core.Common.Gui.Forms.PropertyGridView;

namespace Core.Common.Gui.PropertyBag
{
    /// <summary>
    /// Defines a custom type descriptor for an object to be used as view-model for <see cref="PropertyGridView"/>.
    /// It processes the special attributes defined in <c>Core.Common.Gui.Attributes</c>
    /// to dynamically affect property order or adding/removing <see cref="Attributes"/>.
    /// </summary>
    /// <remarks>This class makes sure the following special attributes on properties are processed:
    /// <list type="bullet">
    /// <item><see cref="DynamicReadOnlyAttribute"/></item>
    /// <item><see cref="DynamicVisibleAttribute"/></item>
    /// <item><see cref="PropertyOrderAttribute"/></item>
    /// <item><see cref="DynamicPropertyOrderAttribute"/></item>
    /// </list>
    /// </remarks>
    public class DynamicPropertyBag : ICustomTypeDescriptor
    {
        /// <summary>
        /// Instantiates a new instance of <see cref="DynamicPropertyBag"/>, wrapping another
        /// object and exposing properties for that object.
        /// </summary>
        /// <param name="propertyObject">The object to be wrapped.</param>
        public DynamicPropertyBag(object propertyObject)
        {
            Properties = new HashSet<PropertySpec>();
            WrappedObject = propertyObject;

            foreach (var propertyInfo in propertyObject.GetType().GetProperties()
                                                       .OrderBy(x => x.MetadataToken))
            {
                Properties.Add(new PropertySpec(propertyInfo));
            }
        }

        /// <summary>
        /// Gets the collection of properties contained within this <see cref="DynamicPropertyBag"/>.
        /// </summary>
        public ICollection<PropertySpec> Properties { get; private set; }

        /// <summary>
        /// Gets the object wrapped inside this <see cref="DynamicPropertyBag"/>.
        /// </summary>
        public object WrappedObject { get; private set; }

        public override string ToString()
        {
            return WrappedObject.ToString();
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

        public TypeConverter GetConverter()
        {
            return TypeDescriptor.GetConverter(this, true);
        }

        public EventDescriptor GetDefaultEvent()
        {
            return TypeDescriptor.GetDefaultEvent(this, true);
        }

        public EventDescriptorCollection GetEvents()
        {
            return TypeDescriptor.GetEvents(this, true);
        }

        public EventDescriptorCollection GetEvents(Attribute[] attributes)
        {
            return TypeDescriptor.GetEvents(this, attributes, true);
        }

        public object GetEditor(Type editorBaseType)
        {
            return TypeDescriptor.GetEditor(this, editorBaseType, true);
        }

        #endregion

        public PropertyDescriptor GetDefaultProperty()
        {
            return Properties.Count > 0 ? new PropertySpecDescriptor(Properties.First(), WrappedObject) : null;
        }

        public PropertyDescriptorCollection GetProperties()
        {
            return GetProperties(new Attribute[0]);
        }

        public PropertyDescriptorCollection GetProperties(Attribute[] attributes)
        {
            var propertyDescriptorsToReturn = Properties.Select(p => new PropertySpecDescriptor(p, WrappedObject))
                                                        .Where(t => ShouldDescriptorBeReturned(t, attributes));

            var propertySpecDescriptors = OrderPropertyDescriptors(propertyDescriptorsToReturn);
            return new PropertyDescriptorCollection(propertySpecDescriptors);
        }

        private PropertyDescriptor[] OrderPropertyDescriptors(IEnumerable<PropertyDescriptor> propertyDescriptorsToReturn)
        {
            var unorderedProperties = new List<PropertyDescriptor>();
            var propertiesWithOrdering = new List<Tuple<int, PropertyDescriptor>>();

            foreach (PropertyDescriptor pd in propertyDescriptorsToReturn)
            {
                var propertyOrderAttribute = pd.Attributes.OfType<PropertyOrderAttribute>().FirstOrDefault();
                if (propertyOrderAttribute != null)
                {
                    propertiesWithOrdering.Add(Tuple.Create(propertyOrderAttribute.Order, pd));
                    continue;
                }

                var dynamicPropertyOrderAttribute = pd.Attributes.OfType<DynamicPropertyOrderAttribute>().FirstOrDefault();
                if (dynamicPropertyOrderAttribute != null)
                {
                    propertiesWithOrdering.Add(Tuple.Create(DynamicPropertyOrderAttribute.PropertyOrder(WrappedObject, pd.Name), pd));
                    continue;
                }

                unorderedProperties.Add(pd);
            }
            var orderedProperties = propertiesWithOrdering.OrderBy(p => p.Item1).Select(p => p.Item2);

            return orderedProperties.Concat(unorderedProperties).ToArray();
        }

        private static bool ShouldDescriptorBeReturned(PropertyDescriptor propertySpecDescriptor, IEnumerable<Attribute> attributesFilter)
        {
            var browsableAttribute = attributesFilter.OfType<BrowsableAttribute>().FirstOrDefault();
            return browsableAttribute == null || propertySpecDescriptor.IsBrowsable == browsableAttribute.Browsable;
        }

        public object GetPropertyOwner(PropertyDescriptor pd)
        {
            return WrappedObject;
        }

        #endregion
    }
}