// Copyright (C) Stichting Deltares 2019. All rights reserved.
//
// This file is part of Riskeer.
//
// Riskeer is free software: you can redistribute it and/or modify
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
    /// Marks a method to be used to determine if a property value can be set or not. The
    /// method should be public and have the signature of <see cref="IsPropertyReadOnly"/>.
    /// </summary>
    /// <seealso cref="DynamicReadOnlyAttribute"/>
    [AttributeUsage(AttributeTargets.Method)]
    public sealed class DynamicReadOnlyValidationMethodAttribute : Attribute
    {
        /// <summary>
        /// Required method signature when marking a method with <see cref="DynamicReadOnlyValidationMethodAttribute"/>.
        /// </summary>
        /// <param name="propertyName">The name of the property to be checked.</param>
        /// <returns><c>true</c> if the referred property should be read-only, <c>false</c> if it should be editable.</returns>
        public delegate bool IsPropertyReadOnly(string propertyName);

        /// <summary>
        /// Creates a delegate that can be used to determine if a property should be read-only.
        /// </summary>
        /// <param name="target">The object instance declaring the validation method.</param>
        /// <returns>The delegate.</returns>
        /// <exception cref="MissingMethodException">Thrown when there isn't a single method
        /// declared on <paramref name="target"/> marked with <see cref="DynamicReadOnlyValidationMethodAttribute"/>
        /// that is matching the signature defined by <see cref="IsPropertyReadOnly"/>.</exception>
        public static IsPropertyReadOnly CreateIsReadOnlyMethod(object target)
        {
            MethodInfo methodInfo = GetIsReadOnlyMethod(target);
            ValidateMethodInfo(methodInfo);
            return CreateIsPropertyReadOnlyDelegate(target, methodInfo);
        }

        private static IsPropertyReadOnly CreateIsPropertyReadOnlyDelegate(object target, MethodInfo methodInfo)
        {
            return (IsPropertyReadOnly) Delegate.CreateDelegate(typeof(IsPropertyReadOnly), target, methodInfo);
        }

        private static void ValidateMethodInfo(MethodInfo methodInfo)
        {
            if (methodInfo.ReturnType != typeof(bool))
            {
                string message = string.Format(CultureInfo.CurrentCulture,
                                               CoreCommonGuiResources.DynamicReadOnlyValidationMethod_must_return_bool_on_Class_0_,
                                               methodInfo.DeclaringType);
                throw new MissingMethodException(message);
            }

            ParameterInfo[] parameterInfos = methodInfo.GetParameters();
            if (parameterInfos.Length != 1)
            {
                string message = string.Format(CultureInfo.CurrentCulture,
                                               CoreCommonGuiResources.DynamicReadOnlyValidationMethod_incorrect_argument_count_must_be_one_string_argument_on_Class_0_,
                                               methodInfo.DeclaringType);
                throw new MissingMethodException(message);
            }

            if (parameterInfos[0].ParameterType != typeof(string))
            {
                string message = string.Format(CultureInfo.CurrentCulture,
                                               CoreCommonGuiResources.DynamicReadOnlyValidationMethod_must_have_string_argument_on_Class_0_,
                                               methodInfo.DeclaringType);
                throw new MissingMethodException(message);
            }
        }

        private static MethodInfo GetIsReadOnlyMethod(object obj)
        {
            MethodInfo[] validationMethods = obj.GetType().GetMethods()
                                                .Where(methodInfo => IsDefined(methodInfo, typeof(DynamicReadOnlyValidationMethodAttribute)))
                                                .ToArray();

            if (validationMethods.Length == 0)
            {
                string message = string.Format(CultureInfo.CurrentCulture,
                                               CoreCommonGuiResources.DynamicReadOnlyValidationMethod_not_found_or_not_public_on_Class_0_,
                                               obj.GetType());
                throw new MissingMethodException(message);
            }

            if (validationMethods.Length > 1)
            {
                string message = string.Format(CultureInfo.CurrentCulture,
                                               CoreCommonGuiResources.DynamicReadOnlyValidationMethod_only_one_allowed_per_Class_0_,
                                               obj.GetType());
                throw new MissingMethodException(message);
            }

            return validationMethods[0];
        }
    }
}