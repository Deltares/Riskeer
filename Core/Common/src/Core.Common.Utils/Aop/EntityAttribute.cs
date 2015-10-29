using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using Core.Common.Utils.Aop.Markers;
using Core.Common.Utils.Collections;
using Core.Common.Utils.Collections.Generic;
using PostSharp.Aspects;
using PostSharp.Aspects.Advices;
using PostSharp.Extensibility;
using PostSharp.Reflection;

namespace Core.Common.Utils.Aop
{
    [Serializable]
    [MulticastAttributeUsage(MulticastTargets.Class, Inheritance = MulticastInheritance.None)]
    [IntroduceInterface(typeof(INotifyPropertyChange), OverrideAction = InterfaceOverrideAction.Ignore)]
    [IntroduceInterface(typeof(INotifyCollectionChange), OverrideAction = InterfaceOverrideAction.Ignore)]
    public class EntityAttribute : InstanceLevelAspect, INotifyPropertyChange, INotifyCollectionChange
    {
        [IntroduceMember(OverrideAction = MemberOverrideAction.Ignore, IsVirtual = true)]
        public event NotifyCollectionChangingEventHandler CollectionChanging;

        [IntroduceMember(OverrideAction = MemberOverrideAction.Ignore, IsVirtual = true)]
        public event NotifyCollectionChangedEventHandler CollectionChanged;

        [IntroduceMember(Visibility = Visibility.Public, IsVirtual = true, OverrideAction = MemberOverrideAction.Ignore)]
        public event PropertyChangingEventHandler PropertyChanging;

        [IntroduceMember(Visibility = Visibility.Public, IsVirtual = true, OverrideAction = MemberOverrideAction.Ignore)]
        public event PropertyChangedEventHandler PropertyChanged;

        public enum EntityEventType
        {
            PropertyChanging,
            PropertyChanged,
            CollectionChanging,
            CollectionChanged
        }

        [ThreadStatic]
        private static bool inInitialize;

        public bool FireOnPropertyChange = true;
        public bool FireOnCollectionChange = true;

        [ImportMember("OnEntityEvent", IsRequired = false, Order = ImportMemberOrder.AfterIntroductions)]
        public Action<object, EventArgs, EntityEventType> OnEntityEventMethod;

        public bool HasParentIsCheckedInItems
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public bool SkipChildItemEventBubbling
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public bool HasParent
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        [IntroduceMember(Visibility = Visibility.Family, IsVirtual = true, OverrideAction = MemberOverrideAction.Ignore)]
        public void OnCollectionChanging(object sender, NotifyCollectionChangingEventArgs e)
        {
            if (CollectionChanging == null)
            {
                var change = Instance as ICustomNotifyCollectionChange;
                if (change != null)
                {
                    change.FireCollectionChanging(sender, e);
                }
                return;
            }

            LogCollectionChanging(sender, e);

            var previousSender = EventSettings.LastEventBubbler;
            EventSettings.LastEventBubbler = Instance;

            try
            {
                CollectionChanging(sender, e);
            }
            finally
            {
                EventSettings.LastEventBubbler = previousSender;
            }
        }

        [IntroduceMember(Visibility = Visibility.Family, IsVirtual = true, OverrideAction = MemberOverrideAction.Ignore)]
        public void OnCollectionChanged(object sender, NotifyCollectionChangingEventArgs e)
        {
            if (CollectionChanged == null)
            {
                var change = Instance as ICustomNotifyCollectionChange;
                if (change != null)
                {
                    change.FireCollectionChanged(sender, e);
                }
                return;
            }

            var previousSender = EventSettings.LastEventBubbler;
            EventSettings.LastEventBubbler = Instance;

            try
            {
                CollectionChanged(sender, e);
            }
            finally
            {
                EventSettings.LastEventBubbler = previousSender;
            }
        }

