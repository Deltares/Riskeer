using System;
using System.Threading;
using System.Windows.Forms;
using DelftTools.TestUtils;
using NUnit.Framework;

namespace DelftTools.Tests.TestUtils
{
    [TestFixture]
    public class WindowsFormsTestHelperTest
    {
        [Test]
        [Category(TestCategory.WindowsForms)]
        public void ShowActionIsRunForAFormUsingShow()
        {
            var form = new Form();
            int callCount = 0;
            WindowsFormsTestHelper.Show(form, delegate
                                                  {
                                                      callCount++;
                                                  });
            //assert the show action was called
            Assert.AreEqual(1,callCount);

            WindowsFormsTestHelper.CloseAll();
        }

        [Test]
        [Category(TestCategory.WindowsForms)]
        public void ShowActionIsRunForUserControl()
        {
            var uc = new UserControl();
            int callCount = 0;
            WindowsFormsTestHelper.Show(uc, delegate
            {
                callCount++;
            });
            //assert the show action was called
            Assert.AreEqual(1, callCount);

            WindowsFormsTestHelper.CloseAll();
        }

        [Test]
        [Category(TestCategory.WindowsForms)]
        public void ShowActionIsRunForForm()
        {
            var form = new Form();
            int callCount = 0;
            WindowsFormsTestHelper.Show(form, delegate
            {
                callCount++;
            });
            //assert the show action was called
            Assert.AreEqual(1, callCount);

            WindowsFormsTestHelper.CloseAll();
        }

        [Test]
        [Category(TestCategory.WindowsForms)]
        public void ExceptionsInActionLogsStackTraceCorrectlyCorrectly()
        {
            try
            {
                WindowsFormsTestHelper.Show(new Label(), MethodWithException);
            }
            catch(Exception e)
            {
                Assert.AreEqual(typeof(InvalidOperationException), e.GetType());
                Assert.AreEqual("my message", e.Message);
                Assert.IsTrue(e.StackTrace.Contains("MethodWithException"));
            }

            WindowsFormsTestHelper.CloseAll();
        }

        private void MethodWithException(Form obj)
        {
            throw new InvalidOperationException("my message");
        }

        [Test]
        [Category(TestCategory.WindowsForms)]
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

        private void MethodWithExceptionInSeparateThread()
        {
            throw new InvalidOperationException("my message from thread");
        }

        [Test]
        [Category(TestCategory.WindowsForms)]
        public void CallActionOnShowModal()
        {
            var form = new Form();
            var callCount = 0;
            Action<Form> action = delegate { callCount++; };
            WindowsFormsTestHelper.ShowModal(form, action);
            
            //assert the show action was called
            Assert.AreEqual(1, callCount);
        }

        [Test]
        [Category(TestCategory.WindowsForms)]
        public void ShownCalledOnShowModal()
        {
            var form = new Form();
            var callCount = 0;

            form.Shown += delegate { callCount++; };

            WindowsFormsTestHelper.ShowModal(form);

            //assert the show action was called
            Assert.AreEqual(1, callCount);
        }

        [Test]
        [Category(TestCategory.WindowsForms)]
        public void CheckEventsOnBuildServer()
        {
            var form = new Form();

            form.Shown += delegate { Console.WriteLine("Shown"); };
            form.Paint += delegate { Console.WriteLine("Paint"); };
            form.Load += delegate { Console.WriteLine("Load"); };
            form.VisibleChanged += delegate { Console.WriteLine("VisibleChanged"); };

            WindowsFormsTestHelper.ShowModal(form);
        }

        [Test]
        [Category(TestCategory.WindowsForms)]
        [ExpectedException(typeof(GuiTestHelper.UnhandledException))]
        public void ExceptionIsCatchedWhenFromShownEvent()
        {
            var form = new Form();

            form.Shown += delegate { Assert.Fail(); };

            WindowsFormsTestHelper.ShowModal(form);
        }
    }
}
