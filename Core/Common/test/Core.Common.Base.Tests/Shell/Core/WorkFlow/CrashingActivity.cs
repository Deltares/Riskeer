using System;
using Core.Common.BaseDelftTools.Workflow;

namespace Core.Common.DelftTools.Tests.Shell.Core.WorkFlow
{
    public class CrashingActivity : Activity
    {
        protected override void OnInitialize()
        {
            throw new NotImplementedException();
        }

        protected override void OnExecute()
        {
            throw new NotImplementedException();
        }

        protected override void OnCancel()
        {
            throw new NotImplementedException();
        }

        protected override void OnCleanUp()
        {
            throw new NotImplementedException();
        }

        protected override void OnFinish()
        {
            throw new NotImplementedException();
        }
    }
}