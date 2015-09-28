using System;
using System.Collections.Generic;
using System.Threading;
using DelftTools.Shell.Core.Workflow;

namespace DeltaShell.Tests.Gui.Forms.ProgressDialog
{
    public class TestStringBasedProgressingActivity : Activity
    {
        private string progressText;

        protected override void OnInitialize()
        {
        }

        protected override void OnExecute()
        {
            foreach (var s in AapNootMies())
            {
                progressText = s;
                if (ProgressChanged != null)
                {
                    ProgressChanged(this, EventArgs.Empty);
                }
                Thread.Sleep(200);//make the change last a little
            }

            Status = ActivityStatus.Done;
        }

        protected override void OnCancel()
        {
            throw new NotImplementedException();
        }

        protected override void OnCleanUp()
        {
        }

        protected override void OnFinish()
        {
        }

        public string GetProgressText()
        {
            return progressText;
        }
        
        public IEnumerable<string> AapNootMies()
        {
            yield return "Aap";
            yield return "Noot";
            yield return "Mies";
            yield return "Teun";
            yield return "Jet";
            yield return "Vuur?";
        }

        public event EventHandler ProgressChanged;
    }
}