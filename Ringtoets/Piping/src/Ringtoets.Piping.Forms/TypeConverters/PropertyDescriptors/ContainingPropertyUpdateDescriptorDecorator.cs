// Copyright (C) Stichting Deltares 2016. All rights reserved.
//
// This file is part of Ringtoets.
//
// Ringtoets is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program. If not, see <http://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of
// Stichting Deltares and remain full property of Stichting Deltares at all times.
// All rights reserved.

using System;
using System.ComponentModel;

namespace Ringtoets.Piping.Forms.TypeConverters.PropertyDescriptors
{
    /// <summary>
    /// Decorates a PropertyDescriptor with an override for SetValue to have the property exposing the 
    /// owner of the property, captured by the PropertyDescriptor, set to that owner's instance as well. 
    /// </summary>
    /// <remarks>Use this class for PropertyDescriptors that allows for changes to properties of a 
    /// considered composed part to also set a new value for the composite owner of that part.</remarks>
    public class ContainingPropertyUpdateDescriptorDecorator : PropertyDescriptor
    {
        private readonly object source;
        private readonly PropertyDescriptor sourceContainingProperty;
        private readonly PropertyDescriptor originalPropertyDescriptor;

        /// <summary>
        /// Creates a new <see cref="ContainingPropertyUpdateDescriptorDecorator"/>
        /// </summary>
        /// <param name="propertyDescription">The original property description.</param>
        /// <param name="source">The source object, which contains the property described by <paramref name="propertyDescription"/>.</param>
        /// <param name="containingProperty">The property which contains the source of the <paramref name="propertyDescription"/>.</param>
        public ContainingPropertyUpdateDescriptorDecorator(PropertyDescriptor propertyDescription, object source, PropertyDescriptor containingProperty)
            : base(propertyDescription)
        {
            this.source = source;
            sourceContainingProperty = containingProperty;
            originalPropertyDescriptor = propertyDescription;
        }

        public override void SetValue(object component, object value)
        {
            originalPropertyDescriptor.SetValue(component, value);
            if (source != null && sourceContainingProperty != null)
            {
                sourceContainingProperty.SetValue(source, component);
            }
        }

        #region Members delegated to wrapped descriptor

        public override bool CanResetValue(object component)
        {
            return originalPropertyDescriptor.CanResetValue(component);
        }

        public override object GetValue(object component)
        {
            return originalPropertyDescriptor.GetValue(component);
        }

        public override void ResetValue(object component)
        {
            originalPropertyDescriptor.ResetValue(component);
        }

        public override bool ShouldSerializeValue(object component)
        {
            return originalPropertyDescriptor.ShouldSerializeValue(component);
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

        public override string Description
        {
            get
            {
                return originalPropertyDescriptor.Description;
            }
        }

        public override string DisplayName
        {
            get
            {
                return originalPropertyDescriptor.DisplayName;
            }
        }

        public override AttributeCollection Attributes
        {
            get
            {
                return originalPropertyDescriptor.Attributes;
            }
        }

        public override string Category
        {
            get
            {
                return originalPropertyDescriptor.Category;
            }
        }

        public override TypeConverter Converter
        {
            get
            {
                return originalPropertyDescriptor.Converter;
            }
        }

        public override bool DesignTimeOnly
        {
            get
            {
                return originalPropertyDescriptor.DesignTimeOnly;
            }
        }

        public override bool IsBrowsable
        {
            get
            {
                return originalPropertyDescriptor.IsBrowsable;
            }
        }

        public override bool IsLocalizable
        {
            get
            {
                return originalPropertyDescriptor.IsLocalizable;
            }
        }

        public override string Name
        {
            get
            {
                return originalPropertyDescriptor.Name;
            }
        }

        public override bool SupportsChangeEvents
        {
            get
            {
                return originalPropertyDescriptor.SupportsChangeEvents;
            }
        }

        #endregion
    }
}