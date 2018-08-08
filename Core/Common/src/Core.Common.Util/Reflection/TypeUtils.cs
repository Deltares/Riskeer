// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Reflection;
using Core.Common.Util.Attributes;

namespace Core.Common.Util.Reflection
{
    /// <summary>
    /// Helper methods dealing with <see cref="Type"/> and reflection.
    /// </summary>
    public static class TypeUtils
    {
        /// <summary>
        /// Gets the string representation of an enum value, taking <see cref="ResourcesDisplayNameAttribute"/>
        /// into account.
        /// </summary>
        /// <typeparam name="TEnum">The type of the enum.</typeparam>
        /// <param name="enumValue">The value of the enum.</param>
        /// <returns>The display name of the enum value.</returns>
        /// <exception cref="InvalidEnumArgumentException">Thrown when <paramref name="enumValue"/>
        /// is an invalid value for <typeparamref name="TEnum"/>.</exception>
        public static string GetDisplayName<TEnum>(TEnum enumValue) where TEnum : IConvertible
        {
            string valueString = enumValue.ToString(CultureInfo.InvariantCulture);
            FieldInfo fieldInfo = typeof(TEnum).GetField(valueString);
            if (fieldInfo == null)
            {
                throw new InvalidEnumArgumentException(nameof(enumValue), Convert.ToInt32(enumValue), typeof(TEnum));
            }
            var resourcesDisplayNameAttribute = (ResourcesDisplayNameAttribute) Attribute.GetCustomAttribute(fieldInfo, typeof(ResourcesDisplayNameAttribute));
            return resourcesDisplayNameAttribute != null ?
                       resourcesDisplayNameAttribute.DisplayName :
                       valueString;
        }

        /// <summary>
        /// Checks if a type implements, inherits from or is a certain other type.
        /// </summary>
        /// <typeparam name="T">The type to check for.</typeparam>
        /// <param name="thisType">Type to check.</param>
        /// <returns><c>true</c> if <paramref name="thisType"/> is the same type as <typeparamref name="T"/>,
        /// or has that as (one of) its supertypes.</returns>
        /// <seealso cref="Implements(Type,Type)"/>
        public static bool Implements<T>(this Type thisType)
        {
            return typeof(T).IsAssignableFrom(thisType);
        }

        /// <summary>
        /// Checks if a type implements, inherits from or is a certain other type.
        /// </summary>
        /// <param name="thisType">Type to check.</param>
        /// <param name="type">The type to check for.</param>
        /// <returns><c>true</c> if <paramref name="thisType"/> is the same type as <paramref name="type"/>,
        /// or has that as (one of) its supertypes.</returns>
        /// <seealso cref="Implements{T}"/>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="type"/> is <c>null</c>.</exception>
        public static bool Implements(this Type thisType, Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }
            return type.IsAssignableFrom(thisType);
        }

        /// <summary>
        /// Gets the value of a field of an instance.
        /// </summary>
        /// <typeparam name="T">Type of the field.</typeparam>
        /// <param name="instance">Instance holding the field. Cannot be <c>null</c>.</param>
        /// <param name="fieldName">Name of the field.</param>
        /// <returns>The value of the field.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="instance"/>
        /// doesn't have a field with the name <paramref name="fieldName"/>.</exception>
        /// <exception cref="ArgumentNullException">Thrown when any input argument is <c>null</c>.</exception>
        /// <remarks>This method can be used for fields of any visibility.</remarks>
        public static T GetField<T>(object instance, string fieldName)
        {
            if (instance == null)
            {
                throw new ArgumentNullException(nameof(instance));
            }
            FieldInfo fieldInfo = GetFieldInfo(instance.GetType(), fieldName);
            if (fieldInfo == null)
            {
                throw new ArgumentOutOfRangeException(nameof(fieldName));
            }

            return (T) fieldInfo.GetValue(instance);
        }

