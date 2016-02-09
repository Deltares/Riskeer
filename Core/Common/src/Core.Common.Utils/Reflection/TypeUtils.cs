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
using System.Linq.Expressions;
using System.Reflection;
using Core.Common.Utils.Properties;

namespace Core.Common.Utils.Reflection
{
    /// <summary>
    /// Helper methods dealing with <see cref="Type"/> and reflection.
    /// </summary>
    public static class TypeUtils
    {
        /// <summary>
        /// Checks if a type implements, inherits from or is a certain other type.
        /// </summary>
        /// <typeparam name="T">The type to check for.</typeparam>
        /// <param name="thisType">Type to check.</param>
        /// <returns>True if <paramref name="thisType"/> is the same type as <typeparamref name="T"/>,
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
        /// <returns>True if <paramref name="thisType"/> is the same type as <paramref name="type"/>,
        /// or has that as (one of) its supertypes.</returns>
        /// <seealso cref="Implements{T}"/>
        public static bool Implements(this Type thisType, Type type)
        {
            return type.IsAssignableFrom(thisType);
        }

        /// <summary>
        /// Gets the name of the member.
        /// </summary>
        /// <typeparam name="T">The type of the class on which the expression takes place.</typeparam>
        /// <param name="expression">The expression.</param>
        /// <returns>The string name of the member.</returns>
        /// <exception cref="System.ArgumentException">When <paramref name="expression"/> 
        /// is not an expression with a member, such as a expression calling multiple methods.</exception>
        public static string GetMemberName<T>(Expression<Func<T, object>> expression)
        {
            var member = expression.Body as MemberExpression;

            if (member != null)
            {
                return GetMemberNameFromMemberExpression(member);
            }

            var unary = expression.Body as UnaryExpression;

            if (unary != null)
            {
                return GetMemberNameFromMemberExpression(unary.Operand as MemberExpression);
            }

            var message = string.Format(Resources.TypeUtils_GetMemberName_0_is_not_a_valid_expression_for_this_method,
                                        expression);
            throw new ArgumentException(message);
        }

        /// <summary>
        /// Gets the value of a field of an instance.
        /// </summary>
        /// <typeparam name="T">Type of the field.</typeparam>
        /// <param name="instance">Instance holding the field. Cannot be null.</param>
        /// <param name="fieldName">Name of the field.</param>
        /// <returns>The value of the field.</returns>
        /// <exception cref="ArgumentOutOfRangeException">When <paramref name="instance"/>
        /// doesn't have a field with the name <paramref name="fieldName"/>.</exception>
        /// <exception cref="ArgumentNullException">When <paramref name="fieldName"/> is null.</exception>
        /// <remarks>This method can be used for fields of any visibility.</remarks>
        public static T GetField<T>(object instance, string fieldName)
        {
            FieldInfo fieldInfo = GetFieldInfo(instance.GetType(), fieldName);
            if (fieldInfo == null)
            {
                throw new ArgumentOutOfRangeException("fieldName");
            }

            return (T)fieldInfo.GetValue(instance);
        }

        /// <summary>
        /// Gets the value of a field of an instance.
        /// </summary>
        /// <param name="obj">Instance holding the field. Cannot be null.</param>
        /// <param name="fieldName">Name of the field.</param>
        /// <param name="newValue">The new value for the field.</param>
        /// <exception cref="ArgumentOutOfRangeException">When <paramref name="obj"/>
        /// doesn't have a field with the name <paramref name="fieldName"/>.</exception>
        /// <exception cref="ArgumentException"><paramref name="newValue"/> is of incorrect type.</exception>
        /// <exception cref="ArgumentNullException">When <paramref name="fieldName"/> is null.</exception>
        /// <remarks>This method can be used for fields of any visibility.</remarks>
        public static void SetField(object obj, string fieldName, object newValue)
        {
            FieldInfo fieldInfo = GetFieldInfo(obj.GetType(), fieldName);
            if (fieldInfo == null)
            {
                throw new ArgumentOutOfRangeException("fieldName");
            }

            fieldInfo.SetValue(obj, newValue);
        }

