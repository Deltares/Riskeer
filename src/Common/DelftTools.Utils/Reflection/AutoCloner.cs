using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using DelftTools.Utils.Aop;
using DelftTools.Utils.Collections;
using DelftTools.Utils.Collections.Generic;

namespace DelftTools.Utils.Reflection
{
    internal static class AutoCloner
    {
        private static IList<AggregateReplaceTask> postProcessTasks;
        private static IList<object> manualClones;
        private static CloneStore cloneStore;
        
        internal static T DeepClone<T>(T inst)
        {
            postProcessTasks = new List<AggregateReplaceTask>();
            manualClones = new List<object>();
            cloneStore = new CloneStore();
            try
            {
                var clone = (T)DeepCloneCore(inst);
                PostProcessAggregates();
                manualClones.ForEach(SearchAndReplaceAggregates);
                return clone;
            }
            finally
            {
                postProcessTasks = null;
                manualClones = null;
                cloneStore = null;
            }
        }

        private static void PostProcessAggregates()
        {
            DoWithEditActionsDisabled(() =>
                {
                    foreach (var todo in postProcessTasks)
                    {
                        var fi = todo.Member as FieldInfo;
                        var pi = todo.Member as PropertyInfo;
                        
                        var clone = cloneStore.GetExistingCloneFor(todo.OriginalValue) ?? //get existing clone
                            todo.OriginalValue;  //if no existing: restore to original value (since it lies outside the graph we're cloning)
                        
                        if (todo.IsListElement)
                        {
                            //if list, the list has already been replaced
                            var list = (IList) todo.Instance;
                            ReplaceListItem(list, todo.ListIndex, clone);
                        }
                        else
                        {
                            if (fi != null)
                                fi.SetValue(todo.Instance, clone);
                            else
                                pi.SetValue(todo.Instance, clone, null);
                        }
                    }
                });
        }

        private static object DeepCloneCore(object inst)
        {
            object o;
            if (GetExistingClone(ref inst, out o)) 
                return o;

            // check if type has manual clone implemented, use it!
            var asManualCloneable = inst as IManualCloneable;
            if (asManualCloneable != null)
            {
                var manualClone = asManualCloneable.Clone();
                manualClones.Add(manualClone);
                cloneStore.AddClone(inst, manualClone);
                return manualClone;
            }
            var instType = inst.GetType();

            // clone arrays
            if (instType.IsArray)
            {
                return DeepCloneArray((Array) inst);
            }

            // geometries
            var baseType = instType.BaseType;
            if ((baseType != null && baseType.Name == "Geometry") && inst is ICloneable)
            {
                return ((ICloneable) inst).Clone();
            }

            // gdi stuff
            if (instType.FullName.StartsWith("System.Drawing.") && inst is ICloneable)
            {
                return ((ICloneable) inst).Clone();
            }
            // datatable stuff
            if (inst is DataTable)
            {
                var copy = DeepCloneDataTable(inst as DataTable);
                cloneStore.AddClone(inst, copy);
                return copy;
            }
            // nhib generic list
            if (instType.Name.StartsWith("PersistentGenericList"))
            {
                var clonedList = CreateInstanceOfList(inst);
                cloneStore.AddClone(inst, clonedList);
                DeepCloneList((IList) inst, clonedList);
                return clonedList;
            }
            // nhib evented list
            if (instType.Name.StartsWith("PersistentEvented"))
            {
                var clonedList = CreateInstanceOfList(inst);
                cloneStore.AddClone(inst, clonedList);
                DeepCloneList((IList) inst, clonedList);
                return clonedList;
            }
            //nhib generic dict
            if (instType.Name.StartsWith("PersistentGenericMap"))
            {
                var dict = (IDictionary) inst;
                var genericArguments = instType.GetGenericArguments().ToArray();
                var clonedDict = (IDictionary) TypeUtils.CreateGeneric(typeof (Dictionary<,>),
                                                                       new[] {genericArguments[0], genericArguments[1]});
                cloneStore.AddClone(inst, clonedDict);

                foreach (var item in dict.Keys)
                {
                    clonedDict[DeepCloneCore(item)] = DeepCloneCore(item);
                }

                return clonedDict;
            }

