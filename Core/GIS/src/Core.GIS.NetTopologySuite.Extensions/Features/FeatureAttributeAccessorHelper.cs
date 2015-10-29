using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Reflection;
using Core.Common.Utils;
using Core.Common.Utils.Reflection;
using Core.Gis.GeoApi.Extensions.Feature;

namespace Core.GIS.NetTopologySuite.Extensions.Features
{
    public static class FeatureAttributeAccessorHelper
    {
        private static readonly IDictionary<Type, IDictionary<string, MethodInfo>> getterCache =
            new Dictionary<Type, IDictionary<string, MethodInfo>>();

        public static IEnumerable<string> GetFeatureAttributeNames(Type featureType)
        {
            return GetFeatureAttributes(featureType).OrderBy(kvp => kvp.Value.Order).Select(kvp => kvp.Key.Name);
        }

        public static IEnumerable<string> GetFeatureAttributeNames(IFeature feature)
        {
            var attributeNames = new List<string>();

            if (feature is DataRow)
            {
                attributeNames.AddRange(from DataColumn column in ((DataRow) feature).Table.Columns
                                        select column.ColumnName);
                return attributeNames;
            }

            if (feature.Attributes != null)
            {
                attributeNames.AddRange(feature.Attributes.Keys);
            }

            attributeNames.AddRange(GetFeatureAttributeNames(feature.GetType()));

            return attributeNames;
        }

        public static IEnumerable<PropertyInfo> GetFeatureProperties(Type featureType)
        {
            return GetFeatureAttributes(featureType).OrderBy(kvp => kvp.Value.Order).Select(kvp => kvp.Key);
        }

        public static T GetAttributeValue<T>(IFeature feature, string name)
        {
            return GetAttributeValue<T>(feature, name, null);
        }

        public static T GetAttributeValue<T>(IFeature feature, string name, object noDataValue, bool throwOnNotFound = true)
        {
            var value = GetAttributeValue(feature, name, throwOnNotFound);

            if (value == null || value == DBNull.Value)
            {
                if (noDataValue == null)
                {
                    return (T) Activator.CreateInstance(typeof(T));
                }

                return (T) Convert.ChangeType(noDataValue, typeof(T));
            }

            if (typeof(T) == typeof(string))
            {
                if (value.GetType().IsEnum)
                {
                    return (T) (object) value.ToString();
                }
                return (T) (object) string.Format("{0:g}", value);
            }
            if (typeof(T) == typeof(double))
            {
                return (T) (object) Convert.ToDouble(value, CultureInfo.InvariantCulture);
            }
            if (typeof(T) == typeof(int))
            {
                return (T) (object) Convert.ToInt32(value, CultureInfo.InvariantCulture);
            }
            if (typeof(T) == typeof(short))
            {
                return (T) (object) Convert.ToInt16(value, CultureInfo.InvariantCulture);
            }
            if (typeof(T) == typeof(float))
            {
                return (T) (object) Convert.ToSingle(value, CultureInfo.InvariantCulture);
            }
            if (typeof(T) == typeof(byte))
            {
                return (T) (object) Convert.ToByte(value, CultureInfo.InvariantCulture);
            }
            if (typeof(T) == typeof(long))
            {
                return (T) (object) Convert.ToInt64(value, CultureInfo.InvariantCulture);
            }

            return (T) value;
        }

        public static object GetAttributeValue(IFeature feature, string name, bool throwOnNotFound = true)
        {
            if (feature.Attributes != null)
            {
                object value;
                if (feature.Attributes.TryGetValue(name, out value))
                {
                    return value;
                }
            }

            if (feature is DataRow)
            {
                return ((DataRow) feature)[name];
            }

            var getter = GetCachedAttributeValueGetter(feature, name, throwOnNotFound);

            if (getter != null)
            {
                return getter.Invoke(feature, null);
            }

            if (throwOnNotFound)
            {
                ThrowOnNotFound(feature.GetType(), name);
            }

            return null;
        }

        public static void SetAttributeValue(IFeature feature, string name, object value, bool throwIfNotFound = true)
        {
            if (feature.Attributes != null)
            {
                if (feature.Attributes.ContainsKey(name))
                {
                    feature.Attributes[name] = value;
                }
            }

            if (feature is DataRow)
            {
                ((DataRow) feature)[name] = value;
                return;
            }

            var featureType = feature.GetType();

            var propertyInfo = GetFeatureProperty(featureType, name);

            if (propertyInfo != null)
            {
                var setter = propertyInfo.GetSetMethod(true);
                if (setter != null)
                {
                    setter.Invoke(feature, new[]
                    {
                        value
                    });
                    return;
                }
            }

            if (throwIfNotFound)
            {
                ThrowOnNotFound(featureType, name);
            }
        }

        public static string GetAttributeExportName(IFeature feature, string name, bool throwIfNotFound = true)
        {
            if (feature.Attributes != null)
            {
                if (feature.Attributes.ContainsKey(name))
                {
                    return name;
                }
            }

            var propertyInfo = GetFeatureProperty(feature.GetType(), name);

            if (propertyInfo != null)
            {
                var attribute =
                    propertyInfo.GetCustomAttributes(true).OfType<FeatureAttributeAttribute>().FirstOrDefault();
                return attribute == null ? propertyInfo.Name : (attribute.ExportName ?? propertyInfo.Name);
            }

            if (throwIfNotFound)
            {
                ThrowOnNotFound(feature.GetType(), name);
            }

            return null;
        }

