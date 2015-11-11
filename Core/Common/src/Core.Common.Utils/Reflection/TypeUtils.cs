using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Core.Common.Utils.Aop;
using log4net;

namespace Core.Common.Utils.Reflection
{
    public static class TypeUtils
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(TypeUtils));

        private static readonly IDictionary<Type, IDictionary<string, PropertyInfo>> PropertyInfoDictionary = new Dictionary<Type, IDictionary<string, PropertyInfo>>();

        private static readonly IDictionary<string, MethodInfo> CachedMethods = new Dictionary<string, MethodInfo>();

        public static bool Implements<T>(this Type thisType)
        {
            return typeof(T).IsAssignableFrom(thisType);
        }

        public static bool Implements(this Type thisType, Type type)
        {
            return type.IsAssignableFrom(thisType);
        }

        public static bool IsNumericalType(this Type type)
        {
            return (type == typeof(Single) ||
                    type == typeof(UInt32) ||
                    type == typeof(UInt16) ||
                    type == typeof(Int64) ||
                    type == typeof(Int32) ||
                    type == typeof(Int16) ||
                    type == typeof(Double));
        }

        /// <summary>
        /// Usage: CreateGeneric(typeof(List&lt;&gt;), typeof(string));
        /// </summary>
        /// <param name="generic"></param>
        /// <param name="innerType"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static object CreateGeneric(Type generic, Type innerType, params object[] args)
        {
            Type specificType = generic.MakeGenericType(new[]
            {
                innerType
            });
            return Activator.CreateInstance(specificType, args);
        }

        public static object CreateGeneric(Type generic, Type[] innerTypes, params object[] args)
        {
            Type specificType = generic.MakeGenericType(innerTypes);
            return Activator.CreateInstance(specificType, args);
        }

        public static object GetPropertyValue(object instance, string propertyName, bool throwOnError = true)
        {
            var implementingType = instance.GetType();

            var propertyInfo = GetPropertyInfo(implementingType, propertyName);

            if (!throwOnError && propertyInfo.GetIndexParameters().Any())
            {
                return null; //invalid combo, would throw
            }

            return propertyInfo.GetValue(instance, new object[0]);
        }

        public static PropertyInfo GetPropertyInfo(Type type, string propertyName)
        {
            IDictionary<string, PropertyInfo> propertyInfoForType;
            PropertyInfo propertyInfo;

            lock (PropertyInfoDictionary)
            {
                if (!PropertyInfoDictionary.TryGetValue(type, out propertyInfoForType))
                {
                    propertyInfoForType = new Dictionary<string, PropertyInfo>();
                    PropertyInfoDictionary.Add(type, propertyInfoForType);
                }
            }

            lock (propertyInfoForType)
            {
                if (!propertyInfoForType.TryGetValue(propertyName, out propertyInfo))
                {
                    propertyInfo = GetPrivatePropertyInfo(type, propertyName);
                    propertyInfoForType.Add(propertyName, propertyInfo);
                }
            }
            return propertyInfo;
        }

        public static object CallGenericMethod(Type declaringType, string methodName, Type genericType,
                                               object instance, params object[] args)
        {
            var key = declaringType + "_" + genericType + "_" + methodName;

            MethodInfo methodInfo;

            CachedMethods.TryGetValue(key, out methodInfo);

            if (methodInfo != null) // performance optimization, reflaction is very expensive
            {
                return CallMethod(methodInfo, instance, args);
            }

            MethodInfo nonGeneric = GetGenericMethod(declaringType, methodName);

            methodInfo = nonGeneric.MakeGenericMethod(genericType); // generify

            CachedMethods[key] = methodInfo;

            return CallMethod(methodInfo, instance, args);
        }

        /// <summary>
        /// (Shallow) copies all public members of a class: Value types are 'cloned', references to other objects are left as-is. Works 
        /// well with NHibernate and PostSharp.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="inst"></param>
        /// <returns></returns>
        public static T MemberwiseClone<T>(T inst)
        {
            inst = Unproxy(inst);

            var clonedInst = CreateInstance<T>(inst.GetType());

            foreach (var fieldAndValue in GetNonInfrastructureFields(inst, inst.GetType()))
            {
                var fieldInfo = fieldAndValue.Key;
                var value = fieldAndValue.Value;
                fieldInfo.SetValue(clonedInst, value);
            }

            return clonedInst;
        }

        public static T Unproxy<T>(T inst)
        {
            var isProxy = inst.GetType().GetInterfaces().Any(i => i.Name == "INHibernateProxy");

            if (!isProxy)
            {
                return inst;
            }

            var type = AppDomain.CurrentDomain.GetAssemblies()
                                .Where(a => a.FullName.Contains("NHibernate"))
                                .SelectMany(a => a.GetTypes())
                                .First(t => t.Name == "DeltaShellProxyInterceptor");

            //static deproxy method:
            var deproxyMethod = type.GetMethod("GetRealObject", BindingFlags.NonPublic | BindingFlags.Static);

            return (T) deproxyMethod.Invoke(null, new object[]
            {
                inst
            });
        }

        public static object CallStaticGenericMethod(Type type, string methodName, Type genericType,
                                                     params object[] args)
        {
            MethodInfo nonGeneric = type.GetMethod(methodName, BindingFlags.Static | BindingFlags.Public);
            MethodInfo methodGeneric = nonGeneric.MakeGenericMethod(genericType);
            return methodGeneric.Invoke(null, args);
        }

        public static IList GetTypedList(Type t)
        {
            return (IList) CreateGeneric(typeof(List<>), t);
        }

        public static IEnumerable ConvertEnumerableToType(IEnumerable enumerable, Type type)
        {
            return (IEnumerable) CallStaticGenericMethod(typeof(Enumerable), "Cast", type, enumerable);
        }

        /// <summary>
        /// Returns typeof(int) for List&lt;int&gt; etc.
        /// </summary>
        /// <param name="t"></param>
        public static Type GetFirstGenericTypeParameter(Type t)
        {
            Type[] types = t.GetGenericArguments();
            if (types.Length > 0)
            {
                return types[0];
            }
            else
            {
                return null;
            }
        }

        public static IEnumerable<FieldInfo> GetAllFields(Type t, BindingFlags bindingFlags)
        {
            if (t == null)
            {
                return Enumerable.Empty<FieldInfo>();
            }

            BindingFlags flags = bindingFlags;
            return t.GetFields(flags).Union(GetAllFields(t.BaseType, bindingFlags));
        }

        public static string GetMemberDescription<T>(Expression<Func<T>> e)
        {
            var member = e.Body as MemberExpression;

            // If the method gets a lambda expression 
            // that is not a member access,
            // for example, () => x + y, an exception is thrown.
            if (member != null)
            {
                var descriptionAttribute = member.Member.GetCustomAttributes(false).OfType<DescriptionAttribute>().FirstOrDefault();
                return descriptionAttribute != null ? descriptionAttribute.Description : member.Member.Name;
            }

            throw new ArgumentException("'" + e + "': is not a valid expression for this method");
        }

        /// <summary>
        /// Gets the attribute that is declared on a field.
        /// </summary>
        /// <example>
        /// class A
        /// {
        ///     [Description("int property")]
        ///     int field
        /// }
        /// </example>
        /// <typeparam name="TAttribute">Attribute type to get</typeparam>
        /// <param name="classType">Class type containing the field</param>
        /// <param name="fieldName">Name of the field</param>
        public static TAttribute GetFieldAttribute<TAttribute>(Type classType, string fieldName) where TAttribute : class
        {
            var fieldInfo = GetFieldInfo(classType, fieldName);
            return ((TAttribute[]) fieldInfo.GetCustomAttributes(typeof(TAttribute), false)).FirstOrDefault();
        }

        public static string GetMemberName<TClass>(Expression<Func<TClass, object>> e)
        {
            var member = e.Body as MemberExpression;

            if (member != null)
            {
                return GetMemberNameFromMemberExpression(member);
            }

            var unary = e.Body as UnaryExpression;

            // If the method gets a lambda expression 
            // that is not a member access,
            // for example, () => x + y, an exception is thrown.
            if (unary != null)
            {
                return GetMemberNameFromMemberExpression(unary.Operand as MemberExpression);
            }

            throw new ArgumentException(
                "'" + e +
                "': is not a valid expression for this method");
        }

        public static string GetMemberName<T>(Expression<Func<T>> e)
        {
            return GetMemberNameFromMemberExpression(e.Body as MemberExpression);
        }

        public static object GetDefaultValue(Type type)
        {
            return type.IsValueType
                       ? Activator.CreateInstance(type)
                       : type == typeof(string) ? "" : null;
        }

        public static object GetField(object instance, string fieldName)
        {
            Type type = instance.GetType();

            FieldInfo fieldInfo = GetFieldInfo(type, fieldName);

            if (fieldInfo == null)
            {
                throw new ArgumentOutOfRangeException("fieldName");
            }

            return fieldInfo.GetValue(instance);
        }

        public static bool HasField(Type type, string fieldName)
        {
            return GetFieldInfoCore(type, fieldName) != null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TObject">Type of the object where field is stored.</typeparam>
        /// <typeparam name="TField">Type of the field, used as return type</typeparam>
        /// <param name="instance"></param>
        /// <param name="fieldName"></param>
        /// <returns></returns>
        public static TField GetField<TObject, TField>(object instance, string fieldName)
        {
            var fieldInfo = typeof(TObject).GetField(fieldName, BindingFlags.Instance
                                                                | BindingFlags.NonPublic
                                                                | BindingFlags.Public);

            if (fieldInfo == null)
            {
                throw new ArgumentOutOfRangeException("fieldName");
            }

            return (TField) fieldInfo.GetValue(instance);
        }

        /// <summary>
        /// Returns the value of a private static field
        /// </summary>
        /// <typeparam name="TField">Type of the field, used as return type</typeparam>
        /// <param name="type">The type of the class that holds the private static field</param>
        /// <param name="staticFieldName">The name of the private static field</param>
        public static TField GetStaticField<TField>(Type type, string staticFieldName)
        {
            var fieldInfo = type.GetField(staticFieldName, BindingFlags.NonPublic | BindingFlags.Static);

            if (fieldInfo == null)
            {
                throw new ArgumentOutOfRangeException("staticFieldName");
            }

            return (TField) fieldInfo.GetValue(null);
        }

        public static void SetField(object obj, string fieldName, object value)
        {
            var fieldInfo = GetFieldInfo(obj.GetType(), fieldName);

            if (fieldInfo == null)
            {
                throw new ArgumentOutOfRangeException("fieldName");
            }

            fieldInfo.SetValue(obj, value);
        }

        public static void SetField<T>(object obj, string fieldName, object value)
        {
            var fieldInfo = typeof(T).GetField(fieldName, BindingFlags.Instance
                                                          | BindingFlags.NonPublic
                                                          | BindingFlags.Public | BindingFlags.FlattenHierarchy);

            if (fieldInfo == null)
            {
                throw new ArgumentOutOfRangeException("fieldName");
            }

            fieldInfo.SetValue(obj, value);
        }

        public static T CallPrivateMethod<T>(object instance, string methodName, params object[] arguments)
        {
            var type = instance.GetType();
            var methodInfo = type.GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Instance);

            return (T) methodInfo.Invoke(instance, arguments);
        }

        public static void CallPrivateMethod(object instance, string methodName, params object[] arguments)
        {
            var type = instance.GetType();
            var methodInfo = type.GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Instance);

            methodInfo.Invoke(instance, arguments);
        }

        public static IEnumerable<PropertyInfo> GetPublicProperties(Type type)
        {
            return type.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.FlattenHierarchy);
        }

        public static object CallPrivateStaticMethod(Type type, string methodName, params object[] arguments)
        {
            var methodInfo = type.GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Static);

            return methodInfo.Invoke(null, arguments);
        }

        public static void SetPropertyValue(object instance, string propertyName, object value)
        {
            instance.GetType().GetProperty(propertyName).GetSetMethod().Invoke(instance, new[]
            {
                value
            });
        }

        public static void SetPrivatePropertyValue(object instance, string propertyName, object value)
        {
            instance.GetType().GetProperty(propertyName).SetValue(instance, value, null);
        }

        public static bool TrySetValueAnyVisibility(object instance, Type type, string propertyName, object value)
        {
            if (type == null)
            {
                return false;
            }

            var propertyInfo = type.GetProperties().First(p => p.Name == propertyName);

            if (propertyInfo == null)
            {
                return false;
            }

            if (!propertyInfo.CanWrite)
            {
                //try base type
                return TrySetValueAnyVisibility(instance, type.BaseType, propertyName, value);
            }
            propertyInfo.SetValue(instance, value, null);
            return true;
        }

        /// <summary>
        /// Test if the assembly is dynamic using a HACK...rewrite if we have more knowledge
        /// </summary>
        /// <param name="assembly"></param>
        /// <returns></returns>
        public static bool IsDynamic(this Assembly assembly)
        {
            //see http://stackoverflow.com/questions/1423733/how-to-tell-if-a-net-assembly-is-dynamic
            //more nice than depending on an exception..
            return (assembly.ManifestModule.GetType().Namespace == "System.Reflection.Emit");
        }

        public static void ClearCaches()
        {
            PropertyInfoDictionary.Clear();
            CachedMethods.Clear();
        }

        internal static IEnumerable<KeyValuePair<FieldInfo, object>> GetNonInfrastructureFields(object inst, Type type)
        {
            foreach (var fi in GetAllAccessibleFieldsForType(type))
            {
                //skip events
                var value = fi.GetValue(inst);
                if (value is Delegate)
                {
                    continue; //don't copy events
                }

                //skip 'Id'
                if (fi.Name == "id" || fi.Name == "_id" || fi.Name == "<Id>k__BackingField")
                {
                    continue;
                }

                //skip PostSharp stuff?
                if (fi.FieldType == typeof(EntityAttribute))
                {
                    continue;
                }

                yield return new KeyValuePair<FieldInfo, object>(fi, value);
            }
        }

        internal static T CreateInstance<T>(Type type)
        {
            if (type.IsArray)
            {
                return (T) (object) Array.CreateInstance(type.GetElementType(), 0);
            }

            var defaultConstructor = type.GetConstructors(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance)
                                         .FirstOrDefault(c => c.GetParameters().Length == 0);

            if (defaultConstructor == null)
            {
                throw new NotImplementedException(string.Format("No default constructor available for type {0}", type));
            }

            return (T) Activator.CreateInstance(type, true);
        }

        private static PropertyInfo GetPrivatePropertyInfo(Type type, string propertyName)
        {
            //get the property by walking up the inheritance chain. See NHibernate's BasicPropertyAccessor
            //we could extend this logic more by looking there...
            if (type == typeof(object) || type == null)
            {
                // the full inheritance chain has been walked and we could
                // not find the PropertyInfo get
                return null;
            }

            var propertyInfo = type.GetProperty(propertyName,
                                                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly);
            if (propertyInfo != null)
            {
                return propertyInfo;
            }

            return GetPrivatePropertyInfo(type.BaseType, propertyName);
        }

        /// <summary>
        /// Returns generic instance method of given name. Cannot use GetMethod() because this does not
        /// work if 2 members have the same name (eg. SetValues and SetValues&lt;T&gt;)
        /// </summary>
        /// <param name="declaringType"></param>
        /// <param name="methodName"></param>
        /// <returns></returns>
        private static MethodInfo GetGenericMethod(Type declaringType, string methodName)
            //,Type genericType,object instance,params object[]args )
        {
            var methods = declaringType.GetMembers(BindingFlags.InvokeMethod | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            return methods.OfType<MethodInfo>().First(m => m.Name == methodName && m.IsGenericMethod);
        }

        private static object CallMethod(MethodInfo methodInfo, object instance, object[] args)
        {
            object result;
            try
            {
                result = methodInfo.Invoke(instance, args);
            }
            catch (TargetInvocationException e)
            {
                // re-throw original exception
                if (e.InnerException != null)
                {
                    log.Error("Exception occured", e); // log outer exception

                    throw e.InnerException;
                }

                throw;
            }

            return result;
        }

        private static IEnumerable<FieldInfo> GetAllAccessibleFieldsForType(Type type)
        {
            return type == null
                       ? new FieldInfo[0]
                       : type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                             .Where(fi => !fi.IsLiteral && !fi.IsInitOnly)
                             .Concat(GetAllAccessibleFieldsForType(type.BaseType)).Distinct();
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
            throw new ArgumentException("'member' not a valid expression for this method");
        }

        private static FieldInfo GetFieldInfo(Type type, string fieldName)
        {
            var fieldInfo = GetFieldInfoCore(type, fieldName) ??
                            GetFieldInfoCore(type, string.Format("<{0}>", fieldName)); //postsharp compatibility mode..*sigh*
            return fieldInfo;
        }

        private static FieldInfo GetFieldInfoCore(Type type, string fieldName)
        {
            if (type == typeof(object) || type == null)
            {
                return null;
            }
            FieldInfo fieldInfo = type.GetField(fieldName, BindingFlags.Instance
                                                           | BindingFlags.NonPublic
                                                           | BindingFlags.Public
                                                           | BindingFlags.Static);
            if (fieldInfo != null)
            {
                return fieldInfo;
            }
            return GetFieldInfoCore(type.BaseType, fieldName);
        }
    }
}