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
using System.Globalization;
using System.Linq;
using System.Reflection;
using CoreCommonGuiResources = Core.Common.Gui.Properties.Resources;

namespace Core.Common.Gui.Attributes
{
    /// <summary>
    /// Marks a method to be used to determine the order of a property. The method
    /// should be public and have the signature of <see cref="PropertyOrder"/>.
    /// </summary>
    /// <seealso cref="DynamicPropertyOrderAttribute"/>
    [AttributeUsage(AttributeTargets.Method)]
    public sealed class DynamicPropertyOrderEvaluationMethodAttribute : Attribute
    {
        /// <summary>
        /// Required method signature when marking a method with <see cref="DynamicPropertyOrderEvaluationMethodAttribute"/>.
        /// </summary>
        /// <param name="propertyName">The name of the property to be checked.</param>
        /// <returns>The order of the property.</returns>
        public delegate int PropertyOrder(string propertyName);

        /// <summary>
        /// Creates a delegate that can be used to determine the order of a property.
        /// </summary>
        /// <param name="target">The object instance declaring the evaluation method.</param>
        /// <returns>The delegate.</returns>
        /// <exception cref="MissingMethodException">Thrown when there isn't a single method
        /// declared on <paramref name="target"/> marked with <see cref="DynamicPropertyOrderEvaluationMethodAttribute"/>
        /// that is matching the signature defined by <see cref="PropertyOrder"/>.</exception>
        public static PropertyOrder CreatePropertyOrderMethod(object target)
        {
            MethodInfo methodInfo = GetPropertyOrderMethod(target);
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
                string message = string.Format(CultureInfo.CurrentCulture,
                                               CoreCommonGuiResources.DynamicPropertyOrderEvaluationMethod_must_return_int_on_Class_0_,
                                               methodInfo.DeclaringType);
                throw new MissingMethodException(message);
            }

            ParameterInfo[] parameterInfos = methodInfo.GetParameters();
            if (parameterInfos.Length != 1)
            {
                string message = string.Format(CultureInfo.CurrentCulture,
                                               CoreCommonGuiResources.DynamicPropertyOrderEvaluationMethod_incorrect_argument_count_must_be_one_string_argument_on_Class_0_,
                                               methodInfo.DeclaringType);
                throw new MissingMethodException(message);
            }

            if (parameterInfos[0].ParameterType != typeof(string))
            {
                string message = string.Format(CultureInfo.CurrentCulture,
                                               CoreCommonGuiResources.DynamicPropertyOrderEvaluationMethod_must_have_string_argument_on_Class_0_,
                                               methodInfo.DeclaringType);
                throw new MissingMethodException(message);
            }
        }

        private static MethodInfo GetPropertyOrderMethod(object obj)
        {
            MethodInfo[] propertyOrderMethods = obj.GetType().GetMethods()
                                                   .Where(methodInfo => IsDefined(methodInfo, typeof(DynamicPropertyOrderEvaluationMethodAttribute)))
                                                   .ToArray();

            if (propertyOrderMethods.Length == 0)
            {
                string message = string.Format(CultureInfo.CurrentCulture,
                                               CoreCommonGuiResources.DynamicPropertyOrderEvaluationMethod_not_found_or_not_public_on_Class_0_,
                                               obj.GetType());
                throw new MissingMethodException(message);
            }

            if (propertyOrderMethods.Length > 1)
            {
                string message = string.Format(CultureInfo.CurrentCulture,
                                               CoreCommonGuiResources.DynamicPropertyOrderEvaluationMethod_only_one_allowed_per_Class_0_,
                                               obj.GetType());
                throw new MissingMethodException(message);
            }

            return propertyOrderMethods[0];
        }
    }
}