        [IntroduceMember(Visibility = Visibility.Family, IsVirtual = true, OverrideAction = MemberOverrideAction.Ignore)]
        public void OnEntityEvent(object sender, EventArgs e, EntityEventType entityEventType)
        {
            switch (entityEventType)
            {
                case EntityEventType.PropertyChanging:
                    OnPropertyChanging(sender, (PropertyChangingEventArgs) e);
                    break;
                case EntityEventType.PropertyChanged:
                    OnPropertyChanged(sender, (PropertyChangedEventArgs) e);
                    break;
                case EntityEventType.CollectionChanging:
                    OnCollectionChanging(sender, (NotifyCollectionChangingEventArgs) e);
                    break;
                case EntityEventType.CollectionChanged:
                    OnCollectionChanged(sender, (NotifyCollectionChangingEventArgs) e);
                    break;
            }
        }

        [IntroduceMember(Visibility = Visibility.Family, IsVirtual = true, OverrideAction = MemberOverrideAction.Ignore)]
        public void OnPropertyChanging(object sender, PropertyChangingEventArgs e)
        {
            if (PropertyChanging == null)
            {
                return;
            }

            LogPropertyChanging(sender, e);

            var previousSender = EventSettings.LastEventBubbler;
            EventSettings.LastEventBubbler = Instance;

            try
            {
                PropertyChanging(sender, e);
            }
            finally
            {
                EventSettings.LastEventBubbler = previousSender;
            }
        }

        [IntroduceMember(Visibility = Visibility.Family, IsVirtual = true, OverrideAction = MemberOverrideAction.Ignore)]
        public void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (PropertyChanged == null)
            {
                return;
            }

            var previousSender = EventSettings.LastEventBubbler;
            EventSettings.LastEventBubbler = Instance;

            try
            {
                PropertyChanged(sender, e);
            }
            finally
            {
                EventSettings.LastEventBubbler = previousSender;
            }
        }

        [OnLocationSetValueAdvice]
        [MethodPointcut("GetPropertiesToNotifyFor")]
        public void OnPropertySet(LocationInterceptionArgs args)
        {
            var fireEvents = EventSettings.BubblingEnabled && FireOnPropertyChange;
            var instance = Instance;
            var locationName = args.LocationName;

            if (fireEvents)
            {
                EditActionAttribute.FireBeforeEventCall(instance, true);
                OnEntityEventMethod(instance, new PropertyChangingEventArgs(locationName),
                                    EntityEventType.PropertyChanging);
            }

            try
            {
                args.ProceedSetValue();
            }
            finally
            {
                if (fireEvents)
                {
                    try
                    {
                        OnEntityEventMethod(instance, new PropertyChangedEventArgs(locationName),
                                            EntityEventType.PropertyChanged);
                    }
                    finally
                    {
                        EditActionAttribute.FireAfterEventCall(instance, true, false);
                    }
                }
            }
        }

        [OnLocationSetValueAdvice]
        [MethodPointcut("GetFieldsToSubscribeTo")]
        public void OnFieldSet(LocationInterceptionArgs args)
        {
            if (FireOnPropertyChange || FireOnCollectionChange)
            {
                Unsubscribe(args.GetCurrentValue());
                Subscribe(args.Value);
            }

            args.ProceedSetValue();
        }

        /// <summary>
        /// </summary>
        /// <remarks>seperate method to take a compile time iso runtime performance hit</remarks>
        /// <param name="args"></param>
        [OnLocationSetValueAdvice]
        [MethodPointcut("GetAggregationListFieldsToSubscribeTo")]
        public void OnFieldSetAggregationList(LocationInterceptionArgs args)
        {
            var notifyCollectionChange = args.Value as INotifyCollectionChange;
            if (notifyCollectionChange != null)
            {
                notifyCollectionChange.SkipChildItemEventBubbling = true;
            }

            OnFieldSet(args);
        }

        public override void RuntimeInitializeInstance()
        {
            if (inInitialize)
            {
                return;
            }

            base.RuntimeInitializeInstance();

            inInitialize = true;
            ManualAspectInitializer.InitializeAspects(Instance);
            inInitialize = false;
        }

