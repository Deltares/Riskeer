using System;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using DelftTools.Controls.Swf.DataEditorGenerator.Metadata;

namespace DelftTools.Controls.Swf.DataEditorGenerator.FromType
{
    public static class ObjectDescriptionFromTypeExtractor
    {
        public static ObjectUIDescription ExtractObjectDescription(Type type)
        {
            return new ObjectUIDescription
            {
                FieldDescriptions =
                    type.GetProperties(
                        BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                        .Where(p => !p.GetCustomAttributes(typeof(HideAttribute), false).Any())
                        .OrderBy(p => p.Name) //alphabetic ordering
                        .Select(ExtractFieldDescription)
                        .ToList(),
            };
        }

        private static FieldUIDescription ExtractFieldDescription(PropertyInfo propertyInfo)
        {
            Func<object, object> getter = o => propertyInfo.GetValue(o, null);
            Action<object, object> setter = (o, v) => propertyInfo.SetValue(o, v, null);
            Func<object, bool> isEnabled = GetIsEnabledFunc(propertyInfo);
            Func<object, bool> isVisible = GetIsVisibleFunc(propertyInfo);

            var descr = new FieldUIDescription(getter, setter, isEnabled, isVisible)
            {
                Name = propertyInfo.Name,
                Category = GetCategoryValue<CategoryAttribute>(propertyInfo, a => a.Category),
                SubCategory = GetCategoryValue<SubCategoryAttribute>(propertyInfo, a => a.SubCategory),
                UnitSymbol = GetCategoryValue<UnitAttribute>(propertyInfo, a => a.Symbol),
                Label = GetCategoryValue<DisplayNameAttribute>(propertyInfo, a => a.DisplayName) ??
                        GetCategoryValue<DescriptionAttribute>(propertyInfo, a => a.Description) ??
                        propertyInfo.Name,
                IsReadOnly = propertyInfo.GetSetMethod() == null || GetAttribute<ReadOnlyAttribute>(propertyInfo) != null,
                AlwaysRefresh = GetAttribute<DependentPropertyAttribute>(propertyInfo) != null,
                ValueType = propertyInfo.PropertyType,
                CustomControlHelper = InstantiateCustomControlHelper(GetCategoryValue<CustomControlHelperAttribute>(propertyInfo,
                                                                                                                    a => a.HelperTypeName), GetCategoryValue<CustomControlHelperAttribute>(propertyInfo,
                                                                                                                                                                                           a => a.AssemblyName)),
            };
            descr.ToolTip = descr.Label;
            return descr;
        }

        private static Func<object, bool> GetIsEnabledFunc(PropertyInfo propertyInfo)
        {
            Func<object, bool> isEnabled = null;
            var enabledIfAttribute = GetAttribute<EnabledIfAttribute>(propertyInfo);
            if (enabledIfAttribute != null)
            {
                var type = propertyInfo.DeclaringType;
                var enabledIfProperty = type.GetProperty(enabledIfAttribute.PropertyName);
                isEnabled = GetOperationIfFunc(enabledIfAttribute, enabledIfProperty);
            }
            return isEnabled;
        }

        private static Func<object, bool> GetIsVisibleFunc(PropertyInfo propertyInfo)
        {
            Func<object, bool> isVisible = null;
            var visibleIfAttribute = GetAttribute<VisibleIfAttribute>(propertyInfo);
            if (visibleIfAttribute != null)
            {
                var type = propertyInfo.DeclaringType;
                var enabledIfProperty = type.GetProperty(visibleIfAttribute.PropertyName);
                isVisible = GetOperationIfFunc(visibleIfAttribute, enabledIfProperty);
            }
            return isVisible;
        }

        private static Func<object, bool> GetOperationIfFunc(OperationIfAttribute attribute, PropertyInfo propertyToSet)
        {
            return delegate(object d)
            {
                var dataSourceValue = propertyToSet.GetValue(d, null);
                var expectedValue = attribute.Value;

                switch (attribute.Operation)
                {
                    case OperationIfAttribute.IfOperation.Equal:
                        return Equals(dataSourceValue, expectedValue);
                    case OperationIfAttribute.IfOperation.NotEqual:
                        return !Equals(dataSourceValue, expectedValue);
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            };
        }

        private static string GetCategoryValue<TAttrib>(PropertyInfo propertyInfo, Func<TAttrib, string> getter)
            where TAttrib : class
        {
            return GetCategoryValue<TAttrib, string>(propertyInfo, getter);
        }

        private static TValue GetCategoryValue<TAttrib, TValue>(PropertyInfo propertyInfo, Func<TAttrib, TValue> getter)
            where TAttrib : class
        {
            var attribute = GetAttribute<TAttrib>(propertyInfo);
            return attribute != null ? getter(attribute) : default(TValue);
        }

        private static TAttrib GetAttribute<TAttrib>(PropertyInfo propertyInfo) where TAttrib : class
        {
            var customAttributes = propertyInfo.GetCustomAttributes(true);
            return (TAttrib) customAttributes.FirstOrDefault(c => c is TAttrib);
        }

        private static ICustomControlHelper InstantiateCustomControlHelper(string customControlHelperTypeName, string assemblyName)
        {
            if (string.IsNullOrEmpty(customControlHelperTypeName))
            {
                return null;
            }

            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            if (!string.IsNullOrEmpty(assemblyName))
            {
                assemblies = assemblies.Where(a => a.FullName.StartsWith(assemblyName)).ToArray();
            }

            if (assemblies.Length == 0)
            {
                throw new ArgumentException(string.Format(
                    "Cannot find assembly {0} for type {1}. Make sure the assembly is loaded.", assemblyName,
                    customControlHelperTypeName));
            }

            var type = assemblies.SelectMany(a => a.GetTypes())
                                 .FirstOrDefault(t => t.FullName.Equals(customControlHelperTypeName));

            if (type == null)
            {
                throw new ArgumentException(string.Format(
                    "Cannot find type {0}, make sure you specify the full type name, including namespace. Also give the assembly name.",
                    customControlHelperTypeName));
            }

            var instance = Activator.CreateInstance(type);
            var helper = instance as ICustomControlHelper;
            if (helper == null)
            {
                throw new ArgumentException(string.Format(
                    "The supplied type must implement ICustomControlHelper: {0}", customControlHelperTypeName));
            }

            return helper;
        }
    }
}