            // do reflection-based clone
            return DeepCloneAuto(inst);
        }

        private static IList CreateInstanceOfList(object inst)
        {
            if (inst.GetType().Name.StartsWith("PersistentGenericList"))
            {
                return (IList) TypeUtils.CreateGeneric(typeof (List<>), inst.GetType().GetGenericArguments().First());
            }
            if (inst.GetType().Name.StartsWith("PersistentEvented"))
            {
                return (IList) TypeUtils.CreateGeneric(typeof (EventedList<>), inst.GetType().GetGenericArguments().First());
            }
            return (IList) TypeUtils.CreateInstance<object>(inst.GetType());
        }

        private static bool GetExistingClone(ref object inst, out object existingClone)
        {
            if (IsInstanceOfSimpleType(inst))
            {
                existingClone = inst;
                return true;
            }

            // unproxy first (if applicable): nhibernate
            inst = TypeUtils.Unproxy(inst);

            // check for existing clones
            existingClone = cloneStore.GetExistingCloneFor(inst);
            return existingClone != null;
        }

        private static DataTable DeepCloneDataTable(DataTable dataTable)
        {
            using (var memoryStream = new MemoryStream())
            {
                var binaryFormatter = new BinaryFormatter();
                binaryFormatter.Serialize(memoryStream, dataTable);
                memoryStream.Seek(0, 0);
                return (DataTable)binaryFormatter.Deserialize(memoryStream);
            }
        }
        
        private static bool IsSimpleType(Type type)
        {
            return (type == typeof (string) || type.IsValueType);
        }

        private static bool IsInstanceOfSimpleType(object inst)
        {
            return inst == null || inst is string || inst is Type || inst is MemberInfo || inst is CultureInfo ||
                   inst.GetType().IsValueType;
        }

        private static object DeepCloneAuto(object inst)
        {
            // create instance
            var clone = TypeUtils.CreateInstance<object>(inst.GetType());
            cloneStore.AddClone(inst, clone);

            DeepClonePropertiesAndFields(inst, clone);

            // clone list elements
            var list = inst as IList;
            var dict = inst as IDictionary;
            if (dict != null)
            {
                DeepCloneDictionary(dict, (IDictionary)clone);
            }
            else if (list != null)
            {
                DeepCloneList(list, (IList)clone);
            }

            return clone;
        }

        private static void DeepClonePropertiesAndFields(object inst, object clone)
        {
            var aggregationProperties = new List<string>();

            // set in all properties (to trigger subscription etc)
            foreach (var pi in OrderByComplexity(GetAllAccessiblePropertiesForType(inst.GetType())))
            {
                object value;
                if (!GetValue(inst, pi, out value))
                    continue;

                object clonedValue;

                if (AttributeInfoCache.IsAggregation(pi)) //if aggregate: only set existing clones
                {
                    aggregationProperties.Add(pi.Name);

                    if (ProcessAggregateMember(value, clone, pi, out clonedValue))
                        continue;
                }
                else
                {
                    clonedValue = DeepCloneCore(value);
                }

                SetValue(pi, clone, clonedValue);
            }

            // set in all fields that we missed so far
            foreach (var fieldAndValue in GetNonInfrastructureFields(inst))
            {
                var fi = fieldAndValue.Key;
                var value = fieldAndValue.Value;

                if (!ShouldReplaceValue(value))
                    continue;

                if (!IsInstanceOfSimpleType(value))
                    if (cloneStore.IsAlreadyClonedInstance(value))
                        continue;

                object clonedValue;
                if (aggregationProperties.Any(p => IsFieldForProperty(fi, p)))
                {
                    if (ProcessAggregateMember(value, clone, fi, out clonedValue))
                        continue;
                }
                else
                {
                    clonedValue = DeepCloneCore(value);
                }

                SetValue(fi, clone, clonedValue);
            }
        }

