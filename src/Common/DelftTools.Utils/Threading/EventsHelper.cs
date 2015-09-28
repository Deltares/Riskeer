// © 2005 IDesign Inc. All rights reserved 
//Questions? Comments? go to 
//http://www.idesign.net

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;

namespace DelftTools.Utils.Threading
{
    ///<summary>
    /// From .net components by O'Reilly
    ///</summary>
    public static class EventsHelper
    {
        private delegate void AsyncFire(Delegate del, object[] args);

        private static void InvokeDelegate(Delegate del, object[] args)
        {
            ISynchronizeInvoke synchronizer = del.Target as ISynchronizeInvoke;
            if (synchronizer != null) //Requires thread affinity
            {
                if (synchronizer.InvokeRequired)
                {
                    synchronizer.Invoke(del, args);
                    return;
                }
            }
            //Not requiring thread afinity or invoke is not required
            del.DynamicInvoke(args);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public static void UnsafeFire(Delegate del, params object[] args)
        {
            if (args.Length > 7)
            {
                Trace.TraceWarning("Too many parameters. Consider a structure to enable the use of the type-safe versions");
            }
            if (del == null)
            {
                return;
            }
            Delegate[] delegates = del.GetInvocationList();

            foreach (Delegate sink in delegates)
            {
                InvokeDelegate(sink, args);
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public static void UnsafeFireInParallel(Delegate del, params object[] args)
        {
            if (args.Length > 7)
            {
                Trace.TraceWarning("Too many parameters. Consider a structure to enable the use of the type-safe versions");
            }
            if (del == null)
            {
                return;
            }
            Delegate[] delegates = del.GetInvocationList();

            List<WaitHandle> calls = new List<WaitHandle>(delegates.Length);
            AsyncFire asyncFire = InvokeDelegate;

            foreach (Delegate sink in delegates)
            {
                IAsyncResult asyncResult = asyncFire.BeginInvoke(sink, args, null, null);
                calls.Add(asyncResult.AsyncWaitHandle);
            }
            WaitHandle[] handles = calls.ToArray();
            WaitHandle.WaitAll(handles);
            Action<WaitHandle> close = delegate(WaitHandle handle) { handle.Close(); };
            Array.ForEach(handles, close);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public static void UnsafeFireAsync(Delegate del, params object[] args)
        {
            if (args.Length > 7)
            {
                Trace.TraceWarning("Too many parameters. Consider a structure to enable the use of the type-safe versions");
            }
            if (del == null)
            {
                return;
            }
            Delegate[] delegates = del.GetInvocationList();
            AsyncFire asyncFire = InvokeDelegate;
            AsyncCallback cleanUp = delegate(IAsyncResult asyncResult) { asyncResult.AsyncWaitHandle.Close(); };
            foreach (Delegate sink in delegates)
            {
                asyncFire.BeginInvoke(sink, args, cleanUp, null);
            }
        }

        public static void Fire<T>(EventHandler<T> del, object sender, EventArgs e) where T : EventArgs
        {
            UnsafeFire(del, sender, e);
        }

        public static void Fire(EventHandler del, object sender, EventArgs e)
        {
            UnsafeFire(del, sender, e);
        }

        public static void Fire<T>(EventHandler<T> del, object sender, T t) where T : EventArgs
        {
            UnsafeFire(del, sender, t);
        }

        public static void Fire(GenericEventHandler del)
        {
            UnsafeFire(del);
        }

        public static void Fire<T>(GenericEventHandler<T> del, T t)
        {
            UnsafeFire(del, t);
        }

        public static void Fire<T, U>(GenericEventHandler<T, U> del, T t, U u)
        {
            UnsafeFire(del, t, u);
        }

        public static void Fire<T, U, V>(GenericEventHandler<T, U, V> del, T t, U u, V v)
        {
            UnsafeFire(del, t, u, v);
        }

        public static void Fire<T, U, V, W>(GenericEventHandler<T, U, V, W> del, T t, U u, V v, W w)
        {
            UnsafeFire(del, t, u, v, w);
        }

        public static void Fire<T, U, V, W, X>(GenericEventHandler<T, U, V, W, X> del, T t, U u, V v, W w, X x)
        {
            UnsafeFire(del, t, u, v, w, x);
        }

        public static void Fire<T, U, V, W, X, Y>(GenericEventHandler<T, U, V, W, X, Y> del, T t, U u, V v, W w, X x, Y y)
        {
            UnsafeFire(del, t, u, v, w, x, y);
        }

        public static void Fire<T, U, V, W, X, Y, Z>(GenericEventHandler<T, U, V, W, X, Y, Z> del, T t, U u, V v, W w, X x,
                                                     Y y, Z z)
        {
            UnsafeFire(del, t, u, v, w, x, y, z);
        }

        public static void FireInParallel(EventHandler del, object sender, EventArgs e)
        {
            UnsafeFireInParallel(del, sender, e);
        }

        public static void FireInParallel<T>(EventHandler<T> del, object sender, T t) where T : EventArgs
        {
            UnsafeFireInParallel(del, sender, t);
        }

        public static void FireInParallel(GenericEventHandler del)
        {
            UnsafeFireInParallel(del);
        }

        public static void FireInParallel<T>(GenericEventHandler<T> del, T t)
        {
            UnsafeFireInParallel(del, t);
        }

        public static void FireInParallel<T, U>(GenericEventHandler<T, U> del, T t, U u)
        {
            UnsafeFireInParallel(del, t, u);
        }

        public static void FireInParallel<T, U, V>(GenericEventHandler<T, U, V> del, T t, U u, V v)
        {
            UnsafeFireInParallel(del, t, u, v);
        }

        public static void FireInParallel<T, U, V, W>(GenericEventHandler<T, U, V, W> del, T t, U u, V v, W w)
        {
            UnsafeFireInParallel(del, t, u, v, w);
        }

        public static void FireInParallel<T, U, V, W, X>(GenericEventHandler<T, U, V, W, X> del, T t, U u, V v, W w, X x)
        {
            UnsafeFireInParallel(del, t, u, v, w, x);
        }

        public static void FireInParallel<T, U, V, W, X, Y>(GenericEventHandler<T, U, V, W, X, Y> del, T t, U u, V v, W w,
                                                            X x, Y y)
        {
            UnsafeFireInParallel(del, t, u, v, w, x, y);
        }

        public static void FireInParallel<T, U, V, W, X, Y, Z>(GenericEventHandler<T, U, V, W, X, Y, Z> del, T t, U u, V v,
                                                               W w, X x, Y y, Z z)
        {
            UnsafeFireInParallel(del, t, u, v, w, x, y, z);
        }

        public static void FireAsync(EventHandler del, object sender, EventArgs e)
        {
            UnsafeFireAsync(del, sender, e);
        }

        public static void FireAsync<T>(EventHandler<T> del, object sender, T t) where T : EventArgs
        {
            UnsafeFireAsync(del, sender, t);
        }

        public static void FireAsync(GenericEventHandler del)
        {
            UnsafeFireAsync(del);
        }

        public static void FireAsync<T>(GenericEventHandler<T> del, T t)
        {
            UnsafeFireAsync(del, t);
        }

        public static void FireAsync<T, U>(GenericEventHandler<T, U> del, T t, U u)
        {
            UnsafeFireAsync(del, t, u);
        }

        public static void FireAsync<T, U, V>(GenericEventHandler<T, U, V> del, T t, U u, V v)
        {
            UnsafeFireAsync(del, t, u, v);
        }

        public static void FireAsync<T, U, V, W>(GenericEventHandler<T, U, V, W> del, T t, U u, V v, W w)
        {
            UnsafeFireAsync(del, t, u, v, w);
        }

        public static void FireAsync<T, U, V, W, X>(GenericEventHandler<T, U, V, W, X> del, T t, U u, V v, W w, X x)
        {
            UnsafeFireAsync(del, t, u, v, w, x);
        }

        public static void FireAsync<T, U, V, W, X, Y>(GenericEventHandler<T, U, V, W, X, Y> del, T t, U u, V v, W w, X x,
                                                       Y y)
        {
            UnsafeFireAsync(del, t, u, v, w, x, y);
        }

        public static void FireAsync<T, U, V, W, X, Y, Z>(GenericEventHandler<T, U, V, W, X, Y, Z> del, T t, U u, V v, W w,
                                                          X x, Y y, Z z)
        {
            UnsafeFireAsync(del, t, u, v, w, x, y, z);
        }
    }
}