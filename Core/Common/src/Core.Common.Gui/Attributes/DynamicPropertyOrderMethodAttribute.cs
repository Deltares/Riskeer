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
using System.Linq;
using System.Reflection;
using CoreCommonGuiResources = Core.Common.Gui.Properties.Resources;

namespace Core.Common.Gui.Attributes
{
    [AttributeUsage(AttributeTargets.Method)]
    public sealed class DynamicPropertyOrderMethodAttribute : Attribute
    {
        public delegate int PropertyOrder(string propertyName);

        public static PropertyOrder CreatePropertyOrderMethod(object target)
        {
            var methodInfo = GetPropertyOrderMethod(target);
            ValidateMethodInfo(methodInfo);
            return CreatePropertyOrderDelegate(target, methodInfo);
        }

        private static PropertyOrder CreatePropertyOrderDelegate(object target, MethodInfo methodInfo)
        {
            return (PropertyOrder) Delegate.CreateDelegate(typeof(PropertyOrder), target, methodInfo);
        }

        private static void ValidateMethodInfo(MethodInfo methodInfo)
        {
            if (methodInfo.ReturnType != typeof(int))
            {
                var message = string.Format(CoreCommonGuiResources.DynamicPropertyOrderMethod_must_return_int_on_Class_0_,
                                            methodInfo.DeclaringType);
                throw new MissingMethodException(message);
            }

            ParameterInfo[] parameterInfos = methodInfo.GetParameters();
            if (parameterInfos.Length != 1)
            {
                var message = string.Format(CoreCommonGuiResources.DynamicPropertyOrderMethod_incorrect_argument_count_must_be_one_string_argument_on_Class_0_,
                                            methodInfo.DeclaringType);
                throw new MissingMethodException(message);
            }

            if (parameterInfos[0].ParameterType != typeof(string))
            {
                var message = string.Format(CoreCommonGuiResources.DynamicPropertyOrderMethod_must_have_string_argument_on_Class_0_,
                                            methodInfo.DeclaringType);
                throw new MissingMethodException(message);
            }
        }

        private static MethodInfo GetPropertyOrderMethod(object obj)
        {
            var propertyOrderMethods = obj.GetType().GetMethods()
                                          .Where(methodInfo => IsDefined(methodInfo, typeof(DynamicPropertyOrderMethodAttribute)))
                                          .ToArray();

            if (propertyOrderMethods.Length == 0)
            {
                var message = string.Format(CoreCommonGuiResources.DynamicPropertyOrderMethod_not_found_or_not_public_on_Class_0_,
                                            obj.GetType());
                throw new MissingMethodException(message);
            }

            if (propertyOrderMethods.Length > 1)
            {
                var message = string.Format(CoreCommonGuiResources.DynamicPropertyOrderMethod_only_one_allowed_per_Class_0_,
                                            obj.GetType());
                throw new MissingMethodException(message);
            }

            return propertyOrderMethods[0];
        }
    }
}