        private static bool ProcessAggregateMember(object value, object clone, MemberInfo pi, out object clonedValue)
        {
            value = TypeUtils.Unproxy(value);

            var list = value as IList;
            if (list != null) //special case :-(
            {
                clonedValue = CreateInstanceOfList(list);

                if (value.GetType() == clonedValue.GetType()) //hack: only if types match (not so for persistent evented lists)
                    DeepClonePropertiesAndFields(value, clonedValue);

                var clonedList = (IList)clonedValue;
                for (var i = 0; i < list.Count; i++)
                {
                    var item = list[i];
                    object clonedItem;
                    if (!GetExistingClone(ref item, out clonedItem))
                    {
                        postProcessTasks.Add(new AggregateReplaceTask
                            {
                                Instance = clonedList,
                                Member = pi,
                                OriginalValue = item,
                                IsListElement = true,
                                ListIndex = i
                            });
                        DoWithEditActionsDisabled(() => clonedList.Add(null)); //add placeholder
                        continue;
                    }
                    DoWithEditActionsDisabled(() => clonedList.Add(clonedItem));
                }
            }
            else
            {
                if (!GetExistingClone(ref value, out clonedValue))
                {
                    postProcessTasks.Add(new AggregateReplaceTask {Instance = clone, Member = pi, OriginalValue = value});
                    return true;
                }
            }
            return false;
        }

        private static bool IsFieldForProperty(FieldInfo fi, string propertyName)
        {
            if (fi.Name.Equals(propertyName, StringComparison.InvariantCultureIgnoreCase))
                return true;
            if (fi.Name == string.Format("<{0}>k__BackingField", propertyName))
                return true;
            return false;
        }

        private static IEnumerable<PropertyInfo> OrderByComplexity(IEnumerable<PropertyInfo> propertyInfos)
        {
            var props = propertyInfos.ToList();

            foreach (var prop in props.Where(p => IsSimpleType(p.PropertyType)))
            {
                yield return prop;
            }
            foreach (var prop in props.Where(p => !IsSimpleType(p.PropertyType)))
            {
                yield return prop;
            }
        }

        private static IEnumerable<KeyValuePair<FieldInfo, object>> GetNonInfrastructureFields(object inst)
        {
            var pairs = new List<KeyValuePair<FieldInfo, object>>();

            foreach (var pair in TypeUtils.GetNonInfrastructureFields(inst, inst.GetType()))
            {
                if (pair.Key.DeclaringType != null && pair.Key.DeclaringType.Namespace.StartsWith("System"))
                    continue; //skip internal .NET fields (eg in List and Dictionary)

                pairs.Add(pair);
            }

            return pairs;
        }

        private static void SetValue(MemberInfo mi, object clone, object clonedValue)
        {
            try
            {
                var propertyInfo = mi as PropertyInfo;
                var fieldInfo = mi as FieldInfo;
                DoWithEditActionsDisabled(
                    () =>
                        {
                            if (propertyInfo != null)
                                propertyInfo.SetValue(clone, clonedValue, null);
                            else
                                fieldInfo.SetValue(clone, clonedValue);
                        });
            }
            catch (Exception e)
            {
                throw new InvalidOperationException(
                    string.Format("Clone: Setting property {0} on class {1} resulted in exception:\n {2}", mi.Name,
                                  mi.DeclaringType, e));
            }
        }

        private static bool GetValue(object inst, PropertyInfo pi, out object value)
        {
            value = null;

            if (pi.Name == "Id")
                return false;

            // we're using these as marker attribute: do not set..todo: consider using a dedicated attribute
            if (AttributeInfoCache.IsNonSerialized(pi) || AttributeInfoCache.IsNoNotify(pi))
                return false;

            try
            {
                value = pi.GetValue(inst, null);
            }
            catch (Exception)
            {
                //gulp: exception during getter; just don't set either
                return false;
            }

            if (!ShouldReplaceValue(value)) 
                return false;

            return true;
        }
        
