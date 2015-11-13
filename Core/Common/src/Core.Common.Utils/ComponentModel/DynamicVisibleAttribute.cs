﻿using System;
using System.Linq;
using System.Reflection;
using Core.Common.Utils.Properties;

namespace Core.Common.Utils.ComponentModel
{
    /// <summary>
    /// Hides or shows a property. When this attribute is used on a property -
    /// a class containing that property must contain a single validation method
    /// (argument propertyName as string, return bool) marked using 
    /// <see cref="DynamicVisibleValidationMethodAttribute"/> attribute.
    /// </summary>
    public class DynamicVisibleAttribute : Attribute
    {
        public static bool IsDynamicVisible(object obj, string propertyName)
        {
            if (string.IsNullOrEmpty(propertyName))
            {
                return false;
            }

            // todo: caching!!!!
            var propertyInfo = obj.GetType().GetProperty(propertyName);
            if (propertyInfo == null)
            {
                throw new MissingMemberException(string.Format(Resource.DynamicVisibleAttribute_IsDynamicVisible_Could_not_find_property__0__on_type__1_, propertyName,
                                                               obj.GetType()));
            }

            if (!propertyInfo.GetCustomAttributes(typeof(DynamicVisibleAttribute), false).Any())
            {
                return false;
            }

            var validationMethod = GetDynamicVisibleValidationMethod(obj);

            // check if property should be read-only
            if (validationMethod == null)
            {
                throw new MissingMethodException(
                    String.Format(Resource.DynamicVisibleAttribute_IsDynamicVisible__0__uses_DynamicVisibleAttribute_but_does_not_have_method_marked_using_DynamicVisibleValidationMethodAttribute, obj));
            }

            return (bool) validationMethod.Invoke(obj, new[]
            {
                propertyName
            });
        }

        private static MethodInfo GetDynamicVisibleValidationMethod(object o)
        {
            var type = o.GetType();

            var validationMethods =
                type.GetMethods().Where(
                    methodInfo =>
                    methodInfo.GetCustomAttributes(false).Any(a => a is DynamicVisibleValidationMethodAttribute)).
                     ToList();

            if (!validationMethods.Any())
            {
                throw new MissingMethodException("DynamicVisibleValidationMethodAttribute not found (or not public), class: " + type);
            }

            if (validationMethods.Count() > 1)
            {
                throw new MissingMethodException("Only one DynamicVisibleValidationMethodAttribute is allowed per class: " + type);
            }

            var validationMethod = validationMethods.First();

            // check return type and arguments
            if (validationMethod.ReturnType != typeof(bool))
            {
                throw new MissingMethodException("DynamicVisibleValidationMethodAttribute must use bool as a return type, class: " + type);
            }

            if (validationMethod.GetParameters().Length != 1)
            {
                throw new MissingMethodException("DynamicVisibleValidationMethodAttribute has incorrect number of arguments, should be 1 of type string, class: " + type);
            }

            if (validationMethod.GetParameters()[0].ParameterType != typeof(string))
            {
                throw new MissingMethodException("DynamicVisibleValidationMethodAttribute has incorrect argument type, should be of type string, class: " + type);
            }

            return validationMethod;
        }
    }
}