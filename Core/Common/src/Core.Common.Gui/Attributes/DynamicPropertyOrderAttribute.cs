﻿// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
using System.Reflection;
using Core.Common.Gui.Properties;

namespace Core.Common.Gui.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class DynamicPropertyOrderAttribute : Attribute
    {
        public static int Order(object obj, string propertyName)
        {
            if (string.IsNullOrEmpty(propertyName))
            {
                return 0;
            }

            if (!IsPropertyDynamicOrdered(obj, propertyName))
            {
                return 0;
            }

            var propertyOrder = DynamicPropertyOrderMethodAttribute.CreatePropertyOrderMethod(obj);

            return propertyOrder(propertyName);
        }

        private static bool IsPropertyDynamicOrdered(object obj, string propertyName)
        {
            MemberInfo propertyInfo = obj.GetType().GetProperty(propertyName);
            if (propertyInfo == null)
            {
                throw new MissingMemberException(string.Format(Resources.Could_not_find_property_0_on_type_1_, propertyName,
                                                               obj.GetType()));
            }

            return IsDefined(propertyInfo, typeof(DynamicPropertyOrderAttribute));
        }
    }
}