        private static bool ShouldReplaceValue(object value)
        {
            if (value == null)
                return false;

            if (value is Delegate)
                return false;

            if (!(value is string) && value is IEnumerable && !(value is ICollection || value is DataTable || IsGenericCollection(value)))
                return false;
            return true;
        }

        private static bool IsGenericCollection(object o)
        {
            return o.GetType().GetInterfaces().Any(i => i.FullName.StartsWith("System.Collections.Generic.ICollection`1"));
        }

        private static object DeepCloneArray(Array instArray)
        {
            var clonedArray = Array.CreateInstance(instArray.GetType().GetElementType(), instArray.Length);
            for (int i = 0; i < instArray.Length; i++)
            {
                clonedArray.SetValue(DeepCloneCore(instArray.GetValue(i)), i);
            }
            return clonedArray;
        }

        private static void DeepCloneList(IList list, IList clonedList)
        {
            //sometimes shallow clones already copies the elements (as-is) and sometimes it doesn't..
            if (clonedList.Count == 0 && list.Count > 0) 
            {
                foreach (var item in list)
                {
                    var clonedItem = DeepCloneCore(item);
                    DoWithEditActionsDisabled(() => clonedList.Add(clonedItem));
                }
            }
            else
            {
                for (int i = 0; i < list.Count; i++)
                {
                    var item = list[i];
                    var clonedItem = DeepCloneCore(item);
                    ReplaceListItem(clonedList, i, clonedItem);
                }
            }
        }

        private static void DeepCloneDictionary(IDictionary list, IDictionary clone)
        {
            var keys = list.Keys.OfType<object>().ToList();
            var values = list.Values.OfType<object>().ToList();

            var clonedKeysLookup = keys.ToDictionary(k => k, DeepCloneCore);
            var clonedValuesLookup = values.ToDictionary(v => v, DeepCloneCore);

            clone.Clear();
            foreach (var key in list.Keys)
            {
                clone[clonedKeysLookup[key]] = clonedValuesLookup[list[key]];
            }
        }

        private static void ReplaceListItem(IList clonedList, int i, object clonedItem)
        {
            DoWithEditActionsDisabled(() =>
                {
                    clonedList.RemoveAt(i);
                    clonedList.Insert(i, clonedItem);
                });
        }

        private static IEnumerable<PropertyInfo> GetAllAccessiblePropertiesForType(Type type)
        {
            return type == null
                       ? new PropertyInfo[0]
                       : type.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                             .Where(pi => pi.GetIndexParameters().Length == 0 && pi.GetSetMethod(true) != null)
                             .Concat(GetAllAccessiblePropertiesForType(type.BaseType));
        }
        
        private static void DoWithEditActionsDisabled(Action action)
        {
            var oldDisabledSetting = EditActionSettings.Disabled;
            var oldRestoreSetting = EditActionSettings.AllowRestoreActions;
            try
            {
                EditActionSettings.Disabled = true;
                EditActionSettings.AllowRestoreActions = true;
                action();
            }
            finally
            {
                EditActionSettings.Disabled = oldDisabledSetting;
                EditActionSettings.AllowRestoreActions = oldRestoreSetting;
            }
        }

