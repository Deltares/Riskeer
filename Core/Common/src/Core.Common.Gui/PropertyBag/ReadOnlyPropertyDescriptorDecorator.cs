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

namespace Core.Common.Gui.PropertyBag
{
    /// <summary>
    /// A decorator <see cref="PropertyDescriptor"/> that forces <see cref="IsReadOnly"/>
    /// to true regardless of the wrapped <see cref="PropertyDescriptor"/>.
    /// </summary>
    public class ReadOnlyPropertyDescriptorDecorator : PropertyDescriptor
    {
        private readonly PropertyDescriptor wrappedPropertyDescriptor;

        /// <summary>
        /// Initializes a new instance of the <see cref="ReadOnlyPropertyDescriptorDecorator"/> class.
        /// </summary>
        /// <param name="propertyDescriptor">The property descriptor to be wrapped.</param>
        public ReadOnlyPropertyDescriptorDecorator(PropertyDescriptor propertyDescriptor) : base(propertyDescriptor)
        {
            wrappedPropertyDescriptor = propertyDescriptor;
        }

        public override bool IsReadOnly
        {
            get
            {
                return true;
            }
        }

        #region Methods and Properties delegates to wrapped property descriptor

        public override bool CanResetValue(object component)
        {
            return wrappedPropertyDescriptor.CanResetValue(component);
        }

        public override object GetValue(object component)
        {
            return wrappedPropertyDescriptor.GetValue(component);
        }

        public override void ResetValue(object component)
        {
            wrappedPropertyDescriptor.ResetValue(component);
        }

        public override void SetValue(object component, object value)
        {
            wrappedPropertyDescriptor.SetValue(component, value);
        }

        public override bool ShouldSerializeValue(object component)
        {
            return wrappedPropertyDescriptor.ShouldSerializeValue(component);
        }

        public override AttributeCollection Attributes
        {
            get
            {
                return wrappedPropertyDescriptor.Attributes;
            }
        }

        public override string Category
        {
            get
            {
                return wrappedPropertyDescriptor.Category;
            }
        }

        public override Type ComponentType
        {
            get
            {
                return wrappedPropertyDescriptor.ComponentType;
            }
        }

        public override TypeConverter Converter
        {
            get
            {
                return wrappedPropertyDescriptor.Converter;
            }
        }

        public override string Description
        {
            get
            {
                return wrappedPropertyDescriptor.Description;
            }
        }

        public override bool DesignTimeOnly
        {
            get
            {
                return wrappedPropertyDescriptor.DesignTimeOnly;
            }
        }

        public override string DisplayName
        {
            get
            {
                return wrappedPropertyDescriptor.DisplayName;
            }
        }

        public override bool IsBrowsable
        {
            get
            {
                return wrappedPropertyDescriptor.IsBrowsable;
            }
        }

        public override bool IsLocalizable
        {
            get
            {
                return wrappedPropertyDescriptor.IsLocalizable;
            }
        }

        public override string Name
        {
            get
            {
                return wrappedPropertyDescriptor.Name;
            }
        }

        public override Type PropertyType
        {
            get
            {
                return wrappedPropertyDescriptor.PropertyType;
            }
        }

        public override bool SupportsChangeEvents
        {
            get
            {
                return wrappedPropertyDescriptor.SupportsChangeEvents;
            }
        }

        #endregion
    }
}