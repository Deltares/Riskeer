using System.Threading;
using System.Windows.Forms;
using Core.Common.Utils.Aop;
using Core.Common.Utils.Test.Aop.TestClasses;
using NUnit.Framework;

namespace Core.Common.Utils.Test.Aop
{
    [TestFixture]
    public class InvokeRequiredAttributeTest
    {
        [Test]
        public void InvokeMethodInASeparateThread()
        {
            var testClass = new InvokeRequiredTestClass();

            // call directly 
            testClass.SynchronizedMethod();

            // call from another thread
            var thread = new Thread(testClass.SynchronizedMethod);
            thread.Start();
            thread.Join();

            Assert.AreEqual(1, testClass.CallsUsingInvokeCount);
            Assert.AreEqual(1, testClass.CallsWithoutInvokeCount);
        }

        [Test]
        public void InvokeDuringAndAfterDispose()
        {
            InvokeRequiredInfo.WaitMethod = Application.DoEvents;

            var testClass = new InvokeRequiredTestClass();

            // call from another thread
            var thread = new Thread(() =>
            {
                // 10 normal calls
                for (int i = 0; i < 10; i++)
                {
                    testClass.SynchronizedMethod();
                }
                // dispose (from wrong thread..but ok)
                testClass.Dispose();
                // subsequent calls are skipped:
                for (int i = 0; i < 10; i++)
                {
                    testClass.SynchronizedMethod();
                }
            });
            thread.Start();
            thread.Join();

            Assert.AreEqual(20, testClass.CallsUsingInvokeCount);
            Assert.AreEqual(0, testClass.CallsWithoutInvokeCount);
        }
    }
}