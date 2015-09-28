///  Basic weak event management. 
/// 
///  Weak allow objects to be garbage collected without having to unsubscribe
///  
///  Taken with some minor variations from:
///  http://diditwith.net/2007/03/23/SolvingTheProblemWithEventsWeakEventHandlers.aspx
///  
///  use as class.theEvent +=new EventHandler<EventArgs>(instance_handler).MakeWeak((e) => class.theEvent -= e);
///  MakeWeak extension methods take an delegate to unsubscribe the handler from the event
/// 
/// 

using System;
using System.ComponentModel;

namespace DelftTools.Utils
{

    /// <summary>
    /// Delegate of an unsubscribe delegate
    /// </summary>
    public delegate void UnregisterDelegate<H>(H eventHandler) where H : class;

    /// <summary>
    /// A handler for an event that doesn't store a reference to the source
    /// handler must be a instance method
    /// </summary>
    /// <typeparam name="T">type of calling object</typeparam>
    /// <typeparam name="E">type of event args</typeparam>
    /// <typeparam name="H">type of event handler</typeparam>
    public class WeakEventHandlerGeneric<T, E, H>
        where T : class
        where E : EventArgs
        where H : class
    {

        private delegate void OpenEventHandler(T @this, object sender, E e);

        private delegate void LocalHandler(object sender, E e);

        private WeakReference m_TargetRef;
        private OpenEventHandler m_OpenHandler;
        private H m_Handler;
        private UnregisterDelegate<H> m_Unregister;

        public WeakEventHandlerGeneric(H eventHandler, UnregisterDelegate<H> unregister)
        {
            m_TargetRef = new WeakReference((eventHandler as Delegate).Target);
            m_OpenHandler = (OpenEventHandler)Delegate.CreateDelegate(typeof(OpenEventHandler), null, (eventHandler as Delegate).Method);
            m_Handler = CastDelegate(new LocalHandler(Invoke));
            m_Unregister = unregister;
        }

        private void Invoke(object sender, E e)
        {
            T target = (T)m_TargetRef.Target;

            if (target != null)
                m_OpenHandler.Invoke(target, sender, e);
            else if (m_Unregister != null)
            {
                m_Unregister(m_Handler);
                m_Unregister = null;
            }
        }

        /// <summary>
        /// Gets the handler.
        /// </summary>
        public H Handler
        {
            get { return m_Handler; }
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="PR.utils.WeakEventHandler&lt;T,E&gt;"/> to <see cref="System.EventHandler&lt;E&gt;"/>.
        /// </summary>
        /// <param name="weh">The weh.</param>
        /// <returns>The result of the conversion.</returns>
        public static implicit operator H(WeakEventHandlerGeneric<T, E, H> weh)
        {
            return weh.Handler;
        }

        /// <summary>
        /// Casts the delegate.
        /// Taken from
        /// http://jacobcarpenters.blogspot.com/2006/06/cast-delegate.html
        /// </summary>
        /// <param name="source">The source.</param>
        /// <returns></returns>
        public static H CastDelegate(Delegate source)
        {
            if (source == null) return null;

            Delegate[] delegates = source.GetInvocationList();
            if (delegates.Length == 1)
                return Delegate.CreateDelegate(typeof(H), delegates[0].Target, delegates[0].Method) as H;

            for (int i = 0; i < delegates.Length; i++)
                delegates[i] = Delegate.CreateDelegate(typeof(H), delegates[i].Target, delegates[i].Method);

            return Delegate.Combine(delegates) as H;
        }
    }

    #region Weak Generic EventHandler<Args> handler

    /// <summary>
    /// An interface for a weak event handler
    /// </summary>
    /// <typeparam name="E"></typeparam>
    public interface IWeakEventHandler<E> where E : EventArgs
    {
        EventHandler<E> Handler { get; }
    }

    /// <summary>
    /// A handler for an event that doesn't store a reference to the source
    /// handler must be a instance method
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="E"></typeparam>
    public class WeakEventHandler<T, E> : WeakEventHandlerGeneric<T, E, EventHandler<E>>, IWeakEventHandler<E>
        where T : class
        where E : EventArgs
    {

        public WeakEventHandler(EventHandler<E> eventHandler, UnregisterDelegate<EventHandler<E>> unregister)
            : base(eventHandler, unregister) { }
    }

    #endregion

    #region Weak PropertyChangedEvent handler

    /// <summary>
    /// An interface for a weak event handler
    /// </summary>
    /// <typeparam name="E"></typeparam>
    public interface IWeakPropertyChangedEventHandler
    {
        PropertyChangedEventHandler Handler { get; }
    }

    /// <summary>
    /// A handler for an event that doesn't store a reference to the source
    /// handler must be a instance method
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="E"></typeparam>
    public class WeakPropertyChangeHandler<T> : WeakEventHandlerGeneric<T, PropertyChangedEventArgs, PropertyChangedEventHandler>, IWeakPropertyChangedEventHandler
     where T : class
    {

        public WeakPropertyChangeHandler(PropertyChangedEventHandler eventHandler, UnregisterDelegate<PropertyChangedEventHandler> unregister)
            : base(eventHandler, unregister) { }
    }

    #endregion

    /// <summary>
    /// Utilities for the weak event method
    /// </summary>
    public static class WeakEventExtensions
    {

        private static void CheckArgs(Delegate eventHandler, Delegate unregister)
        {
            if (eventHandler == null) throw new ArgumentNullException("eventHandler");
            if (eventHandler.Method.IsStatic || eventHandler.Target == null) throw new ArgumentException("Only instance methods are supported.", "eventHandler");
        }

        private static object GetWeakHandler(Type generalType, Type[] genericTypes, Type[] constructorArgTypes, object[] constructorArgs)
        {
            var wehType = generalType.MakeGenericType(genericTypes);
            var wehConstructor = wehType.GetConstructor(constructorArgTypes);
            return wehConstructor.Invoke(constructorArgs);
        }

        /// <summary>
        /// Makes a property change handler weak
        /// </summary>
        /// <typeparam name="E"></typeparam>
        /// <param name="eventHandler">The event handler.</param>
        /// <param name="unregister">The unregister.</param>
        /// <returns></returns>
        public static PropertyChangedEventHandler MakeWeak(this PropertyChangedEventHandler eventHandler, UnregisterDelegate<PropertyChangedEventHandler> unregister)
        {
            CheckArgs(eventHandler, unregister);

            var generalType = typeof(WeakPropertyChangeHandler<>);
            var genericTypes = new[] { eventHandler.Method.DeclaringType };
            var constructorTypes = new[] { typeof(PropertyChangedEventHandler), typeof(UnregisterDelegate<PropertyChangedEventHandler>) };
            var constructorArgs = new object[] { eventHandler, unregister };

            return ((IWeakPropertyChangedEventHandler)GetWeakHandler(generalType, genericTypes, constructorTypes, constructorArgs)).Handler;
        }

        /// <summary>
        /// Makes a generic handler weak
        /// </summary>
        /// <typeparam name="E"></typeparam>
        /// <param name="eventHandler">The event handler.</param>
        /// <param name="unregister">The unregister.</param>
        /// <returns></returns>
        public static EventHandler<E> MakeWeak<E>(this EventHandler<E> eventHandler, UnregisterDelegate<EventHandler<E>> unregister) where E : EventArgs
        {
            CheckArgs(eventHandler, unregister);

            var generalType = typeof(WeakEventHandler<,>);
            var genericTypes = new[] { eventHandler.Method.DeclaringType, typeof(E) };
            var constructorTypes = new[] { typeof(EventHandler<E>), typeof(UnregisterDelegate<EventHandler<E>>) };
            var constructorArgs = new object[] { eventHandler, unregister };

            return ((IWeakEventHandler<E>)GetWeakHandler(generalType, genericTypes, constructorTypes, constructorArgs)).Handler;
        }
    }
}