        /// <summary>
        /// Gets the value of a property of an instance.
        /// </summary>
        /// <typeparam name="T">Type of the property.</typeparam>
        /// <param name="instance">Instance holding the property. Cannot be <c>null</c>.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns>The value of the property.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="instance"/>
        /// doesn't have a property with the name <paramref name="propertyName"/>.</exception>
        /// <exception cref="ArgumentNullException">Thrown when any input argument is <c>null</c>.</exception>
        /// <remarks>This method can be used for properties of any visibility.</remarks>
        public static T GetProperty<T>(object instance, string propertyName)
        {
            if (instance == null)
            {
                throw new ArgumentNullException(nameof(instance));
            }
            PropertyInfo propertyInfo =
                instance.GetType().GetProperty(propertyName, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);
            if (propertyInfo == null)
            {
                throw new ArgumentOutOfRangeException(nameof(propertyInfo));
            }
            MethodInfo getter = propertyInfo.GetGetMethod(true);
            return (T) getter.Invoke(instance, null);
        }

        /// <summary>
        /// Sets the value of a field of an instance.
        /// </summary>
        /// <param name="obj">Instance holding the field. Cannot be <c>null</c>.</param>
        /// <param name="fieldName">Name of the field.</param>
        /// <param name="newValue">The new value for the field.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="obj"/>
        /// doesn't have a field with the name <paramref name="fieldName"/>.</exception>
        /// <exception cref="ArgumentException">Thrown when <paramref name="newValue"/> is of incorrect type.</exception>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="fieldName"/> 
        /// or <paramref name="obj"/> is <c>null</c>.</exception>
        /// <remarks>This method can be used for fields of any visibility.</remarks>
        public static void SetField(object obj, string fieldName, object newValue)
        {
            if (obj == null)
            {
                throw new ArgumentNullException(nameof(obj));
            }

            FieldInfo fieldInfo = GetFieldInfo(obj.GetType(), fieldName);
            if (fieldInfo == null)
            {
                throw new ArgumentOutOfRangeException(nameof(fieldName));
            }

            fieldInfo.SetValue(obj, newValue);
        }

        /// <summary>
        /// Calls the private method that returns a value.
        /// </summary>
        /// <typeparam name="T">The return type of the method.</typeparam>
        /// <param name="instance">The instance declaring the method. Cannot be <c>null</c>.</param>
        /// <param name="methodName">Name of the method.</param>
        /// <param name="arguments">The arguments for the method.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when the method referred to by <paramref name="methodName"/>
        /// is not declared or inherited by the class of <paramref name="instance"/>.</exception>
        /// <exception cref="TargetInvocationException">Thrown when the invoked method or constructor throws an exception.</exception>
        /// <exception cref="TargetParameterCountException">Thrown when the <paramref name="arguments"/>
        /// array does not have the correct number of arguments.</exception>
        /// <exception cref="InvalidOperationException">The type that declares the method 
        /// is an open generic type. That is, the <see cref="Type.ContainsGenericParameters"/>
        /// property returns <c>true</c> for the declaring type.</exception>
        /// <exception cref="AmbiguousMatchException">Thrown when more than one method is found with 
        /// the specified name and matching the specified binding constraints.</exception>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="instance"/>
        /// or <paramref name="methodName"/> is <c>null</c>.</exception>
        /// <returns>The return value of the method.</returns>
        public static T CallPrivateMethod<T>(object instance, string methodName, params object[] arguments)
        {
            if (instance == null)
            {
                throw new ArgumentNullException(nameof(instance));
            }

            MethodInfo methodInfo = instance.GetType().GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Instance);
            if (methodInfo == null)
            {
                throw new ArgumentOutOfRangeException(nameof(methodName));
            }

