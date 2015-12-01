using System;
using System.Threading;
using System.Windows.Forms;
using NUnit.Framework;

namespace Core.Common.TestUtils
{
    [TestFixture]
    public class WindowsFormsTestHelperTest
    {
        [Test]
        public void ShowActionIsRunForAFormUsingShow()
        {
            var form = new Form();
            int callCount = 0;

            WindowsFormsTestHelper.Show(form, delegate { callCount++; });

            Assert.AreEqual(1, callCount);

            WindowsFormsTestHelper.CloseAll();
        }

        [Test]
        public void ShowActionIsRunForUserControl()
        {
            var uc = new UserControl();
            int callCount = 0;

            WindowsFormsTestHelper.Show(uc, delegate { callCount++; });

            Assert.AreEqual(1, callCount);

            WindowsFormsTestHelper.CloseAll();
        }

        [Test]
        public void ShowActionIsRunForForm()
        {
            var form = new Form();
            int callCount = 0;

            WindowsFormsTestHelper.Show(form, delegate { callCount++; });

            Assert.AreEqual(1, callCount);

            WindowsFormsTestHelper.CloseAll();
        }

        [Test]
        public void ExceptionsInActionLogsStackTraceCorrectlyCorrectly()
        {
            try
            {
                WindowsFormsTestHelper.Show(new Label(), MethodWithException);
            }
            catch (Exception e)
            {
                Assert.AreEqual(typeof(InvalidOperationException), e.GetType());
                Assert.AreEqual("my message", e.Message);
                Assert.IsTrue(e.StackTrace.Contains("MethodWithException"));
            }

            WindowsFormsTestHelper.CloseAll();
        }

        [Test]
        public void UnhandledThreadExceptionsInActionLogsStackTraceCorrectlyCorrectly()
        {
            var thread = new Thread(MethodWithExceptionInSeparateThread);

            try
            {
                WindowsFormsTestHelper.Show(new Label(),
                                            delegate
                                            {
                                                thread.Start();
                                                thread.Join();
                                            });
            }
            catch (Exception e)
            {
                Assert.AreEqual(typeof(GuiTestHelper.UnhandledException), e.GetType());
                Assert.IsTrue(e.Message.Contains("my message from thread"));
                Assert.IsTrue(e.StackTrace.Contains("MethodWithExceptionInSeparateThread"));
            }

            WindowsFormsTestHelper.CloseAll();
        }

        [Test]
        public void CallActionOnShowModal()
        {
            var callCount = 0;

            WindowsFormsTestHelper.ShowModal(new Form(), delegate { callCount++; });

            Assert.AreEqual(1, callCount);
        }

        [Test]
        public void ShownCalledOnShowModal()
        {
            var callCount = 0;
            var form = new Form();

            form.Shown += delegate { callCount++; };

            WindowsFormsTestHelper.ShowModal(form);

            Assert.AreEqual(1, callCount);
        }

        [Test]
        [ExpectedException(typeof(GuiTestHelper.UnhandledException))]
        public void ExceptionIsCatchedWhenFromShownEvent()
        {
            var form = new Form();

            form.Shown += delegate { Assert.Fail(); };

            WindowsFormsTestHelper.ShowModal(form);
        }

        private void MethodWithException(Form obj)
        {
            throw new InvalidOperationException("my message");
        }

        private void MethodWithExceptionInSeparateThread()
        {
            throw new InvalidOperationException("my message from thread");
        }
    }
}