using System.Collections.Generic;
using System.Windows.Forms;
using Core.Common.Base.Service;

namespace Core.Common.Gui.Forms.ProgressDialog
{
    public static class ActivityProgressDialogRunner
    {
        public static void Run(IWin32Window owner, Activity activity)
        {
            Run(owner, new[] { activity });
        }

        public static void Run(IWin32Window owner, IEnumerable<Activity> activities)
        {
            using (var activityProgressDialog = new ActivityProgressDialog(owner, activities))
            {
                activityProgressDialog.ShowDialog();
            }
        }
    }
}
