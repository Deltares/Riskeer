using System;
using System.ComponentModel;
using System.Threading;
using DelftTools.Utils.Aop;

namespace DelftTools.Utils.Tests.Aop.TestClasses
{
    public class InvokeRequiredTestClass : ISynchronizeInvoke, IDisposable
    {
        private readonly Thread mainThread;

        public InvokeRequiredTestClass()
        {
            mainThread = Thread.CurrentThread;
        }

        public int CallsUsingInvokeCount { get; private set; }

        public int CallsWithoutInvokeCount { get; private set; }

        public bool IsDisposed { get; set; }

        [InvokeRequired]
        public void SynchronizedMethod()
        {
            if (!InvokeRequired)
            {
                CallsWithoutInvokeCount++;
            }
        }

        public void Dispose()
        {
            using (InvokeRequiredAttribute.BlockInvokeCallsDuringDispose())
            {
                Thread.Sleep(50);
                IsDisposed = true;
            }
        }

        #region ISynchronizeInvoke Members

        public IAsyncResult BeginInvoke(Delegate method, object[] args)
        {
            return null;
        }

        public object EndInvoke(IAsyncResult result)
        {
            return null;
        }

        public object Invoke(Delegate method, object[] args)
        {
            CallsUsingInvokeCount++;
            method.DynamicInvoke(args);
            return null;
        }

        public bool InvokeRequired
        {
            get
            {
                return Thread.CurrentThread != mainThread;
            }
        }

        #endregion
    }
}