        [Conditional("TRACING")]
        private void LogCollectionChanging(object sender, NotifyCollectionChangingEventArgs e)
        {
            //if (EventSettings.EnableLogging)
            {
                var instance = Instance;
                if (instance == sender)
                {
                    Console.WriteLine(
                        "CollectionChanging >>> '{0}[{1}]', item:{2}, index:{3}, action:{4} - BEGIN >>>>>>>>>>>>>>",
                        instance, instance != null ? instance.GetType().Name : "null", e.Item, e.Index, e.Action);
                }
                else if (sender.GetType().Name.Contains("EventedList"))
                {
                    var senderTypeName = sender.GetType().GetGenericArguments()[0].Name;
                    Console.WriteLine(
                        "CollectionChanging >>> '{0}[{1}]' -> '{5}[{6}]', item:{2}, index:{3}, action:{4}",
                        "EventedList", senderTypeName, e.Item, e.Index, e.Action, instance,
                        instance != null ? instance.GetType().Name : "null");
                }
                else
                {
                    Console.WriteLine(
                        "CollectionChanging >>> '{0}[{1}]' -> '{5}[{6}]', item:{2}, index:{3}, action:{4}", sender,
                        sender.GetType().Name, e.Item, e.Index, e.Action, instance,
                        instance != null ? instance.GetType().Name : "null");
                }
            }
        }

        [Conditional("TRACING")]
        private void LogPropertyChanging(object sender, PropertyChangingEventArgs e)
        {
            var instance = Instance;
            var sourceTypeName = sender != null ? sender.GetType().Name : "<no type>";
            var instanceTypeName = instance != null ? instance.GetType().Name : "<no type>";
            Console.WriteLine("PropertyChanging >>> {0}.{1} ({2} [{3}])",
                              sourceTypeName, e.PropertyName, instance, instanceTypeName);
        }

        private void Subscribe(object value, bool subscribePropertyChanged = true)
        {
            if (subscribePropertyChanged && FireOnPropertyChange)
            {
                var propertyChanging = value as INotifyPropertyChanging;
                if (propertyChanging != null)
                {
                    propertyChanging.PropertyChanging += ChildPropertyChanging;
                }
                var propertyChanged = value as INotifyPropertyChanged;
                if (propertyChanged != null)
                {
                    propertyChanged.PropertyChanged += ChildPropertyChanged;
                }
            }
            if (FireOnCollectionChange)
            {
                var collectionChanging = value as INotifyCollectionChanging;
                if (collectionChanging != null)
                {
                    collectionChanging.CollectionChanging += ChildCollectionChanging;
                }
                var collectionChanged = value as INotifyCollectionChanged;
                if (collectionChanged != null)
                {
                    collectionChanged.CollectionChanged += ChildCollectionChanged;
                }
            }
        }

        private void ChildCollectionChanging(object sender, NotifyCollectionChangingEventArgs e)
        {
            OnEntityEventMethod(sender, e, EntityEventType.CollectionChanging);
        }

        private void ChildCollectionChanged(object sender, NotifyCollectionChangingEventArgs e)
        {
            OnEntityEventMethod(sender, e, EntityEventType.CollectionChanged);
        }

        private void Unsubscribe(object oldValue)
        {
            if (FireOnPropertyChange)
            {
                var oldPropertyChanging = oldValue as INotifyPropertyChanging;
                if (oldPropertyChanging != null)
                {
                    oldPropertyChanging.PropertyChanging -= ChildPropertyChanging;
                }
                var oldPropertyChanged = oldValue as INotifyPropertyChanged;
                if (oldPropertyChanged != null)
                {
                    oldPropertyChanged.PropertyChanged -= ChildPropertyChanged;
                }
            }
            if (FireOnCollectionChange)
            {
                var collectionChanging = oldValue as INotifyCollectionChanging;
                if (collectionChanging != null)
                {
                    collectionChanging.CollectionChanging -= ChildCollectionChanging;
                }
                var collectionChanged = oldValue as INotifyCollectionChanged;
                if (collectionChanged != null)
                {
                    collectionChanged.CollectionChanged -= ChildCollectionChanged;
                }
            }
        }

        private void ChildPropertyChanging(object sender, PropertyChangingEventArgs e)
        {
            OnEntityEventMethod(sender, e, EntityEventType.PropertyChanging);
        }

