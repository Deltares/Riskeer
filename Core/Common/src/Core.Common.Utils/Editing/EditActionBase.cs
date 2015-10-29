using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Core.Common.Utils.Editing
{
    public abstract class EditActionBase : IEditAction
    {
        private static readonly IDictionary<Type, Func<IEditAction>> CreationCache = new Dictionary<Type, Func<IEditAction>>();

        protected EditActionBase(string name)
        {
            Name = name;
        }

        public string Name { get; set; }

        public virtual object Instance { get; set; }

        public virtual object[] Arguments { get; set; }

        public object ReturnValue { get; set; }

        public virtual bool HandlesRestore
        {
            get
            {
                return false;
            }
        }

        public static IEditAction Create(Type type)
        {
            Func<IEditAction> func;

            if (!CreationCache.TryGetValue(type, out func))
            {
                //using compiled lambda expressions, this code creates a cached version of the 
                //constructor for editactions. Although the initial construction takes quite some 
                //time, in the long run it's significantly quicker than using Activator.CreateInstance
                var construct = Expression.New(type.GetConstructor(Type.EmptyTypes));
                var cast = Expression.TypeAs(construct, typeof(IEditAction));
                func = (Func<IEditAction>) Expression.Lambda(cast).Compile();
                CreationCache.Add(type, func);
            }

            return func();
        }

        public virtual void BeforeChanges() {}
    }
}