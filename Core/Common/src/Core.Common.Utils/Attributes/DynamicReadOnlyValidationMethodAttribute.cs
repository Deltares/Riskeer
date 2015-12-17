using System;
using System.Linq;
using System.Reflection;

using CoreCommonUtilsResources = Core.Common.Utils.Properties.Resource;

namespace Core.Common.Utils.Attributes
{
    /// <summary>
    /// Marks a method to be used to determine if a property value can be set or not. The
    /// method should be public and have the signature of <see cref="IsPropertyReadOnly"/>.
    /// </summary>
    /// <seealso cref="DynamicReadOnlyAttribute"/>
    public class DynamicReadOnlyValidationMethodAttribute : Attribute
    {
        /// <summary>
        /// Required method signature when marking a method with <see cref="DynamicReadOnlyValidationMethodAttribute"/>.
        /// </summary>
        /// <param name="propertyName">Name of the property to be checked.</param>
        /// <returns>True if the referred property should be read-only, false if it should be editable.</returns>
        public delegate bool IsPropertyReadOnly(string propertyName);

        /// <summary>
        /// Creates a delegate that can be used to determine if a property should be read-only.
        /// </summary>
        /// <param name="target">The object instance declaring the validation method.</param>
        /// <returns>The delegate.</returns>
        /// <exception cref="System.MissingMethodException">When there isn't a single method
        /// declared on <paramref name="target"/> marked with <see cref="DynamicReadOnlyValidationMethodAttribute"/>
        /// and/or isn't matching the signature defined by <see cref="IsPropertyReadOnly"/>.</exception>
        public static IsPropertyReadOnly CreateIsReadOnlyMethod(object target)
        {
            var methodInfo = GetIsReadOnlyMethod(target);
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
                var message = String.Format(CoreCommonUtilsResources.DynamicReadOnlyValidationMethod_must_return_bool_on_Class_0_,
                                            methodInfo.DeclaringType);
                throw new MissingMethodException(message);
            }

            ParameterInfo[] parameterInfos = methodInfo.GetParameters();
            if (parameterInfos.Length != 1)
            {
                var message = String.Format(CoreCommonUtilsResources.DynamicReadOnlyValidationMethod_incorrect_argument_count_must_be_one_string_argument_on_Class_0_,
                                            methodInfo.DeclaringType);
                throw new MissingMethodException(message);
            }

            if (parameterInfos[0].ParameterType != typeof(string))
            {
                var message = String.Format(CoreCommonUtilsResources.DynamicReadOnlyValidationMethod_must_have_string_argument_on_Class_0_,
                                            methodInfo.DeclaringType);
                throw new MissingMethodException(message);
            }
        }

        private static MethodInfo GetIsReadOnlyMethod(object obj)
        {
            var validationMethods = obj.GetType().GetMethods()
                                       .Where(methodInfo => IsDefined(methodInfo, typeof(DynamicReadOnlyValidationMethodAttribute)))
                                       .ToArray();

            if (validationMethods.Length == 0)
            {
                var message = String.Format(CoreCommonUtilsResources.DynamicReadOnlyValidationMethod_not_found_or_not_public_on_Class_0_,
                                            obj.GetType());
                throw new MissingMethodException(message);
            }

            if (validationMethods.Length > 1)
            {
                var message = String.Format(CoreCommonUtilsResources.DynamicReadOnlyValidationMethod_only_one_allowed_per_Class_0_,
                                            obj.GetType());
                throw new MissingMethodException(message);
            }

            return validationMethods[0];
        }
    }
}