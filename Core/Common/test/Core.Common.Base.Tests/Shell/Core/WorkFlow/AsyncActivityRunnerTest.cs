using System.Globalization;
using System.Threading;
using Core.Common.BaseDelftTools.Workflow;
using NUnit.Framework;
using Rhino.Mocks;
using WindowsFormsApplication = System.Windows.Forms.Application;

namespace Core.Common.Base.Tests.Shell.Core.WorkFlow
{
    [TestFixture]
    public class AsyncActivityRunnerTest
    {
        private static readonly MockRepository mocks = new MockRepository();

        [Test]
        public void DoesNotCompleteSuccesfullyIfAnExceptionWasThrown()
        {
            var activity = new CrashingActivity();

            var asynchTask = new AsyncActivityRunner(activity, a => a.Execute());
            int callCount = 0;
            asynchTask.Completed += (s, e) =>
            {
                callCount++;
                Assert.IsFalse(((AsyncActivityRunner) s).CompletedSuccesfully);
            };
            asynchTask.Run();

            Thread.Sleep(100);

            //do events...otherwise taskcompleted wont run
            WindowsFormsApplication.DoEvents();

            Assert.AreEqual(1, callCount);
        }

        [Test]
        public void ShouldSetUICultureAsInMainThread()
        {
            var activityFinished = false;
            var uiCultureIsSet = false;
            var activity = mocks.Stub<IActivity>();

            var oldUICulture = Thread.CurrentThread.CurrentUICulture;
            Assert.AreNotEqual("nl-NL", oldUICulture.Name);

            try
            {
                Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo("nl-NL");

                activity.Expect(a => a.Execute()).Callback(
                    delegate
                    {
                        uiCultureIsSet = "nl-NL" == Thread.CurrentThread.CurrentUICulture.Name;
                        activityFinished = true;
                        return false;
                    });

                mocks.ReplayAll();

                var task = new AsyncActivityRunner(activity, a => a.Execute());
                task.Run();
            }
            finally
            {
                // restore culture
                Thread.CurrentThread.CurrentUICulture = oldUICulture;
            }

            while (!activityFinished)
            {
                Thread.Sleep(0);
            }

            Assert.AreEqual(true, uiCultureIsSet, "UI culture is not set in background thread");
        }
    }
}