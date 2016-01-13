using System;
using System.Linq;
using System.Reflection;

using CoreCommonGuiResources = Core.Common.Gui.Properties.Resources;

namespace Core.Common.Gui.Attributes
{
    /// <summary>
    /// Marks a method to be used to determine if a property should be shown or not. The
    /// method should be public and have the signature of <see cref="IsPropertyVisible"/>.
    /// </summary>
    /// <seealso cref="DynamicVisibleAttribute"/>
    [AttributeUsage(AttributeTargets.Method)]
    public sealed class DynamicVisibleValidationMethodAttribute : Attribute
    {
        /// <summary>
        /// Required method signature when marking a method with <see cref="DynamicVisibleValidationMethodAttribute"/>.
        /// </summary>
        /// <param name="propertyName">Name of the property to be checked.</param>
        /// <returns>True if the referred property should be visible, false if it should be hidden.</returns>
        public delegate bool IsPropertyVisible(string propertyName);

        /// <summary>
        /// Creates a delegate that can be used to determine if a property should be visible.
        /// </summary>
        /// <param name="target">The object instance declaring the validation method.</param>
        /// <returns>The delegate.</returns>
        /// <exception cref="System.MissingMethodException">When there isn't a single method
        /// declared on <paramref name="target"/> marked with <see cref="DynamicVisibleValidationMethodAttribute"/>
        /// and/or isn't matching the signature defined by <see cref="IsPropertyVisible"/>.</exception>
        public static IsPropertyVisible CreateIsVisibleMethod(object target)
        {
            var methodInfo = GetIsVisibleMethod(target);
            ValidateMethodInfo(methodInfo);
            return CreateIsPropertyVisibleDelegate(target, methodInfo);
        }

        private static IsPropertyVisible CreateIsPropertyVisibleDelegate(object target, MethodInfo methodInfo)
        {
            return (IsPropertyVisible)Delegate.CreateDelegate(typeof(IsPropertyVisible), target, methodInfo);
        }

        private static void ValidateMethodInfo(MethodInfo methodInfo)
        {
            if (methodInfo.ReturnType != typeof(bool))
            {
                var message = String.Format(CoreCommonGuiResources.DynamicVisibleValidationMethod_must_return_bool_on_Class_0_,
                                            methodInfo.DeclaringType);
                throw new MissingMethodException(message);
            }

            ParameterInfo[] parameterInfos = methodInfo.GetParameters();
            if (parameterInfos.Length != 1)
            {
                var message = String.Format(CoreCommonGuiResources.DynamicVisibleValidationMethod_incorrect_argument_count_must_be_one_string_argument_on_Class_0_,
                                            methodInfo.DeclaringType);
                throw new MissingMethodException(message);
            }

            if (parameterInfos[0].ParameterType != typeof(string))
            {
                var message = String.Format(CoreCommonGuiResources.DynamicVisibleValidationMethod_must_have_string_argument_on_Class_0_,
                                            methodInfo.DeclaringType);
                throw new MissingMethodException(message);
            }
        }

        private static MethodInfo GetIsVisibleMethod(object obj)
        {
            var validationMethods = obj.GetType().GetMethods()
                                       .Where(methodInfo => IsDefined(methodInfo, typeof(DynamicVisibleValidationMethodAttribute)))
                                       .ToArray();

            if (validationMethods.Length == 0)
            {
                var message = String.Format(CoreCommonGuiResources.DynamicVisibleValidationMethod_not_found_or_not_public_on_Class_0_,
                                            obj.GetType());
                throw new MissingMethodException(message);
            }

            if (validationMethods.Length > 1)
            {
                var message = String.Format(CoreCommonGuiResources.DynamicVisibleValidationMethod_only_one_allowed_per_Class_0_,
                                            obj.GetType());
                throw new MissingMethodException(message);
            }

            return validationMethods[0];
        }
    }
}