﻿// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of 
// Stichting Deltares and remain full property of Stichting Deltares at all times. 
// All rights reserved.

using System;
using System.ComponentModel;

namespace Ringtoets.Piping.Forms.TypeConverters.PropertyDescriptors
{
    public class RoutedPropertyDescriptor : PropertyDescriptor
    {
        private readonly Func<object, object> rerouteToActualPropertyOwner;
        private readonly PropertyDescriptor originalPropertyDescriptor;

        public RoutedPropertyDescriptor(PropertyDescriptor descr, Func<object, object> routerFunction)
            : base(descr)
        {
            originalPropertyDescriptor = descr;
            rerouteToActualPropertyOwner = routerFunction;
        }

        public override bool CanResetValue(object component)
        {
            return originalPropertyDescriptor.CanResetValue(rerouteToActualPropertyOwner(component));
        }

        public override object GetValue(object component)
        {
            return originalPropertyDescriptor.GetValue(rerouteToActualPropertyOwner(component));
        }

        public override void ResetValue(object component)
        {
            originalPropertyDescriptor.ResetValue(rerouteToActualPropertyOwner(component));
        }

        public override void SetValue(object component, object value)
        {
            originalPropertyDescriptor.SetValue(rerouteToActualPropertyOwner(component), value);
        }

        public override bool ShouldSerializeValue(object component)
        {
            return originalPropertyDescriptor.ShouldSerializeValue(rerouteToActualPropertyOwner(component));
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
    }
}