        /// <summary>
        /// Calls the private method that returns a value.
        /// </summary>
        /// <typeparam name="T">The return type of the method.</typeparam>
        /// <param name="instance">The instance declaring the method. Cannot be null.</param>
        /// <param name="methodName">Name of the method.</param>
        /// <param name="arguments">The arguments for the method.</param>
        /// <exception cref="ArgumentOutOfRangeException">The method referred to by <paramref name="methodName"/>
        /// is not declared or inherited by the class of <paramref name="instance"/>.</exception>
        /// <exception cref="TargetInvocationException">The invoked method or constructor throws an exception.</exception>
        /// <exception cref="TargetParameterCountException">The <paramref name="arguments"/>
        /// array does not have the correct number of arguments.</exception>
        /// <exception cref="InvalidOperationException">The type that declares the method 
        /// is an open generic type. That is, the <see cref="Type.ContainsGenericParameters"/>
        /// property returns true for the declaring type.</exception>
        /// <exception cref="AmbiguousMatchException">More than one method is found with 
        /// the specified name and matching the specified binding constraints.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="methodName"/> is null.</exception>
        /// <returns>The return value of the method.</returns>
        public static T CallPrivateMethod<T>(object instance, string methodName, params object[] arguments)
        {
            var methodInfo = instance.GetType().GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Instance);
            if (methodInfo == null)
            {
                throw new ArgumentOutOfRangeException("methodName");
            }

            return (T) methodInfo.Invoke(instance, arguments);
        }

        /// <summary>
        /// Calls the private method that without returning its value.
        /// </summary>
        /// <param name="instance">The instance declaring the method. Cannot be null.</param>
        /// <param name="methodName">Name of the method.</param>
        /// <param name="arguments">The arguments for the method.</param>
        /// <exception cref="ArgumentOutOfRangeException">The method referred to by <paramref name="methodName"/>
        /// is not declared or inherited by the class of <paramref name="instance"/>.</exception>
        /// <exception cref="TargetInvocationException">The invoked method or constructor throws an exception.</exception>
        /// <exception cref="TargetParameterCountException">The <paramref name="arguments"/>
        /// array does not have the correct number of arguments.</exception>
        /// <exception cref="InvalidOperationException">The type that declares the method 
        /// is an open generic type. That is, the <see cref="Type.ContainsGenericParameters"/>
        /// property returns true for the declaring type.</exception>
        /// <exception cref="AmbiguousMatchException">More than one method is found with 
        /// the specified name and matching the specified binding constraints.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="methodName"/> is null.</exception>
        public static void CallPrivateMethod(object instance, string methodName, params object[] arguments)
        {
            var methodInfo = instance.GetType().GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Instance);
            if (methodInfo == null)
            {
                throw new ArgumentOutOfRangeException("methodName");
            }

            methodInfo.Invoke(instance, arguments);
        }

        /// <summary>
        /// Sets the value of a property with a setter.
        /// </summary>
        /// <param name="instance">The instance declaring the property. Cannot be null.</param>
        /// <param name="propertyName">Name of the property to be set.</param>
        /// <param name="value">The new value of the property.</param>
        /// <exception cref="ArgumentNullException"><paramref name="propertyName"/> is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">The property referred to by <paramref name="propertyName"/>
        /// is not declared or inherited by the class of <paramref name="instance"/>.</exception>
        /// <exception cref="ArgumentException">The property has not setter.</exception>
        /// <exception cref="TargetParameterCountException">Property is an indexed property.</exception>
        /// <exception cref="TargetInvocationException">An error occurred while setting the
        /// property value. For example, an index value specified for an indexed property
        /// is out of range. The <see cref="Exception.InnerException"/> property indicates
        /// the reason for the error.</exception>
        public static void SetPrivatePropertyValue(object instance, string propertyName, object value)
        {
            var propertyInfo = instance.GetType().GetProperty(propertyName);
            if (propertyInfo == null)
            {
                throw new ArgumentOutOfRangeException("propertyName");
            }

            propertyInfo.SetValue(instance, value, null);
        }

        private static string GetMemberNameFromMemberExpression(MemberExpression member)
        {
            if (member != null)
            {
                return member.Member.Name;
            }
            throw new ArgumentException(Resources.TypeUtils_GetMemberNameFromMemberExpression_member_not_a_valid_expression_for_this_method);
        }

        /// <summary>
        /// Gets a field (of any visibility specification) declared on the type (as instance
        /// or static field).
        /// </summary>
        /// <param name="type">Declaring type of the field.</param>
        /// <param name="fieldName">Name of the field.</param>
        /// <returns>A <see cref="FieldInfo"/> object capturing the requested field, or null
        /// if the field cannot be found in <paramref name="type"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="fieldName"/> is null.</exception>
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