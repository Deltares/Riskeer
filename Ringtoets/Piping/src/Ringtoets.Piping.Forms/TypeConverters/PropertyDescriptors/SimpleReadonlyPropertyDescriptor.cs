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
    /// This class defines a simple readonly property item that isn't an actual property 
    /// of an object, but a standalone piece of data. Because the piece of data does not 
    /// belong to some component, the data is readonly and cannot be set or changed.
    /// </summary>
    public class SimpleReadonlyPropertyDescriptorItem : PropertyDescriptor
    {
        private readonly object valueObject;

        /// <summary>
        /// Initializes a new instance of the <see cref="SimpleReadonlyPropertyDescriptorItem"/> class.
        /// </summary>
        /// <param name="displayName">The display name of the data.</param>
        /// <param name="description">The descriptive text associated with the data.</param>
        /// <param name="id">The name of the 'property', allowing it searched for in collections.</param>
        /// <param name="value">The value that should be shown in the value field.</param>
        public SimpleReadonlyPropertyDescriptorItem(string displayName, string description, string id, object value) :
            base(id, new Attribute[]
            {
                new DisplayNameAttribute(displayName),
                new DescriptionAttribute(description),
                new ReadOnlyAttribute(true)
            })
        {
            valueObject = value;
        }

        public override Type ComponentType
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public override bool IsReadOnly
        {
            get
            {
                return true;
            }
        }

        public override Type PropertyType
        {
            get
            {
                return valueObject.GetType();
            }
        }

        public override bool CanResetValue(object component)
        {
            return false;
        }

        public override object GetValue(object component)
        {
            return valueObject;
        }

        public override void ResetValue(object component)
        {
            throw new NotImplementedException();
        }

        public override void SetValue(object component, object value)
        {
            throw new NotImplementedException();
        }

        public override bool ShouldSerializeValue(object component)
        {
            return false;
        }
    }
}