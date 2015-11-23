using System;
using System.Linq;
using System.Reflection;
using Core.Common.Utils.Properties;

namespace Core.Common.Utils.ComponentModel
{
    /// <summary>
    /// Marks property as a conditional read-only property. When this attribute is used on a property - a class containing that property
    /// must contain a single validation method (argument propertyName as string, returns bool) marked using [DynamicReadOnlyValidationMethod] 
    /// attribute.
    /// </summary>
    public class DynamicReadOnlyAttribute : Attribute
    {
        public static bool IsDynamicReadOnly(object obj, string propertyName)
        {
            if (string.IsNullOrEmpty(propertyName))
            {
                return false;
            }

            // todo: caching!!!!
            var propertyInfo = obj.GetType().GetProperty(propertyName);
            if (propertyInfo == null)
            {
                throw new MissingMemberException(string.Format(Resource.Could_not_find_property_0_on_type_1_, propertyName,
                                                               obj.GetType()));
            }

            if (!propertyInfo.GetCustomAttributes(typeof(DynamicReadOnlyAttribute), false).Any())
            {
                return false;
            }

            var validationMethod = GetDynamicReadOnlyValidationMethod(obj);

            // check if property should be read-only
            if (validationMethod == null)
            {
                throw new MissingMethodException(
                    String.Format(Resource.DynamicReadOnlyAttribute_IsDynamicReadOnly_0_uses_DynanamicReadOnlyAttribute_but_does_not_have_method_marked_using_DynamicReadOnlyValidationMethodAttribute, obj));
            }

            var shouldBeReadOnly = (bool) validationMethod.Invoke(obj, new[]
            {
                propertyName
            });

            return shouldBeReadOnly;
        }

        private static MethodInfo GetDynamicReadOnlyValidationMethod(object o)
        {
            var type = o.GetType();

            var validationMethods =
                type.GetMethods().Where(
                    methodInfo =>
                    methodInfo.GetCustomAttributes(false).Any(a => a is DynamicReadOnlyValidationMethodAttribute)).
                     ToList();

            if (!validationMethods.Any())
            {
                throw new MissingMethodException(string.Format(Resource.DynamicReadOnlyAttribute_GetDynamicReadOnlyValidationMethod_DynamicReadOnlyValidationMethod_not_found_or_not_public_class_0_, type));
            }

            if (validationMethods.Count() > 1)
            {
                throw new MissingMethodException(string.Format(Resource.DynamicReadOnlyAttribute_GetDynamicReadOnlyValidationMethod_Only_one_DynamicReadOnlyValidationMethod_is_allowed_per_class_0_, type));
            }

            var validationMethod = validationMethods.First();

            // check return type and arguments
            if (validationMethod.ReturnType != typeof(bool))
            {
                throw new MissingMethodException(string.Format(Resource.DynamicReadOnlyAttribute_GetDynamicReadOnlyValidationMethod_DynamicReadOnlyValidationMethod_must_use_bool_as_a_return_type_class_0_, type));
            }

            if (validationMethod.GetParameters().Length != 1)
            {
                throw new MissingMethodException(string.Format(Resource.DynamicReadOnlyAttribute_GetDynamicReadOnlyValidationMethod_DynamicReadOnlyValidationMethod_has_incorrect_number_of_arguments_Should_be_1_of_type_string_class_0_, type));
            }

            if (validationMethod.GetParameters()[0].ParameterType != typeof(string))
            {
                throw new MissingMethodException(string.Format(Resource.DynamicReadOnlyAttribute_GetDynamicReadOnlyValidationMethod_DynamicReadOnlyValidationMethod_has_incorrect_argument_type_Should_be_of_type_string_class_0_, type));
            }

            return validationMethod;
        }
    }
}