        private void ChildPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            OnEntityEventMethod(sender, e, EntityEventType.PropertyChanged);
        }

        #region Compile Time

        private IEnumerable<PropertyInfo> GetPropertiesToNotifyFor(Type type)
        {
            if (type.IsDefined(typeof(CompilerGeneratedAttribute), false))
            {
                return new PropertyInfo[0];
            }
            return
                from property in type.GetProperties(BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.Public)
                where ShouldNotifyFor(property)
                select property;
        }

        private static bool ShouldNotifyFor(PropertyInfo property)
        {
            return property.CanWrite && !property.IsDefined(typeof(NoNotifyPropertyChangeAttribute), false);
        }

        private static PropertyInfo GetPropertyForField(FieldInfo field)
        {
            var declaringType = field.DeclaringType;

            if (declaringType == null)
            {
                return null;
            }

            return declaringType
                .GetProperties()
                .FirstOrDefault(p => p.Name.ToLower() == field.Name.ToLower() ||
                                     field.Name == string.Format("<{0}>k__BackingField", p.Name));
        }

        private static bool IsEventedListAggregation(FieldInfo fieldInfo)
        {
            return fieldInfo.FieldType.IsGenericType &&
                   fieldInfo.FieldType.GetGenericTypeDefinition() == typeof(IEventedList<>);
        }

        private IEnumerable<FieldInfo> GetFieldsToSubscribeTo(Type type)
        {
            return GetApplicableFields(type)
                .Where(info => !info.PropertyInfo.IsDefined(typeof(AggregationAttribute), false))
                .Select(i => i.FieldInfo);
        }

        private IEnumerable<FieldInfo> GetAggregationListFieldsToSubscribeTo(Type type)
        {
            return GetApplicableFields(type)
                .Where(info => info.PropertyInfo.IsDefined(typeof(AggregationAttribute), false) &&
                               IsEventedListAggregation(info.FieldInfo))
                .Select(info => info.FieldInfo);
        }

        private struct FieldPropertyInfo
        {
            public readonly FieldInfo FieldInfo;
            public readonly PropertyInfo PropertyInfo;

            public FieldPropertyInfo(FieldInfo fieldInfo, PropertyInfo propertyInfo)
            {
                FieldInfo = fieldInfo;
                PropertyInfo = propertyInfo;
            }
        }

        private static IEnumerable<FieldPropertyInfo> GetApplicableFields(Type type)
        {
            if (type.IsDefined(typeof(CompilerGeneratedAttribute), false))
            {
                return new FieldPropertyInfo[0];
            }

            return type.GetFields(BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.NonPublic)
                       .Where(field =>
                              !(field.FieldType == typeof(string)) &&
                              !field.FieldType.IsValueType &&
                              !field.IsDefined(typeof(NoNotifyPropertyChangeAttribute), false))
                       .Select(field => new FieldPropertyInfo(field, GetPropertyForField(field)))
                       .Where(t => t.PropertyInfo != null && ShouldNotifyFor(t.PropertyInfo));
        }

        #endregion
    }

    /// <summary>
    /// Settings for EntityAttribute behavior
    /// </summary>
    /// <remarks>Seperate class from EntityAttribute to remove need to reference PostSharp if you just want to check/modify these fields</remarks>
    public static class EventSettings
    {
        public static bool BubblingEnabled = true;
        public static bool EnableLogging = false;
        public static object LastEventBubbler;

        #region Global Helpers

        public static void FirePropertyChanging(object lastSender, object originalSender,
                                                PropertyChangingEventArgs args,
                                                PropertyChangingEventHandler fireAction)
        {
            if (fireAction == null)
            {
                return;
            }

            var previousSender = LastEventBubbler;
            LastEventBubbler = lastSender;

            try
            {
                fireAction(originalSender, args);
            }
            finally
            {
                LastEventBubbler = previousSender;
            }
        }

        public static void FirePropertyChanged(object lastSender, object originalSender,
                                               PropertyChangedEventArgs args,
                                               PropertyChangedEventHandler fireAction)
        {
            if (fireAction == null)
            {
                return;
            }

            var previousSender = LastEventBubbler;
            LastEventBubbler = lastSender;

            try
            {
                fireAction(originalSender, args);
            }
            finally
            {
                LastEventBubbler = previousSender;
            }
        }

        #endregion
    }
}