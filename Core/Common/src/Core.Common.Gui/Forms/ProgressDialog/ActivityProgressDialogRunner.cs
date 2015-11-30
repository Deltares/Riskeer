using System.Collections.Generic;
using Core.Common.Base.Workflow;
using Core.Common.Controls.Swf;

namespace Core.Common.Gui.Forms.ProgressDialog
{
    public static class ActivityProgressDialogRunner
    {
        public static void Run(IActivity activity)
        {
            Run(new[] { activity });
        }

        public static void Run(IEnumerable<IActivity> activities)
        {
            ModalHelper.ShowModal(new ActivityProgressDialog(activities));
        }
    }
}