        public static string GetPropertyDisplayName(Type featureType, string name, bool throwIfNotFound = true)
        {
            if (featureType.Implements<DataRow>())
            {
                return name;
            }

            var propertyInfo = GetFeatureProperty(featureType, name);

            if (propertyInfo != null)
            {
                var displayNameAttribute =
                    propertyInfo.GetCustomAttributes(true).OfType<DisplayNameAttribute>().FirstOrDefault();

                if (displayNameAttribute != null)
                {
                    return !string.IsNullOrEmpty(displayNameAttribute.DisplayName)
                               ? displayNameAttribute.DisplayName
                               : propertyInfo.Name;
                }

                return propertyInfo.Name;
            }
            if (throwIfNotFound)
            {
                ThrowOnNotFound(featureType, name);
            }
            return null;
        }

        public static string GetAttributeDisplayName(IFeature feature, string name, bool throwIfNotFound = true)
        {
            if (feature.Attributes != null && feature.Attributes.ContainsKey(name))
            {
                return name;
            }
            return GetPropertyDisplayName(feature.GetType(), name, throwIfNotFound);
        }

        public static Type GetAttributeType(IFeature feature, string name, bool throwIfNotFound = true)
        {
            if (feature is DataRow)
            {
                var row = (DataRow) feature;
                if (row.Table.Columns.Contains(name))
                {
                    return row.Table.Columns[name].DataType;
                }
            }

            if (feature.Attributes != null)
            {
                if (feature.Attributes.ContainsKey(name))
                {
                    return feature.Attributes[name].GetType();
                }
            }

            var featureType = feature.GetType();

            var propertyInfo = GetFeatureProperty(featureType, name);

            if (propertyInfo != null)
            {
                return propertyInfo.PropertyType;
            }
            if (throwIfNotFound)
            {
                ThrowOnNotFound(featureType, name);
            }
            return null;
        }

        public static bool IsReadOnly(Type featureType, string name)
        {
            if (featureType.GetProperty(name).GetSetMethod() == null)
            {
                return true;
            }
            return GetAttributeReadonlyFlag(featureType, name);
        }

        public static string GetFormatString(Type featureType, string name)
        {
            var propertyInfo = GetFeatureProperty(featureType, name);

            if (propertyInfo == null)
            {
                return "";
            }

            var formatAttribute =
                propertyInfo.GetCustomAttributes(typeof(DisplayFormatAttribute), true).FirstOrDefault();

            return formatAttribute != null ? ((DisplayFormatAttribute) formatAttribute).FormatString : "";
        }

        private static Dictionary<PropertyInfo, FeatureAttributeAttribute> GetFeatureAttributes(Type featureType)
        {
            var result = new Dictionary<PropertyInfo, FeatureAttributeAttribute>();
            if (featureType == null)
            {
                return result;
            }
            foreach (var p in featureType.GetProperties())
            {
                var attribute = p.GetCustomAttributes(true).OfType<FeatureAttributeAttribute>().FirstOrDefault();
                if (attribute != null)
                {
                    result.Add(p, attribute);
                }
            }
            return result;
        }

        private static void ThrowOnNotFound(Type featureType, string name)
        {
            throw new ArgumentException("Can't find attribute " + name + " for type " + featureType.Name);
        }

        private static PropertyInfo GetFeatureProperty(Type featureType, string name)
        {
            return featureType == null ? null : featureType.GetProperties().FirstOrDefault(p => p.Name == name);
        }

        private static MethodInfo GetCachedAttributeValueGetter(IFeature feature, string name, bool throwIfNotFound = true)
        {
            MethodInfo getter;
            IDictionary<string, MethodInfo> gettersForType;

            if (!getterCache.TryGetValue(feature.GetType(), out gettersForType))
            {
                gettersForType = new Dictionary<string, MethodInfo>();
                getterCache.Add(feature.GetType(), gettersForType);
            }

            if (!gettersForType.TryGetValue(name, out getter))
            {
                getter = GetAttributeValueGetter(feature, name, throwIfNotFound);
                gettersForType.Add(name, getter);
            }

            return getter;
        }

        private static MethodInfo GetAttributeValueGetter(IFeature feature, string name, bool throwIfNotFound)
        {
            var propertyInfo = GetFeatureProperty(feature.GetType(), name);
            if (propertyInfo != null)
            {
                return propertyInfo.GetGetMethod(true);
            }
            if (throwIfNotFound)
            {
                ThrowOnNotFound(feature.GetType(), name);
            }
            return null;
        }

        private static bool GetAttributeReadonlyFlag(Type featureType, string name, bool throwIfNotFound = true)
        {
            if (featureType.Implements<DataRow>())
            {
                return false;
            }

            var propertyInfo = GetFeatureProperty(featureType, name);

            if (propertyInfo != null)
            {
                var readOnlyAttribute =
                    propertyInfo.GetCustomAttributes(true).OfType<ReadOnlyAttribute>().FirstOrDefault();

                return readOnlyAttribute != null && readOnlyAttribute.IsReadOnly;
            }
            if (throwIfNotFound)
            {
                ThrowOnNotFound(featureType, name);
            }
            return false;
        }
    }
}