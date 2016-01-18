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
        /// Determines whether the given type can be considered a number or not.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        public static bool IsNumericalType(this Type type)
        {
            return (type == typeof(Single) ||
                    type == typeof(UInt32) ||
                    type == typeof(UInt16) ||
                    type == typeof(Int64) ||
                    type == typeof(Int32) ||
                    type == typeof(Int16) ||
                    type == typeof(byte) ||
                    type == typeof(Double) ||
                    type == typeof(Decimal));
        }

        /// <summary>
        /// Gets the name of the member.
        /// </summary>
        /// <typeparam name="TClass">The type of the class on which the expression takes place.</typeparam>
        /// <param name="expression">The expression.</param>
        /// <returns>The string name of the member.</returns>
        /// <exception cref="System.ArgumentException">When <paramref name="expression"/> is invalid.</exception>
        public static string GetMemberName<TClass>(Expression<Func<TClass, object>> expression)
        {
            var member = expression.Body as MemberExpression;

            if (member != null)
            {
                return GetMemberNameFromMemberExpression(member);
            }

            var unary = expression.Body as UnaryExpression;

            // If the method gets a lambda expression 
            // that is not a member access,
            // for example, () => x + y, an exception is thrown.
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
        /// <typeparam name="TField">Type of the field.</typeparam>
        /// <param name="instance">Instance holding the field. Cannot be null.</param>
        /// <param name="fieldName">Name of the field.</param>
        /// <returns>The value of the field.</returns>
        /// <exception cref="ArgumentOutOfRangeException">When <paramref name="instance"/>
        /// doesn't have a field with the name <paramref name="fieldName"/>.</exception>
        /// <exception cref="ArgumentNullException">When <paramref name="fieldName"/> is null.</exception>
        /// <remarks>This method can be used for fields of any visibility.</remarks>
        public static TField GetField<TField>(object instance, string fieldName)
        {
            FieldInfo fieldInfo = GetFieldInfo(instance.GetType(), fieldName);
            if (fieldInfo == null)
            {
                throw new ArgumentOutOfRangeException("fieldName");
            }

            return (TField)fieldInfo.GetValue(instance);
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
            // If the method gets a lambda expression 
            // that is not a member access,
            // for example, () => x + y, an exception is thrown.
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