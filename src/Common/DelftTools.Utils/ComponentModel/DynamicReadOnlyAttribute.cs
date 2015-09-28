using System;
using System.Linq;
using System.Reflection;

namespace DelftTools.Utils.ComponentModel
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
                return false;

            // todo: caching!!!!
            var propertyInfo = obj.GetType().GetProperty(propertyName);
            if (propertyInfo == null)
            {
                throw new MissingMemberException(string.Format("Could not find property {0} on type {1}", propertyName,
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
                    String.Format("{0} uses DynanamicReadOnlyAttribute but does not have method marked using DynamicReadOnlyValidationMethodAttribute", obj));
            }

            var shouldBeReadOnly = (bool)validationMethod.Invoke(obj, new[] { propertyName });

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
                throw new MissingMethodException("DynamicReadOnlyValidationMethod not found (or not public), class: " + type);
            }

            if (validationMethods.Count() > 1)
            {
                throw new MissingMethodException("Only one DynamicReadOnlyValidationMethod is allowed per class: " + type);
            }

            var validationMethod = validationMethods.First();

            // check return type and arguments
            if (validationMethod.ReturnType != typeof(bool))
            {
                throw new MissingMethodException("DynamicReadOnlyValidationMethod must use bool as a return type, class: " + type);
            }

            if (validationMethod.GetParameters().Length != 1)
            {
                throw new MissingMethodException("DynamicReadOnlyValidationMethod has incorrect number of arguments, should be 1 of type string, class: " + type);
            }

            if (validationMethod.GetParameters()[0].ParameterType != typeof(string))
            {
                throw new MissingMethodException("DynamicReadOnlyValidationMethod has incorrect argument type, should be of type string, class: " + type);
            }

            return validationMethod;
        }
    }
}
