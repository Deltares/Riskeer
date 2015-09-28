using System;
using System.Globalization;
using System.Threading;

namespace DelftTools.Shell.Core.Workflow
{
    /// <summary>
    /// AsynchActivityRunner is a composition of BackGroundWorker doing the job and the Activity to be done.
    /// 
    /// TODO: merge with ActivityRunner - YAGNI
    /// </summary>
    public class AsyncActivityRunner
    {
        // private readonly BackgroundWorker backgroundWorker;

        private readonly Action<IActivity> action;
        private readonly CultureInfo uiCulture;
        private readonly CultureInfo culture;

        public AsyncActivityRunner(IActivity activity, Action<IActivity> action)
        {
            Activity = activity;
            this.action = action;

            uiCulture = Thread.CurrentThread.CurrentUICulture;
            culture = Thread.CurrentThread.CurrentCulture;
        }

        public IActivity Activity { get; private set; }

        /// <summary>
        /// A task completed succesfully if it ran without exceptions.
        /// </summary>
        public bool CompletedSuccesfully { get; private set; }

        public Exception Exception { get; set; }

        public event EventHandler Completed;

        // TODO: why do we need to pass activity and action here? Isn't it expected that task will always call Execute?

        public void Run()
        {
            ThreadPool.QueueUserWorkItem(delegate { RunAction(); });
        }

        private void RunAction()
        {
            Thread.CurrentThread.CurrentUICulture = uiCulture;
            Thread.CurrentThread.CurrentCulture = culture;

            try
            {
                action(Activity);
                CompletedSuccesfully = true;
            }
            catch (Exception e)
            {
                Exception = e;
                CompletedSuccesfully = false;
            }

            if (Completed != null)
            {
                Completed(this, EventArgs.Empty);
            }
        }

        public void Cancel()
        {
            Activity.Cancel();
        }
    }
}