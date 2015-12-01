using System.Collections.Generic;
using Core.Common.Base.Service;
using Core.Common.Controls.Swf;

namespace Core.Common.Gui.Forms.ProgressDialog
{
    public static class ActivityProgressDialogRunner
    {
        public static void Run(Activity activity)
        {
            Run(new[] { activity });
        }

        public static void Run(IEnumerable<Activity> activities)
        {
            ModalHelper.ShowModal(new ActivityProgressDialog(activities));
        }
    }
}
