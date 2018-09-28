// Copyright (C) Stichting Deltares 2018. All rights reserved.
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
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="propertySpec"/>
        /// is <c>null</c>.</exception>
        public PropertySpecDescriptor(PropertySpec propertySpec, object instance)
            : base(ValidateNotNull(propertySpec).Name, propertySpec.Attributes.ToArray())
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
            throw new InvalidOperationException("Do not call 'ResetValue' if 'CanResetValue(object)' returns false.");
        }

        public override object GetValue(object component)
        {
            object propertyValue = item.GetValue(component);
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

        private static PropertySpec ValidateNotNull(PropertySpec propertySpec)
        {
            if (propertySpec == null)
            {
                throw new ArgumentNullException(nameof(propertySpec));
            }

            return propertySpec;
        }
    }
}