            return (T) methodInfo.Invoke(instance, arguments);
        }

        /// <summary>
        /// Calls the private method that without returning its value.
        /// </summary>
        /// <param name="instance">The instance declaring the method. Cannot be <c>null</c>.</param>
        /// <param name="methodName">Name of the method.</param>
        /// <param name="arguments">The arguments for the method.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when the method referred to by <paramref name="methodName"/>
        /// is not declared or inherited by the class of <paramref name="instance"/>.</exception>
        /// <exception cref="TargetInvocationException">Thrown when the invoked method or constructor throws an exception.</exception>
        /// <exception cref="TargetParameterCountException">Thrown when the <paramref name="arguments"/>
        /// array does not have the correct number of arguments.</exception>
        /// <exception cref="InvalidOperationException">Thrown when the type that declares the method 
        /// is an open generic type. I.e., the <see cref="Type.ContainsGenericParameters"/>
        /// property returns <c>true</c> for the declaring type.</exception>
        /// <exception cref="AmbiguousMatchException">Thrown when more than one method is found with 
        /// the specified name and matching the specified binding constraints.</exception>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="instance"/>
        /// or <paramref name="methodName"/> is <c>null</c>.</exception>
        public static void CallPrivateMethod(object instance, string methodName, params object[] arguments)
        {
            if (instance == null)
            {
                throw new ArgumentNullException(nameof(instance));
            }

            MethodInfo methodInfo = instance.GetType().GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Instance);
            if (methodInfo == null)
            {
                throw new ArgumentOutOfRangeException(nameof(methodName));
            }

            methodInfo.Invoke(instance, arguments);
        }

        /// <summary>
        /// Sets the value of a property with a setter.
        /// </summary>
        /// <param name="instance">The instance declaring the property. Cannot be <c>null</c>.</param>
        /// <param name="propertyName">Name of the property to be set.</param>
        /// <param name="value">The new value of the property.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="instance"/>
        /// or <paramref name="propertyName"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when the property referred to by <paramref name="propertyName"/>
        /// is not declared or inherited by the class of <paramref name="instance"/>.</exception>
        /// <exception cref="ArgumentException">Thrown when the property has no setter.</exception>
        /// <exception cref="TargetParameterCountException">Thrown when the Property is an indexed property.</exception>
        /// <exception cref="TargetInvocationException">Thrown when an error occurred while setting the
        /// property value. For example, an index value specified for an indexed property
        /// is out of range. The <see cref="Exception.InnerException"/> property indicates
        /// the reason for the error.</exception>
        public static void SetPrivatePropertyValue(object instance, string propertyName, object value)
        {
            if (instance == null)
            {
                throw new ArgumentNullException(nameof(instance));
            }

            PropertyInfo propertyInfo = instance.GetType().GetProperty(propertyName);
            if (propertyInfo == null)
            {
                throw new ArgumentOutOfRangeException(nameof(propertyName));
            }

            propertyInfo.SetValue(instance, value, null);
        }

        /// <summary>
        /// Gets the attributes of type <typeparamref name="TAttribute"/> from a property on
        /// <typeparamref name="TObject"/>.
        /// </summary>
        /// <param name="propertyName">Name of the property to be get the attributes from.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="propertyName"/> is <c>null</c>.</exception>
        /// <exception cref="AmbiguousMatchException">Thrown when more than one property is found with the specified name.</exception>
        /// <exception cref="TypeLoadException">Thrown when a custom attribute type cannot be loaded.</exception>
        /// <exception cref="InvalidOperationException">Thrown when the property belongs to a type that
        /// is loaded into the reflection-only context.</exception>
        public static IEnumerable<TAttribute> GetPropertyAttributes<TObject, TAttribute>(string propertyName)
        {
            if (propertyName == null)
            {
                throw new ArgumentNullException(nameof(propertyName));
            }

            return typeof(TObject).GetProperty(propertyName)?.GetCustomAttributes(typeof(TAttribute), false).Select(attribute => (TAttribute) attribute);
        }

        /// <summary>
        /// Gets a field (of any visibility specification) declared on the type (as instance
        /// or static field).
        /// </summary>
        /// <param name="type">Declaring type of the field.</param>
        /// <param name="fieldName">Name of the field.</param>
        /// <returns>A <see cref="FieldInfo"/> object capturing the requested field, or <c>null</c>
        /// if the field cannot be found in <paramref name="type"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="fieldName"/> is <c>null</c>.</exception>
        private static FieldInfo GetFieldInfo(Type type, string fieldName)
        {
            if (type == typeof(object) || type == null)
            {
                return null;
            }
            FieldInfo fieldInfo = type.GetField(fieldName, BindingFlags.Instance
                                                           | BindingFlags.NonPublic
                                                           | BindingFlags.Public
                                                           | BindingFlags.Static);
            return fieldInfo ?? GetFieldInfo(type.BaseType, fieldName);
        }
    }
}