        private static void SearchAndReplaceAggregates(object root)
        {
            var visitedItems = new HashSet<object>();
            var queue = new Queue<object>();
            queue.Enqueue(root);

            while (queue.Count > 0)
            {
                var inst = queue.Dequeue();

                if (IsInstanceOfSimpleType(inst))
                    continue;
                
                // geometries
                var baseType = inst.GetType().BaseType;
                if ((baseType != null && baseType.Name == "Geometry") && inst is ICloneable)
                    continue;

                // gdi stuff
                if (inst.GetType().FullName.StartsWith("System.Drawing.") && inst is ICloneable)
                    continue;

                if (visitedItems.Contains(inst))
                    continue;
                visitedItems.Add(inst);

                foreach (var pi in GetAllAccessiblePropertiesForType(inst.GetType()))
                {
                    object value;
                    if (!GetValue(inst, pi, out value))
                        continue;

                    if (AttributeInfoCache.IsAggregation(pi))
                    {
                        if (value is IList) //lists are special case of aggregation
                        {
                            var clone = (IList) value; //already cloned..just not the values

                            for (var i = 0; i < clone.Count; i++)
                            {
                                var item = clone[i];
                                var clonedItem = cloneStore.GetExistingCloneFor(item);
                                if (clonedItem != null)
                                {
                                    ReplaceListItem(clone, i, clonedItem);
                                }
                            }
                        }
                        else
                        {
                            var clone = cloneStore.GetExistingCloneFor(value);
                            if (clone != null)
                            {
                                SetValue(pi, inst, clone);
                            }
                        }
                    }
                    else
                    {
                        queue.Enqueue(value);
                    }
                }
                if (inst is IList)
                {
                    var list = inst as IList;
                    foreach (var item in list)
                    {
                        queue.Enqueue(item);
                    }
                }
            }
        }

        private class AggregateReplaceTask
        {
            public object Instance;
            public MemberInfo Member;
            public object OriginalValue;
            public bool IsListElement;
            public int ListIndex;

            public override string ToString()
            {
                return string.Format("{0} {1}{3} - {2}", Instance, Member.Name, OriginalValue,
                                     IsListElement ? string.Format("[{0}]", ListIndex) : "");
            }
        }

        private static class AttributeInfoCache
        {
            private static readonly IDictionary<PropertyInfo, AttributeDetails> PropertyAttributeInfo =
                new Dictionary<PropertyInfo, AttributeDetails>();

            private struct AttributeDetails
            {
                public bool NonSerialized;
                public bool NoNotify;
                public bool Aggregation;
            }

            public static bool IsAggregation(PropertyInfo pi)
            {
                AttributeDetails info;
                Initialize(pi, out info);
                return info.Aggregation;
            }
            public static bool IsNoNotify(PropertyInfo pi)
            {
                AttributeDetails info;
                Initialize(pi, out info);
                return info.NoNotify;
            }

            public static bool IsNonSerialized(PropertyInfo pi)
            {
                AttributeDetails info;
                Initialize(pi, out info);
                return info.NonSerialized;
            }

            private static void Initialize(PropertyInfo pi, out AttributeDetails info)
            {
                if (!PropertyAttributeInfo.TryGetValue(pi, out info))
                {
                    var attributes = pi.GetCustomAttributes(false); //or true?
                    PropertyAttributeInfo[pi] = new AttributeDetails
                        {
                            Aggregation = attributes.Any(a => a is AggregationAttribute),
                            NoNotify = attributes.Any(a => a is NoNotifyPropertyChangeAttribute),
                            NonSerialized = attributes.Any(a => a is NonSerializedAttribute)
                        };
                }
            }
        }

        private class CloneStore
        {
            private readonly IDictionary<object, object> objectToCloneLookup = new Dictionary<object, object>();
            private readonly HashSet<object> clones = new HashSet<object>();

            public void AddClone(object inst, object clone)
            {
                objectToCloneLookup.Add(inst, clone);
                clones.Add(clone); //performance
            }

            public object GetExistingCloneFor(object inst)
            {
                object clone;
                return objectToCloneLookup.TryGetValue(inst, out clone) ? clone : null;
            }

            public bool IsAlreadyClonedInstance(object clone)
            {
                //lookup in hashset iso dictionary Values for performance
                return clones.Contains(clone);
            }
        }
    }

}