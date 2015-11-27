using System.Globalization;
using System.Threading;
using Core.Common.Base.Workflow;
using NUnit.Framework;
using WindowsFormApplication = System.Windows.Forms.Application;

namespace Core.Common.Base.Test.Shell.Core.WorkFlow
{
    [TestFixture]
    public class BackgroundWorkerTest
    {
        private CultureInfo originalCulture;
        private CultureInfo originalUICulture;

        [Test]
        public void TestCultureWhileDoingWork() // Note: this test fails when using System.ComponentModel.BackgroundWorker
        {
            var testFailed = false;

            // Set the thread culture before creating the background worker
            var currentCulture = new CultureInfo("nl-NL");
            var currentUiCulture = new CultureInfo("tr-TR");

            Thread.CurrentThread.CurrentCulture = currentCulture;
            Thread.CurrentThread.CurrentUICulture = currentUiCulture;

            // Create the background worker
            var backGroundWorker = new BackgroundWorker();

            backGroundWorker.DoWork += (sender, args) =>
            {
                if (!Thread.CurrentThread.CurrentCulture.Equals(currentCulture) ||
                    !Thread.CurrentThread.CurrentUICulture.Equals(currentUiCulture))
                {
                    testFailed = true;
                }
            };

            // Run the background worker
            backGroundWorker.RunWorkerAsync();

            while (backGroundWorker.IsBusy)
            {
                WindowsFormApplication.DoEvents();
                Thread.Sleep(100);
            }

            // The correct culture should have been used while "doing work"
            Assert.IsFalse(testFailed);
        }

        # region SetUp/TearDown

        [SetUp]
        public void SetUp()
        {
            originalCulture = Thread.CurrentThread.CurrentCulture;
            originalUICulture = Thread.CurrentThread.CurrentUICulture;
        }

        [TearDown]
        public void TearDown()
        {
            Thread.CurrentThread.CurrentCulture = originalCulture;
            Thread.CurrentThread.CurrentUICulture = originalUICulture;
        }

        # endregion
    }
}