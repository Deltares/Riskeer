using System;
using System.Diagnostics;
using System.Threading;
using DelftTools.Shell.Core.Workflow;

namespace DeltaShell.Tests.Gui.Forms.ProgressDialog
{
    public class TestActivity : Activity
    {
        private readonly TimeSpan initializationTime;
        private readonly TimeSpan executionTime;
        private bool doCancel;

        public TestActivity(int initializationTimeInSeconds, int executionTimeInSeconds, string name)
        {
            doCancel = false;
            initializationTime = new TimeSpan(0, 0, initializationTimeInSeconds);
            executionTime = new TimeSpan(0, 0, executionTimeInSeconds);
            Name = name;
        }

        protected override void OnInitialize()
        {
            Log("Starting initialization");
            var start = DateTime.Now;
            while ((DateTime.Now - start) < initializationTime)
            {
                if (doCancel)
                {
                    Log("Cancelled");
                    return; //todo set status etc
                }
                Thread.Sleep(100); //sleep it off :)
            }
            Log("Finished initialization");
        }

        protected override void OnExecute()
        {
            Log("Started execution");
            var start = DateTime.Now;
            while ((DateTime.Now - start) < executionTime)
            {
                if (doCancel)
                {
                    Log("Cancelled");
                    return; //todo set status etc
                }
                Thread.Sleep(100); //sleep it off :)
            }
            Log("Finished execution");

            Status = ActivityStatus.Done;
        }

        protected override void OnCancel()
        {
            doCancel = true;
        }

        protected override void OnCleanUp() {}

        protected override void OnFinish() {}

        private void Log(string message)
        {
            Debug.WriteLine(string.Format("{0}:{1}", Name, message));
        }
    }
}