using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace DelftTools.Utils.Aop
{
    /// <summary>
    /// This class takes care of manually initializing uninitialized entity aspects. Each level of the instance 
    /// (type and all base types) may have their own aspects. We need to do this explicitly because sometimes, 
    /// due to virtual calls in (base) constructors, aspects are required before the (derived) constructor of a 
    /// class has ran. In the constructor PostSharp initializes the aspect for each class.
    /// </summary>
    /// <remarks>There is still an open issue: any fields touched before the (derived) constructor executes are 
    /// subscribed to the wrong entity attribute. This can happen if you have virtual properties that don't call
    /// base but use their own field.</remarks>
    /// <remarks>It may not look this way, but this code is already pretty optimized. If you see room for 
    /// improvement, please verify it using the tests in EntityAttributeTest</remarks>
    internal static class ManualAspectInitializer
    {
        public static void InitializeAspects(object instance)
        {
            InitializeAspectsManually(instance, GetAspectInfo(instance.GetType()));
        }

        private static void InitializeAspectsManually(object instance, AspectInfo info)
        {
            while (info != null)
            {
                var baseInfo = info.BaseAspectInfo;

                if (baseInfo == null)
                {
                    //current info is of the lowest baseclass (there is no baseinfo), skip because we know this 
                    //must already have been initialized (since that triggered us to come here)
                    return;
                }

                if (!info.IsEmpty)
                {
                    var aspect = info.AspectGetter(instance);

                    if (aspect != null)
                        return; //aspect already initialized, don't do it again!

                    //actual initialize
                    info.InitializeMethod.Invoke(instance, new object[0]);
                }
                info = baseInfo;
            }
        }
        
        private static AspectInfo GetAspectInfo(Type type)
        {
            //first check the first level cache: we can expect subsequent calls for the same type due to the following reasons:
            //1. Creating one instance calls this method for each type in the hierarchy (always with the toplevel type)
            //2. We usually construct multiple objects of the same type in a row

            lock (LastTypeLock)
            {
                if (lastType != type)
                {
                    lastType = type;
                    lastAspectInfo = LookupAspectInfoForHierarchy(type);
                }
                return lastAspectInfo;
            }
        }

        private static AspectInfo LookupAspectInfoForHierarchy(Type typeLevel)
        {
            bool createdNew;
            var info = LookupAspectInfoForType(typeLevel, out createdNew);

            if (createdNew)
            {
                var baseType = typeLevel.BaseType;
                if (baseType != null)
                    info.BaseAspectInfo = LookupAspectInfoForHierarchy(baseType);
            }
            return info;
        }

        private static AspectInfo LookupAspectInfoForType(Type typeLevel, out bool created)
        {
            AspectInfo aspectInfo;
            created = false;

            if (!AspectLookup.TryGetValue(typeLevel, out aspectInfo))
            {
                var aspectField = typeLevel.GetFields(BindingFlags.Instance | BindingFlags.NonPublic)
                                           .SingleOrDefault(f => f.FieldType == typeof(EntityAttribute));

                if (aspectField != null)
                {
                    var initializeMethod = typeLevel.GetMethod("<>z__InitializeAspects",
                                                               BindingFlags.Instance | BindingFlags.NonPublic);
                    aspectInfo = new AspectInfo(FieldGetterUsingIl<EntityAttribute>(typeLevel, aspectField),
                                                initializeMethod);
                }
                else
                {
                    aspectInfo = new AspectInfo();
                }
                AspectLookup.Add(typeLevel, aspectInfo);
                created = true;
            }
            return aspectInfo;
        }

        private static FieldGetterDelegate<TValue> FieldGetterUsingIl<TValue>(Type objectType, FieldInfo fieldInfo)
        {
            var dm = new DynamicMethod("GetAspect", typeof(TValue), new[] { typeof(object) }, objectType, true);
            var il = dm.GetILGenerator();
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldfld, fieldInfo);
            il.Emit(OpCodes.Ret);
            return (FieldGetterDelegate<TValue>)dm.CreateDelegate(typeof(FieldGetterDelegate<TValue>));
        }

        private class AspectInfo
        {
            public readonly FieldGetterDelegate<EntityAttribute> AspectGetter;
            public readonly MethodInfo InitializeMethod;
            public readonly bool IsEmpty = false;
            public AspectInfo BaseAspectInfo;

            public AspectInfo()
            {
                IsEmpty = true;
            }

            public AspectInfo(FieldGetterDelegate<EntityAttribute> aspectGetter, MethodInfo initializeMethod)
            {
                AspectGetter = aspectGetter;
                InitializeMethod = initializeMethod;
            }
        }

        private delegate T FieldGetterDelegate<out T>(object obj);
        
        private static readonly object LastTypeLock = new object();
        //'first level cache'
        private static Type lastType;
        private static AspectInfo lastAspectInfo;
        //'second level cache'
        private static readonly IDictionary<Type, AspectInfo> AspectLookup = new Dictionary<Type, AspectInfo>();
    }
}